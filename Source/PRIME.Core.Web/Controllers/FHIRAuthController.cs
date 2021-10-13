using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class FHIRAuthController : Controller
    {
        #region Controllers
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>        
        /// <param name="logger">Logger</param>
        public FHIRAuthController()
        {
            

        }
        #endregion

        public class AuthReq
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("fhirAuthUrl")]
            public string FhirAuthUrl { get; set; }
            [JsonProperty("code")]
            public string Code { get; set; }

        }

        /// <summary>
        /// Get FHIR metadata
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AuthReq req)
        {
            if(req==null)
                return BadRequest("Request is empty");
            if (string.IsNullOrEmpty(req.FhirAuthUrl))
                return BadRequest("URL is empty");

            if (string.IsNullOrEmpty(req.Code))
                return BadRequest("Code is empty");

            var res = await PrimeFhirClient.AuthenticateWithCode(req.FhirAuthUrl,req.Code);

            return Ok(res);

        }

    }
}