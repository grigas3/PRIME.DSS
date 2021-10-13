using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS.Treatment;
using PRIME.Core.Web.Extensions;

namespace PRIME.Core.Web.Controllers
{

    public class PrefetchResponse{


        public string Patient
        {
            get { return "Patient/{{context.patientId}}"; }
        }

    }

    public class DiscoverResponse
    {


        public string Hook { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        public PrefetchResponse Prefetch
        {
            get { return new PrefetchResponse(); }
        }


    }

    public class FHIRAuthorization
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }
        public string Scope { get; set; }
        public string Subject { get; set; }

    }

    public class CDSRequestContext
    {

        public int? ClientId { get; set; }
        public string UserId { get; set; }
        public string PatientId { get; set; }
        public string ClientCode { get; set; }

    }

    public class FHIRRequest
    {
        public string Hook { get; set; }
        public string HookInstance { get; set; }
        public string FhirServer { get; set; }
        public FHIRAuthorization FhirAuthorization { get; set; }
        public string Prefetch { get; set; }
        

    }

    
    public class CDSRequest : FHIRRequest
    {
        public string Id { get; set; }
        public CDSRequestContext Context { get; set; }
        public bool EvaluateDSS { get; set; }
    }


    public abstract class BaseCDSController:Controller
    {
        public class CDSInput
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("input")] public string Input { get; set; }
        }

        public class ConditionRepository : IConditionRepository
        {
            private readonly List<Condition> _codes = new List<Condition>();



            public void Add(Condition c)
            {
                _codes.Add(c);
            }

            public async Task Init(string id)
            {

            }

            public Task Aggregate(string patientId, IAggregator aggregator, List<AggrModel> aggregators)
            {
                throw new NotImplementedException();
            }

            public bool? HasCondition(string code, string system)
            {
                var v= _codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();

                if (v.HasValue)
                    return v.Value > 0;
                return new bool?();
            }

            public bool? HasCondition(string code, string system, Func<object, bool> convert)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Get Condition Value
            /// </summary>
            /// <param name="code"></param>
            /// <param name="system"></param>
            /// <returns></returns>
            public double? GetCondition(string code, string system)
            {
                var v=_codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();

                if (v.HasValue)
                {
                    return v.Value;
                }

                return new double();

            }

            public void AddCondition(string oCode, string codeNamespace,double value=1.0)
            {

                if (_codes.Any(e => Match(e, oCode)))
                {
                    _codes.FirstOrDefault(e => Match(e, oCode)).Value = value;

                }
                else
                {
                    _codes.Add(new Condition() { Code = oCode, Value = value });
                }


            }
            private bool Match(Condition obs, string code)
            {

                if (obs.Code == null)
                    return false;
                if (code == null)
                    return false;
                    return code.ToLower() == obs.Code.ToLower();

                
            }

            public void RemoveCondition(string oCode, string codeNamespace)
            {
                if (string.IsNullOrEmpty(codeNamespace))
                    codeNamespace = "PRIME";
                var c= _codes.FirstOrDefault(e=>e.Code.ToLower() == oCode.ToLower());
                if(c!=null)
                _codes.Remove(c);
            }

            public ConditionResult GetConditionRes(string code, string system)
            {
                var v = _codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();

                if (v.HasValue)
                {
                    return new ConditionResult()
                    {
                        Value=v.Value,
                        Code=code,
                        CodeNameSpace =system
                    };
                }

                return new ConditionResult()
                {
                    
                    Code = code,
                    CodeNameSpace = system
                };
            }
        }

        public class Condition
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public double? Value { get; set; }
            public string Category { get; set; }
        }
    }

    [Route("api/v1/[controller]")]
    public class CDSController : BaseCDSController
    {
        private readonly ICDSService _cDsService;

        private readonly List<Condition> _conditions = new List<Condition>
        {
            new Condition
            {
                Name = "Levodopa",
                Code = "LEVODOPA",
                Category = "MED"
            },

            new Condition
            {
                Name = "MAO",
                Code = "MAO",
                Category = "MED"
            },
            new Condition
            {
                Name = "COMT",
                Code = "COMT",
                Category = "MED"
            },
            new Condition
            {
                Name = "Dopamine Agonists",
                Code = "DOPAMINE",
                Category = "MED"
            },
            new Condition
            {
                Name = "Motor symptoms",
                Code = "MOTOR",
                Category = "PD"
            },
            new Condition
            {
                Name = "Fluctuations",
                Code = "FLUCTUATIONS",
                Category = "PD"
            },
            new Condition
            {
                Name = "Dyskinesia",
                Code = "DYSKINESIA",
                Category = "PD"
            },
            new Condition
            {
                Name = "History of Impulsivity",
                Code = "IMPHISTORY",
                Category = "PD"
            },

            new Condition
            {
                Name = "Alcohol",
                Code = "ALCOHOL",
                Category = "PD"
            },
            new Condition
            {
                Name = "Daytime Sleepiness",
                Code = "SLEEPINESS",
                Category = "PD"
            }
        };

        private readonly IRepositoryService _repositoryService;


        public CDSController(IRepositoryService repoService,ICDSService service)
        {
            _repositoryService = repoService;
            _cDsService = service;
        }

      
        /// <summary>
        ///     Post a form with DSS Model Values
        /// </summary>
        /// <param name="dssInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CDSInput model)
        {
            var repository = new ConditionRepository();
            if (model != null && !string.IsNullOrEmpty(model.Input))
            {
                //Deserialize inputs
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.Input);

                var cdsModel = await _repositoryService.FindAsync<DSSModel>(int.Parse(model.Id));
                var classifier = TreatmentClassifier.FromJson(cdsModel.Config);
                var vars = classifier.Variables;
                var variables = classifier.Variables.Select(e => new Condition()
                {
                    Code = e.Code,
                    Name = e.Name,
                    Category = cdsModel.Name
                });
                foreach (var c in vars)
                {
                    if (!values.ContainsKey(c.Code))
                        continue;


                    var v = values[c.Code];

                    if (string.IsNullOrEmpty(v))
                        repository.Add(new Condition {Code = c.Code, Value = new double?()});
                    else if (v.ToLower() == "true")
                        repository.Add(new Condition {Code = c.Code, Value = 1});
                    else if (v.ToLower() == "false")
                        repository.Add(new Condition {Code = c.Code, Value = 0});
                    else
                        repository.Add(new Condition {Code = c.Code, Value = new double?()});
                }

                
                var cards = await _cDsService.GetCardsAsync(repository, cdsModel);

                //Run DSS
                return Ok(cards);
            }

            return Ok(new List<Card>());
        }

        //[HttpGet("conditions/{category}")]
        //public IActionResult Get(string category)
        //{
        //    if (string.IsNullOrEmpty(category))
        //        return Ok(_conditions);
        //    return Ok(_conditions.Where(e => e.Category.ToLower() == category.ToLower()));
        //}


    


    }
}