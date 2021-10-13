using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRIME.Core.Common.Models;
using PRIME.Core.Context.Entities;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// FHIR based Patient Controller
    /// </summary>
    [Route("api/v1/[controller]")]
    public class PatientController:Controller
    {


        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FHIRRequest request)
        {
            if (request == null)
            {
                return BadRequest("You should provide a proper request");
            }
            //if (string.IsNullOrEmpty(request.Id))
            //{
            //    return BadRequest("You should provide a proper id");
            //}

          
            if (request.FhirAuthorization == null)
            {
                return BadRequest("You should provide a proper authentication context");
            }

            
            if (string.IsNullOrEmpty(request.FhirServer))
            {
                return BadRequest("You should provide a proper FHIR Server");
            }

            CDSClient client = null;

            //Get Data from FHIR

            
            FhirPatientQuery query = new FhirPatientQuery(new FhirProxyConfiguration()
            {
                Url = request.FhirServer,
                Token = request.FhirAuthorization.Access_Token,
                Headers = new List<FhirHeader>(),
                RequiresAuthentication = true,

            });

            var res = await query.Get();

            return Ok(res.Select(e => new PatientInfo()
            {
                FamilyName = e.Name.FirstOrDefault()?.Family,
                GivenName = e.Name.FirstOrDefault()?.Given.FirstOrDefault(), Id = e.Id
            }));

        }


    }
}
