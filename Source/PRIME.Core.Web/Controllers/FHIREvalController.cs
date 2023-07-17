using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class FHIREvalController : Controller
    {
        #region Private Declarations     
        private readonly IAggregator _aggregator;
        private readonly IRepositoryService _context;
        private readonly ILogger _logger;
        #endregion

        #region Controllers
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="aggregator"></param>        
        /// <param name="logger">Logger</param>
        public FHIREvalController(IRepositoryService context, IAggregator aggregator, ILogger<AggregationController> logger)
        {
            _context = context;
            _logger = logger;
            _aggregator = aggregator;
        }
        #endregion

        /// <summary>
        /// Evaluate
        /// </summary>
        /// <param name="code"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AggregationController.FHIREvaluation model)
        {
            var amodel = await _context.FindAsync<AggrModel>(int.Parse(model.Id));



            LocalFhirConditionRepository repository = new LocalFhirConditionRepository(model.BundleJson);
            await repository.Init(model.PatientId);
            await repository.Aggregate(model.PatientId, _aggregator, new List<AggrModel>()
            {
                amodel

            });

            return Ok(repository.GetMetaObservations());

        }


    }
}