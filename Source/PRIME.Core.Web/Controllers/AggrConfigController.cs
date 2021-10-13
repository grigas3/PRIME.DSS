using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PRIME.Core.Aggregators;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.Web.Extensions;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// DSS Controller
    /// </summary>
    [Route("api/v1/[controller]")]
    public class AggrConfigController : Controller
    {
        #region Controllers

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="logger">Logger</param>
        public AggrConfigController(IRepositoryService context, ILogger<DSSController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        #region Private Declarations

        private readonly IRepositoryService _context;
        private readonly ILogger _logger;

        #endregion

        /// <summary>
        ///     Get DSS Config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var model = await _context.FindAsync<AggrModel>(int.Parse(id));

            if (model == null)
                return NotFound("Aggregation Model not found");
            
            var ret = model.GetConfig();
            return Ok(ret);


        }
        


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody]AggrConfig model)
        {
            try
            {
                var dss = await _context.FindAsync<AggrModel>(int.Parse(model.Id));
                dss.Config = JsonConvert.SerializeObject(model);
                await _context.InserOrUpdateAsync(dss);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Post DSS Model failed");
                return BadRequest("Post DSS Model failed");
            }


            return Ok(model);
        }



    }
}