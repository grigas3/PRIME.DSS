using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class CDSController : Controller
    {
        private readonly ICDSService _cDSService;

        private readonly List<Condition> conditions = new List<Condition>
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


        public CDSController(ICDSService service)
        {
            _cDSService = service;
        }

        /// <summary>
        ///     Post a form with DSS Model Values
        /// </summary>
        /// <param name="dssInput"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] CDSInput model)
        {
            var repository = new ConditionRepository();
            if (model != null && !string.IsNullOrEmpty(model.Input))
            {
                //Deserialize inputs
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.Input);

                foreach (var c in values.Keys)
                {
                    var v = values[c];

                    if (string.IsNullOrEmpty(v))
                        repository.Add(new Condition {Code = c.ToLower(), Value = new bool?()});
                    else if (v.ToLower() == "true")
                        repository.Add(new Condition {Code = c.ToLower(), Value = true});
                    else if (v.ToLower() == "false")
                        repository.Add(new Condition {Code = c.ToLower(), Value = false});
                    else
                        repository.Add(new Condition {Code = c.ToLower(), Value = new bool?()});
                }
            }

            foreach (var c in conditions) repository.Add(c);

            //Run DSS
            return Ok(_cDSService.GetCards(repository));
        }

        [HttpGet("conditions/{category}")]
        public IActionResult Get(string category)
        {
            if (string.IsNullOrEmpty(category))
                return Ok(conditions);
            return Ok(conditions.Where(e => e.Category.ToLower() == category.ToLower()));
        }


        private class ConditionRepository : IConditionRepository
        {
            private readonly List<Condition> _codes = new List<Condition>();

           

            public void Add(Condition c)
            {
                _codes.Add(c);
            }

            public async Task Init(string id)
            {
                
            }

            public bool? HasCondition(string code, string system)
            {
                return _codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();
            }

            public bool? HasCondition(string code, string system, Func<object, bool> convert)
            {
                throw new NotImplementedException();
            }
        }


        public class Condition
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public bool? Value { get; set; }
            public string Category { get; set; }
        }

        public class CDSInput
        {
            [JsonProperty("input")] public string Input { get; set; }
        }
    }
}