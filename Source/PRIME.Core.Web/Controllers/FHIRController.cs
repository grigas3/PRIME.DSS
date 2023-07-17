using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class FHIRController:Controller
    {
        #region Private Declarations        
        private readonly IRepositoryService _context;
        private readonly ILogger _logger;

        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>        
        /// <param name="logger">Logger</param>
        public FHIRController(IRepositoryService context, ILogger<DSSController> logger)
        {
            _context = context;
            _logger = logger;

        }
        #endregion

        /// <summary>
        /// Get FHIR metadata
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Get(string id)
        {


            CDSClient item = null;
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Empty id");
            }

            var idN = 0;
            if (int.TryParse(id, out idN))
            {
                item = await _context.FindAsync<CDSClient>(idN);
            }
            else
            {
                item = (await _context.GetAsync<CDSClient>(e=>e.Code==id)).FirstOrDefault();
            }

            if (item == null)
                return BadRequest("CDSS Not found");
            if(string.IsNullOrEmpty(item.Url))
                return BadRequest("URL is empty");

            var res=await PrimeFhirClient.GetMetadata(item.Url);


            if(string.IsNullOrEmpty(res.AuthenticationUrl))
                res.AuthenticationUrl=item.AuthenticationUrl;

            if (string.IsNullOrEmpty(res.ResourceUrl))
                res.ResourceUrl = item.ResourceUrl;

            if (string.IsNullOrEmpty(res.AuthorizationUrl))
                res.AuthorizationUrl = item.AuthorizationUrl;

            return Ok(res);

        }

    }
}
