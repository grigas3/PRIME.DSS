using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Web.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRIME.Core.Context.Entities;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// DSS Value
    /// This Controller expose two methods
    /// one get method to run DSS evaluation for a specific patient
    /// and one post method to get evaluation based on DSSInput values from a form
    /// </summary>
    [Route("api/v1/[controller]")]
    public class DSSEvalController : Controller
    {
        #region Private  declaration
        private readonly IAggregator _aggregator;
        
        private readonly ICDSService _dssRunner;
        private readonly IRepositoryService _context;
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DSS Context</param>
        /// <param name="dSSRunner">DSS Runner</param>
        public DSSEvalController(IRepositoryService context,
            ICDSService dSSRunner, 
            IAggregator aggregator)
        {
            _dssRunner = dSSRunner;
            _context = context;
            _aggregator = aggregator;

        }

     
        /// <summary>
        /// Post a form with DSS Model Values
        /// </summary>
        /// <param name="dssInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]DSSInput model)
        {

            int clientId = 0;
            if (!string.IsNullOrEmpty((model.Id)))
            {
                var amodel = await _context.FindAsync<DSSModel>(int.Parse(model.Id));

                if (amodel == null || !amodel.CDSClientId.HasValue)
                    return BadRequest();

                clientId = amodel.CDSClientId.Value;
            }
            else if (!string.IsNullOrEmpty(model.ClientId))
            {

                clientId = int.Parse(model.ClientId);
            }
            else
            {
                return BadRequest("Invalid arguments");
            }


            IConditionRepository repository = null;

            if (!string.IsNullOrEmpty(model.BundleJson))
            {
                var aggrModels = (await _context.GetAsync<AggrModel>(e => e.CDSClientId == clientId)).ToList();
                repository = new LocalFhirConditionRepository(model.BundleJson);
                await repository.Init(model.PatientId);
                await repository.Aggregate(model.PatientId, _aggregator, aggrModels);

            }
            else
            {
                repository=new BaseCDSController.ConditionRepository();

                if (model.Variables != null)
                {
                    foreach (var c in model.Variables)
                    {
                        var codeNamespace = c.CodeNameSpace;
                        if (string.IsNullOrEmpty(codeNamespace))
                            codeNamespace = "PRIME";

                        repository.AddCondition(c.Code, codeNamespace, c.Value);

                    }
                }
            }
            var cards=await _dssRunner.GetCardsAsync(repository, clientId);

            return Ok(cards);


        }

    }
}