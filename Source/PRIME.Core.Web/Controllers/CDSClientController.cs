using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class CDSClientController:Controller
    {
        #region Private Declarations        
        private readonly IRepositoryService _context;
        private readonly ILogger _logger;

        #endregion
        #region Controllers
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>        
        /// <param name="logger">Logger</param>
        public CDSClientController(IRepositoryService context, ILogger<DSSController> logger)
        {
            _context = context;
            _logger = logger;

        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Get(int id=0,bool includeModels=false)
        {
            if (id == 0)
            {
                var list=await _context.GetAsync<CDSClient>();
                return Ok(list);

            }
            else
            {
                var item = await _context.FindAsync<CDSClient>(id);

                if (includeModels)
                {
                    var models=await _context.GetAsync<DSSModel>(e=>e.CDSClientId==id);
                 
                }
                //Enumerate in order to force the Models to include them
                var list=item.DSSModels.ToList();
                return Ok(item);
            }

        }
       
       
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CDSClient client)
        {
         
                await _context.InserOrUpdateAsync<CDSClient>(client);
                return Ok();
        }



        [HttpPut]
        public async Task<IActionResult> Put([FromBody]CDSClient client)
        {

            await _context.InserOrUpdateAsync<CDSClient>(client);
            return Ok();
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            await _context.DeleteAsync<CDSClient>(id);
            return Ok();
        }
    }
}
