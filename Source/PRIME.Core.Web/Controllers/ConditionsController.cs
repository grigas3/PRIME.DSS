using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS.Treatment;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// Conditions of a CDSS model
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ConditionController : BaseCDSController
    {
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


        public ConditionController(IRepositoryService repoService)
        {
            _repositoryService = repoService;
            
        }
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(_conditions);

            }
            else
            {
                var model = await _repositoryService.FindAsync<DSSModel>(int.Parse(id));

                var classifier = TreatmentClassifier.FromJson(model.Config);

                var variables = classifier.Variables.Select(e => new Condition()
                {
                    Code = e.Code,
                    Name = e.Name,
                    Category = model.Name
                });

                return Ok(variables);
            }


        }

    



       

    }
}