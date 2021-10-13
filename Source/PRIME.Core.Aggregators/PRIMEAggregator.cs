using PRIME.Core.Common.Exceptions;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRIME.Core.Common.Extensions;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.Aggregators
{
   

    /// <summary>
    /// UPDRS Score Aggregator
    /// </summary>
    public class PRIMEAggregator : IAggregator
    {
        #region Private Const declarations

        private const int timeInterval = 30 * 60 * 1000;
        private const int dayInterval = 60 * 60 * 24 * 1000;

        /// <summary>
        /// Time aggregation type value
        /// </summary>
        private const string TimeAggregationType = "time";

        /// <summary>
        /// Day aggregation type value
        /// </summary>
        private const string DayAggregationType = "day";

        /// <summary>
        /// Total aggregation type value
        /// </summary>
        private const string TotalAggregationType = "total";


        private readonly IGenericLogger _logger;

        private const int MAXTAKE = 1000;

        #endregion


        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="proxy">Data proxy</param>
        /// <param name="logger"></param>
        /// <param name="aggrDefinitionDictionary"></param>
        public PRIMEAggregator(IGenericLogger logger)
        {
            this._logger = logger;
        }

     
        /// <summary>
        /// Run Aggregation
        /// This method 
        /// 1) loads the aggregation definition
        /// 2) Fetch all required observations using the DataProxy
        /// 3) Calls the PerformAggregation method to perform the aggregation and returns a new observation
        /// 
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="code">Meta observation Code</param>
        /// <param name="lastExecutionTime"></param>
        /// <param name="aggregationType">Overrides the default aggregation type</param>
        /// <param name="filterType">Overrides the default filter type</param>
        /// <returns></returns>
        public async Task<IObservation> RunSingle(string patientId, string code, string codeNamespace,
            IEnumerable<PDObservation> rawObs, string configStr)
        {
            if (configStr == null) throw new ArgumentNullException(nameof(configStr));


            var config = AggrConfig.FromString(configStr);
            List<PDObservation> observations = new List<PDObservation>();
          //  foreach (var c in config.Variables)
          //  {
                try
                {
                    if (string.IsNullOrEmpty(config.Code))
                    throw new ArgumentNullException("Code");

                if (string.IsNullOrEmpty(config.CodeNameSpace))
                    throw new ArgumentNullException("Config");

                //Get Observations For patient
                var ret = rawObs.Where((e =>
                        e.Code.ToLower() == config.Code.ToLower() && e.CodeNameSpace.ToLower() == config.CodeNameSpace.ToLower()));
                    observations.AddRange(ret);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error in aggregation");
                }
      //      }


            var metaObservation = PerformAggregation(config, patientId, observations);
            metaObservation.Code = config.OutputCode;
            metaObservation.CodeNameSpace = "PRIME";
            

            return metaObservation;
        }

        /// <summary>
        /// Total Aggregation
        /// </summary>
        /// <param name="definition">Aggregation definition</param>
        /// <param name="patientId">Patient Id</param>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="observations">Observations</param>
        /// <returns></returns>
        private IEnumerable<IObservation> PerformTotalAggregation(AggrConfig definition, string patientId, long timestamp, IEnumerable<PDObservation> observations)
        {

            double v = definition.Beta;
            foreach (var c in definition.Variables)
            {

                var v1 = c.Weight * observations.Where(e => e.Code == c.Code).Select(e => e.Value).DefaultIfEmpty().Average();
                v += v1;

            }
            return new List<IObservation>() { new PDObservation() { Value = v, PatientId = patientId, Code = definition.Code, Timestamp = timestamp } };



        }
        /// <summary>
        /// PerformAggregation Aggregation
        /// </summary>
        /// <param name="definition">Aggregation Definition</param>
        /// <param name="patientId">Patient Id</param>
        /// <param name="observations">Observations</param>
        /// <returns></returns>
        private IObservation PerformAggregation(AggrConfig definition, string patientId,
            IEnumerable<PDObservation> observations)
        {
            //-----------------
            // 1st Level Aggregation
            //------------
            List<PDObservation> metaObservations = observations.ToList();
           

            if (metaObservations == null)
            {
                //Something wrong happened!!
                throw new Exception();
            }

            //---------------------
            //Threshold meta-observations
            //---------------------            
            Thresholding(definition, metaObservations);


            //---------------------
            //Level aggregation
            //---------------------      
            var obs=MetaAggregation(definition, patientId, metaObservations);

            obs.Description = $"{definition.Code} {metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Average()}";

            obs.Code = definition.OutputCode;
            obs.CodeNameSpace = "PRIME";
            return obs;
        }


        private void Thresholding(AggrConfig definition, List<PDObservation> metaObservations)
        {
            if (metaObservations == null)
                return;

            var mean = metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Average();
            var q2 = metaObservations.Select(e => e.Value * e.Value).DefaultIfEmpty(0).Average();
            int n = metaObservations.Count();
            var std = q2 / n - mean * mean;


            //-----------------
            //THRESHOLDING
            //-----------------
            //If not thresholding required return mean
            if (definition.Threshold)
            {
                //Set threshold based on threshold type and value
                var threshold = definition.ThresholdValue;

                if (definition.ThresholdType == "std")
                {
                    threshold = mean + definition.ThresholdValue * std;
                }

                //New observation timestamp
                foreach (var obs in metaObservations)
                {
                    obs.Value = (obs.Value > threshold) ? 1.0 : 0.0;
                }
            }
        }


        /// <summary>
        /// 2nd Level Aggregation
        /// </summary>
        /// <param name="definition">Aggregation Definition</param>
        /// <param name="patientId">Patient Id</param>
        /// <param name="metaObservations">Observations</param>
        /// <returns></returns>
        private PDObservation MetaAggregation(AggrConfig definition, string patientId,
            List<PDObservation> metaObservations)
        {
            if (string.IsNullOrEmpty(definition.MetaAggregationType))
                throw new NotSupportedException();

            var metaAggr = definition.MetaAggregationType.ToLower();

            var endT = metaObservations.Select(e => e.Timestamp).DefaultIfEmpty().Max();
            int n = metaObservations.Count();
            if (metaAggr == "sum")
            {
                return 
                    new PDObservation()
                    {
                        Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
                        Value = definition.Scale( metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Sum())
                
                        };
            }
            else if (metaAggr == "average")
            {
                return 
                
                    new PDObservation()
                    {
                        Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
                        Value = definition.Scale(metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Average())
                    
                };
            }

            //else if (metaAggr == "mfi")
            //{
            //    return
                
            //        new PDObservation()
            //        {
            //            Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
            //            Value = definition.MetaScale *
            //                    (metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Max() -
            //                     metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Average())
                    
            //    };
            //}
            else if (metaAggr == "count")
            {
                return 
                
                    new PDObservation()
                    {
                        Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
                        Value = definition.Scale(metaObservations.Count())
                    
                };
            }

            else if (metaAggr == "any")
            {
                return 
                
                    new PDObservation()
                    {
                        Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
                        Value = metaObservations.Any() ? 1 : 0
                    
                };
            }
            else if (metaAggr == "max")
            {
                return 
                
                    new PDObservation()
                    {
                        Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
                        Value = definition.Scale(metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Max())
                    
                };
            }
            else if (metaAggr == "min")
            {
                return
                
                    new PDObservation()
                    {
                        Code = definition.Code, Weight = n, PatientId = patientId, Timestamp = endT,
                        Value = definition.Scale(metaObservations.Select(e => e.Value).DefaultIfEmpty(0).Min())
                    
                };
            }
          
            else
            {
                throw new NotSupportedException();
            }
        }




        public Task<IEnumerable<IObservation>> Run(string patientId, string code, string codeNamepace,
            DateTime? lastExecutionTime, string aggregationType = null,
            string filterType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IObservation>> Run(string patientId, string code, string codeNamespace, IEnumerable<PDObservation> rawObs, string config)
        {
            throw new NotImplementedException();
        }

       
    }
}