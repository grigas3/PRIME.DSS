using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// 
    /// </summary>
    public class ConditionClassifierFactory
    {
        private readonly List<ConditionClassifier> _conditionClassifiers = new List<ConditionClassifier>();

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            var files = Directory.GetFiles(path, "*.json");


            foreach (var c in files)
            {
                var json = File.ReadAllText(c);
                _conditionClassifiers.Add(JsonConvert.DeserializeObject<ConditionClassifier>(json));
            }
        }



        /// <summary>
        /// Get Condition Options
        /// </summary>
        /// <param name="treatment"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<Condition> GetConditions(IConditionRepository repository)
        {
            List<Condition> options = new List<Condition>();
            foreach (var c in _conditionClassifiers)
            {
                List<NaiveVarInstance> vInputs = new List<NaiveVarInstance>();

                foreach (var v in c.Variables)
                {

                        var b = repository.HasCondition(v.Code.ToLower(),null);

                        if (b.HasValue)
                            vInputs.Add(new NaiveVarInstance()
                            {
                                Code = v.Code.ToLower(),
                                Name = v.Name,
                                Present = b.Value
                            });
                }


                var p = c.GetOutput(vInputs);
               
                
                    options.Add(new Condition()
                    {
                        Name = c.Name,
                        Code = c.Code.ToLower(),
                        Probability = p
                    });
                
            }

            return options;
        }


        /// <summary>
        /// Get Condition Options
        /// </summary>
        /// <param name="treatment"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<Condition> GetConditionOptions(TreatmentOption treatment, IConditionRepository repository)
        {
            List<Condition> options = new List<Condition>();
            foreach (var c in _conditionClassifiers)
            {
                List<NaiveVarInstance> vInputs = new List<NaiveVarInstance>();

                foreach (var v in c.Variables)
                {
                    if (treatment != null && treatment.Option == TreatmentAdmission.Add &&
                        v.Code.ToLower() == treatment.Code.ToLower())
                    {
                        vInputs.Add(new NaiveVarInstance() {Code = v.Code.ToLower(), Name = v.Name, Present = true});
                    }
                    else if (treatment != null && treatment.Option == TreatmentAdmission.Remove &&
                             v.Code.ToLower() == treatment.Code.ToLower())
                    {
                        vInputs.Add(new NaiveVarInstance() {Code = v.Code.ToLower(), Name = v.Name, Present = false});
                    }
                    else if (treatment != null && treatment.Option == TreatmentAdmission.Replace &&
                             v.Code == treatment.ReplacementCode)
                    {
                        vInputs.Add(new NaiveVarInstance()
                            {Code = treatment.ReplacementCode, Name = v.Name, Present = false});
                    }
                    else if (treatment != null && treatment.Option == TreatmentAdmission.Replace &&
                             v.Code.ToLower() == treatment.Code.ToLower())
                    {
                        vInputs.Add(new NaiveVarInstance()
                            {Code = treatment.Code.ToLower(), Name = v.Name, Present = true});
                    }
                    else
                    {
                        var b = repository.HasCondition(v.Code.ToLower(),null);

                        if (b.HasValue)
                            vInputs.Add(new NaiveVarInstance()
                                {Code = v.Code.ToLower(), Name = v.Name, Present = b.Value});
                    }
                }


                var p = c.GetOutput(vInputs);
                if (p > 0.1)
                {
                    options.Add(new Condition()
                    {
                        Treatment = treatment,
                        Name = c.Name,
                        Code = c.Code.ToLower(),
                        Probability = p
                    });
                }
            }

            return options;
        }
    }
}