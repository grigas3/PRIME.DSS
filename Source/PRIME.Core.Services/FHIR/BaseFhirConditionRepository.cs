using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Itenso.TimePeriod;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace PRIME.Core.Services.FHIR
{
    public abstract class BaseFhirConditionRepository : IConditionRepository
    {
        #region Readonly Properties
       


        private readonly List<IObservation> _metaObservations = new List<IObservation>();
        #endregion

  

        private static bool MatchCode(CodeableConcept concept, string code, string system)
        {
            foreach (var c in concept.Coding)
            {

                if (string.IsNullOrEmpty(c.Code))
                    continue;



                if (c.Code.ToLower() == code.ToLower() && c.System == system)
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
        public abstract Task Init(string id);
    


        /// <summary>
        /// Set Bundle
        /// </summary>
        /// <param name="bundle"></param>
        public void SetBundle(Bundle bundle)
        {

            _bundle = bundle;
        }


        private Bundle _bundle;

        private PDObservation GetBirthDateElement(Patient patient)
        {
            if (patient.BirthDateElement == null)
                return null;
            var t = patient.BirthDateElement.ToDateTimeOffset();
            if (!t.HasValue)
                return null;

            return new PDObservation()
            {
                Code = "AGE",
                Value = new DateDiff(DateTime.Now, t.Value.Date).ElapsedYears
            };
        }

        private IEnumerable<PDObservation> GetPatientPropertiesAsObs(Patient patient)
        {
            var ret = new List<PDObservation>();
            {
                var t = GetBirthDateElement(patient);
                if (t != null)
                    ret.Add(t);
            }
            {
                ret.Add(new PDObservation()
                {
                    Code = "MALE",
                    Value = patient.Gender == AdministrativeGender.Male ? 1 : 0,
                
                });
            }

            {
                ret.Add(new PDObservation()
                {
                    Code = "FEMALE",
                    Value = patient.Gender == AdministrativeGender.Female ? 1 : 0
                });
            }

            return ret;

        }

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
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (system == null) throw new ArgumentNullException(nameof(system));


            var mobs = _metaObservations.FirstOrDefault((e => e.Code.ToLower() == code && e.CodeNameSpace.ToLower() == system.ToLower()));
            if (mobs != null)
            {
                return mobs.Value > 0;
            }

            foreach (var s in _bundle.Entry)
            {
                if ((s.Resource is Patient) && system == "Patient")
                {
                    return (bool?)GetPatientProperty(s.Resource as Patient, code);
                }

                else if ((s.Resource is Observation) && (s.Resource as Observation).Code != null && MatchCode((s.Resource as Observation).Code, code, system))
                {
                    return true;
                }
                else if ((s.Resource is Condition) && MatchCode((s.Resource as Condition).Code, code, system)
                                                   && (s.Resource as Condition).ClinicalStatus != null && (s.Resource as Condition).ClinicalStatus.Coding != null && (s.Resource as Condition).ClinicalStatus.Coding.Any(c => c.Code.ToLower() == "active"))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Has Condition
        /// Currently supports only patient, Observation and Condition
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public ConditionResult GetConditionRes(string code, string system)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (system == null) throw new ArgumentNullException(nameof(system));
            var mobs = _metaObservations.FirstOrDefault((e => e.Code.ToLower() == code.ToLower() && e.CodeNameSpace.ToLower() == system.ToLower()));
            if (mobs != null)
            {
                return new ConditionResult()
                    {Value = mobs.Value, Code = code, CodeNameSpace = system, Description = mobs.Description+" Value obtained from PRIME aggregation"};
            }

            foreach (var s in _bundle.Entry)
            {
                if ((s.Resource is Patient) && system == "Patient")
                {
                    var r = (bool?)GetPatientProperty(s.Resource as Patient, code);
                    if (r.HasValue)
                    {

                        return new ConditionResult()
                            { Value = r.Value ? 1 : 0, Code = code, CodeNameSpace = system, Description = "Value obtained from " + system };
                     
                    }
                }

                else if ((s.Resource is Observation) && (s.Resource as Observation).Code != null && MatchCode((s.Resource as Observation).Code, code, system))
                {
                    var obs = s.Resource as Observation;
                    if (obs.Value == null)
                        continue;

                    if (obs.Status.HasValue && (obs.Status == ObservationStatus.Cancelled || obs.Status == ObservationStatus.EnteredInError || obs.Status == ObservationStatus.Amended))
                    {
                        continue;

                    }

                    long timestamp = 0;
                    if (obs.Issued.HasValue)
                    {
                        timestamp = obs.Issued.Value.ToUnixTimeSeconds();
                    }
                    var e = (Hl7.Fhir.Model.Quantity)obs.Value;

                    if (!e.Value.HasValue)
                        continue;
                    var r = decimal.ToDouble(e.Value.Value);
                    return new ConditionResult()
                        { Value = r, Code = code, CodeNameSpace = system};
                  
                }
                else if ((s.Resource is Condition) && MatchCode((s.Resource as Condition).Code, code, system)
                                                   && (s.Resource as Condition).ClinicalStatus != null && (s.Resource as Condition).ClinicalStatus.Coding != null && (s.Resource as Condition).ClinicalStatus.Coding.Any(c => c.Code.ToLower() == "active"))
                {
                    return new ConditionResult()
                        { Value = 1, Code = code, CodeNameSpace = system,Description ="Value obtained from "+system};
                }
            }

            return new ConditionResult()
            {
                CodeNameSpace=system,
                Code=code,
                Description = "The variable should be manually edited"
            };
        }



        /// <summary>
        /// Has Condition
        /// Currently supports only patient, Observation and Condition
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public double? GetCondition(string code, string system)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (system == null) throw new ArgumentNullException(nameof(system));
            var mobs = _metaObservations.FirstOrDefault((e => e.Code.ToLower() == code.ToLower() && e.CodeNameSpace.ToLower() == system.ToLower()));
            if (mobs != null)
            {
                return mobs.Value;
            }

            foreach (var s in _bundle.Entry)
            {
                if ((s.Resource is Patient) && system == "Patient")
                {
                    var r = (bool?)GetPatientProperty(s.Resource as Patient, code);
                    if (r.HasValue)
                    {
                        return r.Value ? 1 : 0;
                    }
                }

                else if ((s.Resource is Observation) && (s.Resource as Observation).Code != null && MatchCode((s.Resource as Observation).Code, code, system))
                {
                    var obs = s.Resource as Observation;
                    if (obs.Value == null)
                        continue;

                    if (obs.Status.HasValue && (obs.Status == ObservationStatus.Cancelled || obs.Status == ObservationStatus.EnteredInError || obs.Status == ObservationStatus.Amended))
                    {
                        continue;

                    }

                    long timestamp = 0;
                    if (obs.Issued.HasValue)
                    {
                        timestamp = obs.Issued.Value.ToUnixTimeSeconds();
                    }
                    var e = (Hl7.Fhir.Model.Quantity)obs.Value;

                    if (!e.Value.HasValue)
                        continue;
                    var r = decimal.ToDouble(e.Value.Value);

                    return r;
                }
                else if ((s.Resource is Condition) && MatchCode((s.Resource as Condition).Code, code, system)
                                                   && (s.Resource as Condition).ClinicalStatus != null && (s.Resource as Condition).ClinicalStatus.Coding != null && (s.Resource as Condition).ClinicalStatus.Coding.Any(c => c.Code.ToLower() == "active"))
                {
                    return 1;
                }
          }

            return new double?();
        }


     

        private class TmpCondition:IObservation
        {
            public double Value { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string CodeNameSpace { get; set; }
            public long Timestamp { get; set; }
            public string Description { get; set; }
        }
        public void AddCondition(string oCode, string codeNamespace,double value=1.0)
        {

            if (_metaObservations.Any(e => Match(e, oCode, codeNamespace)))
            {                
                _metaObservations.FirstOrDefault(e => Match(e, oCode, codeNamespace)).Value = value;
                
            }
            else
            {
                _metaObservations.Add(new TmpCondition() { Code = oCode, Value = value });
            }
        }

        private bool Match(IObservation obs, string code, string codenamespace)
        {

            if (obs.Code == null)
                return false;
            if (code == null)
                return false;
            if(codenamespace==null)
                return code.ToLower()==obs.Code.ToLower();

            if (obs.CodeNameSpace == null)
                return code.ToLower() == obs.Code.ToLower();
            ;

            return code.ToLower() == obs.Code.ToLower() && codenamespace.ToLower() == obs.CodeNameSpace.ToLower();

        }


        public void RemoveCondition(string oCode, string codeNamespace)
        {
            var c = _metaObservations.FirstOrDefault(e => e.Code == oCode);
            if (c != null)
                _metaObservations.Remove(c);
        }

        public List<IObservation> GetMetaObservations()
        {

            return _metaObservations;
        }

        /// <summary>
        /// Has Condition
        /// Currently supports only patient, Observation and Condition
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public List<PDObservation> GetObservations()
        {

            List<PDObservation> observations = new List<PDObservation>();
            if (_bundle == null)
                return observations;

            foreach (var s in _bundle.Entry)
            {
                if ((s.Resource is Patient))
                {
                    observations.AddRange(GetPatientPropertiesAsObs(s.Resource as Patient));

                }

                else if ((s.Resource is Observation) && (s.Resource as Observation).Code != null)
                {
                    var obs = s.Resource as Observation;
                    if (obs.Value == null)
                        continue;

                    if (obs.Status.HasValue && (obs.Status == ObservationStatus.Cancelled || obs.Status == ObservationStatus.EnteredInError || obs.Status == ObservationStatus.Amended))
                    {
                        continue;

                    }

                    long timestamp = 0;
                    if (obs.Issued.HasValue)
                    {
                        timestamp = obs.Issued.Value.ToUnixTimeSeconds();
                    }
                    var e = (Hl7.Fhir.Model.Quantity)obs.Value;

                    if (!e.Value.HasValue)
                        continue;
                    var r = decimal.ToDouble(e.Value.Value);


                    observations.Add(new PDObservation()
                    {
                        Code = (s.Resource as Observation).Code.Coding.FirstOrDefault()?.Code,
                        Weight = 1,
                        Value = r,
                        CodeNameSpace = (s.Resource as Observation).Code.Coding.FirstOrDefault()?.System,


                    });
                }
                else if ((s.Resource is Condition) && (s.Resource as Condition).ClinicalStatus != null && (s.Resource as Condition).ClinicalStatus.Coding != null && (s.Resource as Condition).ClinicalStatus.Coding.Any(c => c.Code.ToLower() == "active"))
                {

                    observations.Add(new PDObservation()
                    {
                        Code = (s.Resource as Condition).Code.Coding.FirstOrDefault()?.Code,
                        Weight = 1,
                        Value = 1,
                        CodeNameSpace = (s.Resource as Condition).Code.Coding.FirstOrDefault()?.System,

                    });
                }
            }

            return observations;
        }


        private class ObsInfo
        {
            public bool Value { get; set; }
            public long Timestamp { get; set; }
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


            var mobs = _metaObservations.FirstOrDefault((e => e.Code.ToLower() == code && e.CodeNameSpace == system));
            if (mobs != null)
            {
                return mobs.Value > 0;
            }

            foreach (var s in _bundle.Entry)
            {
                if (s.Resource == null)
                    continue;

                if ((s.Resource is Patient) && system == "Patient")
                {
                    return convert.Invoke(GetPatientProperty(s.Resource as Patient, code));
                }

                else if ((s.Resource is Observation) && MatchCode((s.Resource as Observation).Code, code, system))
                {
                    var r = false;
                    var obs = s.Resource as Observation;
                    if (obs.Value == null)
                        continue;

                    if (obs.Status.HasValue && (obs.Status == ObservationStatus.Cancelled || obs.Status == ObservationStatus.EnteredInError || obs.Status == ObservationStatus.Amended))
                    {
                        continue;

                    }

                    long timestamp = 0;
                    if (obs.Issued.HasValue)
                    {
                        timestamp = obs.Issued.Value.ToUnixTimeSeconds();
                    }
                    var e = (Hl7.Fhir.Model.Quantity)obs.Value;

                    if (!e.Value.HasValue)
                        continue;
                    r = convert.Invoke(e.Value.Value);

                    observations.Add(new ObsInfo()
                    {
                        Value = r,
                        Timestamp = timestamp
                    });



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

        public async Task Aggregate(string patientId, IAggregator aggregator, List<AggrModel> aggregators)
        {
            var obs = GetObservations();

            foreach (var aggr in aggregators)
            {
                var r = await aggregator.RunSingle(patientId, aggr.Code, "PRIME", obs, aggr.Config);

                if (_metaObservations.Any(e => e.CodeNameSpace == r.CodeNameSpace && e.Code == r.Code))
                {
                    _metaObservations.FirstOrDefault(e => e.CodeNameSpace == r.CodeNameSpace && e.Code == r.Code)
                        .Value = r.Value;
                }
                else{
                    _metaObservations.Add(r);
                }

            }


        }
    }
}