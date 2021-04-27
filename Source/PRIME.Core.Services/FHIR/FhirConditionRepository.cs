using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Itenso.TimePeriod;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PRIME.Core.Common.Interfaces;
using Date = Itenso.TimePeriod.Date;
using Quantity = Hl7.Fhir.ElementModel.Types.Quantity;

namespace PRIME.Core.Services.FHIR
{
    public class FhirConditionRepository : IConditionRepository
    {
        #region Readonly Properties
        private readonly FhirProxyConfiguration _proxyConfiguration;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="proxyConfiguration"></param>
        public FhirConditionRepository(FhirProxyConfiguration proxyConfiguration)
        {
            _proxyConfiguration = proxyConfiguration;
        }


        private static bool MatchCode(CodeableConcept concept, string code, string system)
        {
            foreach (var c in concept.Coding)
            {
                if (c.Code == code && c.System == system)
                    return true;
            }
            return false;
        }


        public bool HasBundle()
        {
            return _bundle != null;
        }
        /// <summary>
        /// Init Repository
        /// This is a Patient bundle with all patient information inside
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Init(string id)
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in _proxyConfiguration.Headers)
            {
                headers.Add(header.Key, header.Value);
            }

            if (_proxyConfiguration.RequiresAuthentication)
            {
                var token =await PrimeFhirClient.Authenticate(_proxyConfiguration.AuthUrl,
                    _proxyConfiguration.UserName,
                    _proxyConfiguration.Password);

                if (token == null)
                    throw new Exception("Authentication error");
                headers.Add("Authorization", "Bearer " + token);
            }

            var bundle = await PrimeFhirClient.GetBundle(_proxyConfiguration.Url, id, headers);
            _bundle = bundle;

        }

        /// <summary>
        /// Set Bundle
        /// </summary>
        /// <param name="bundle"></param>
        public void SetBundle(Bundle bundle)
        {

            _bundle = bundle;
        }


        private Bundle _bundle;

        private object GetPatientProperty(Patient patient, string property)
        {
            if (property == "Age")
            {
                var t = patient.BirthDateElement.ToDateTimeOffset();
                if (!t.HasValue)
                    return null;
                return new DateDiff(DateTime.Now, t.Value.Date).ElapsedYears;
            }
            if (property == "Male")
            {
                return patient.Gender == AdministrativeGender.Male;
            }

            if (property == "Female")
            {
                return patient.Gender == AdministrativeGender.Female;
            }

            return null;

        }

        /// <summary>
        /// Has Condition
        /// Currently supports only patient, Observation and Condition
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public bool? HasCondition(string code, string system)
        {
            foreach (var s in _bundle.Entry)
            {
                if ((s.Resource is Patient) && system == "Patient")
                {
                    return (bool?) GetPatientProperty(s.Resource as Patient, code);
                }

                else if ((s.Resource is Observation) && (s.Resource as Observation).Code!=null&&MatchCode((s.Resource as Observation).Code, code, system))
                {
                    return true;
                }
                else if ((s.Resource is Condition) && MatchCode((s.Resource as Condition).Code, code, system)
                                                   && (s.Resource as Condition).ClinicalStatus!=null && (s.Resource as Condition).ClinicalStatus.Coding!=null&&(s.Resource as Condition).ClinicalStatus.Coding.Any(c => c.Code.ToLower() == "active"))
                {
                    return true;
                }
            }

            return false;
        }

        private class ObsInfo
        {
            public  bool Value { get; set; }
            public long  Timestamp { get; set; }
        }
     

        /// <summary>
        /// Has Condition With Converter
        /// Currently supports only patient and Observation
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public bool? HasCondition(string code, string system, Func<object, bool> convert)
        {
            List<ObsInfo> observations = new List<ObsInfo>();

            foreach (var s in _bundle.Entry)
            {
                if(s.Resource==null)
                    continue;
                
                if ((s.Resource is Patient) && system == "Patient")
                {
                    return convert.Invoke(GetPatientProperty(s.Resource as Patient, code));
                }

                else if ((s.Resource is Observation) && MatchCode((s.Resource as Observation).Code, code, system) )
                {
                    var r = false;
                    var obs = s.Resource as Observation;
                    if (obs.Value == null)
                        continue;

                    if (obs.Status.HasValue&&(obs.Status==ObservationStatus.Cancelled|| obs.Status == ObservationStatus.EnteredInError || obs.Status == ObservationStatus.Amended))
                    {
                        continue;
                        
                    }

                    long timestamp = 0;
                    if (obs.Issued.HasValue)
                    {
                        timestamp= obs.Issued.Value.ToUnixTimeSeconds();
                    }
                    var e = (Hl7.Fhir.Model.Quantity)obs.Value;

                    if (!e.Value.HasValue)
                        continue;
                    r = convert.Invoke(e.Value.Value);

                  observations.Add(new ObsInfo(){
                      Value =r,
                      Timestamp = timestamp});
                   


                }
                else if ((s.Resource is Condition) && MatchCode((s.Resource as Condition).Code, code, system)
                                                   && (s.Resource as Condition).Category.Any(e => e.Coding.Any(c => c.Code == "Active")))
                {
                    throw new NotSupportedException();
                }

                if (observations.Count > 0)
                {
                    return observations.OrderByDescending(e => e.Timestamp).FirstOrDefault().Value;

                }
            }

            return false;
        }
    }

}
