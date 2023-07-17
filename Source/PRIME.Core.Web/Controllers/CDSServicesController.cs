using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Web.Controllers
{

    /// <summary>
    /// CDS Hooks Service
    /// </summary>
    [Route("api/v1/[controller]")]
    public class CDSServicesController : Controller
    {
        private readonly ICDSService _cDsService;
        private readonly IAggregator _aggregator;
        private readonly IRepositoryService _repositoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repoService"></param>
        /// <param name="service"></param>
        /// <param name="aggregator"></param>
        public CDSServicesController(IRepositoryService repoService, 
            ICDSService service
            ,IAggregator aggregator)
        {
            _repositoryService = repoService;
            _cDsService = service;
            _aggregator = aggregator;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(string code=null)
        {

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("You should include ?code=[PRIME CDS Client code]");
            }

            var client = (await _repositoryService.GetAsync<CDSClient>(e => e.Code == code)).FirstOrDefault();

            if (client == null)
            {
                return BadRequest("You should include ?code=[PRIME CDS Client code]");
            }


            var response = new List<DiscoverResponse>();

            var models = (await _repositoryService.GetAsync<DSSModel>(e => e.CDSClientId == client.Id));

            foreach (var model in models)
            {

                response.Add(new DiscoverResponse()
                {
                    Id = model.Id.ToString(),
                    Description = model.Description,
                    Hook = "patient-view",
                    Title = model.Name,
                });

            }


            return Ok(response);



        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CDSRequest request)
        {
            if (request == null)
            {
                return BadRequest("You should provide a proper request");
            }
            //if (string.IsNullOrEmpty(request.Id))
            //{
            //    return BadRequest("You should provide a proper id");
            //}

            if (request.Context == null)
            {
                return BadRequest("You should provide a proper request context");
            }
            if (request.FhirAuthorization == null)
            {
                return BadRequest("You should provide a proper authentication context");
            }

            if (string.IsNullOrEmpty(request.Context.PatientId))
            {
                return BadRequest("You should provide a proper request context patientId");
            }
            if (string.IsNullOrEmpty(request.Context.ClientCode)&&!request.Context.ClientId.HasValue)
            {
                return BadRequest("You should provide a proper request context Client Code or ClientId");
            }
            if (string.IsNullOrEmpty(request.FhirServer))
            {
                return BadRequest("You should provide a proper FHIR Server");
            }
            //Get Data from FHIR

            FhirConditionRepository repository = new FhirConditionRepository(new FhirProxyConfiguration()
            {
                Url = request.FhirServer,
                Token = request.FhirAuthorization.Access_Token,
                Headers = new List<FhirHeader>(),
                RequiresAuthentication = true,

            });

            try
            {
                //Evaluate DSS
                var r=await CDSSMainEvaluator.EvaluateAsync(
                    _cDsService,
                    _aggregator,
                    _repositoryService,
                    repository,
                    request.Context.PatientId,
                    request.Context.ClientId, 
                    request.Context.ClientCode,
                    request.EvaluateDSS);

                return Ok(r.Item1);

            }
            catch (Exception ex)
            {
                return BadRequest((ex.Message));
            }



        }
    }
}