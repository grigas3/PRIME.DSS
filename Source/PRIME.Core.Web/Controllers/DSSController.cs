using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema.Generation;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS;
using PRIME.Core.DSS.Fuzzy;
using PRIME.Core.Web.Context;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    ///     DSS Controller
    /// </summary>
    [Route("api/v1/[controller]")]
    public class DSSController : Controller
    {
        #region Controllers

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="logger">Logger</param>
        public DSSController(IRepositoryService context, ILogger<DSSController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        #region Private Declarations

        private readonly IRepositoryService _context;
        private readonly ILogger _logger;

        #endregion

        #region

        /// <summary>
        ///     Get dss model
        ///     If
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of dss models</returns>
        [HttpGet]
        [Produces("application/json", "application/xml", Type = typeof(DSSModel))]
        public async Task<IActionResult> Get(int id = 0)
        {
            if (id == 0)
            {
                var list = (await _context.GetAsync<DSSModel>("CDSClient")).ToList();
                return Ok(list);
            }

            var item = await _context.FindAsync<DSSModel>(id);

            if (item == null)
                return NotFound("DSS Model not found");
            //Return item
            return Ok(item);
        }

      


        /// <summary>
        ///     Get DSS Definition Schema
        /// </summary>
        /// <returns></returns>
        [HttpGet("schema")]
        public IActionResult GetSchema()
        {
            var generator = new JSchemaGenerator();

            var schema = generator.Generate(typeof(DSSConfig));

            return Json(schema.ToString());
        }

        /// <summary>
        ///     Post DSS Model
        ///     Call: POST api/dss/5
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody]DSSModel model)
        {
            try
            {
                await _context.InserOrUpdateAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Post DSS Model failed");
                return BadRequest("Post DSS Model failed");
            }


            return Ok(model);
        }

        #endregion


        #region Testing Methods

        /// <summary>
        ///     Create Dummy Data
        /// </summary>
        /// <returns></returns>
        // GET api/dss/dummy
        [HttpGet("dummy")]
        public IActionResult CreateDummy()
        {
        //    AddDummyDSS(_context);

            return Ok();
        }


        private void AddDummyDSS(DSSContext context)
        {
            var config = string.Empty;
            try
            {
                config = System.IO.File.ReadAllText("modelyesno2.json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Config file");
            }

            try
            {
                context.Set<DSSModel>().Add(new DSSModel
                {
                    Name = "YesNo",
                    Code = "MedicationChange",
                    Description = "YesNo Model",
                    Config = config,
                    CreatedBy = "admin",
                    ModifiedBy = "admin",
                    ModifiedDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    Id = 1
                });

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add Test Data");
            }
        }

        #endregion


        #region CRUD



        [HttpPut]
        public async Task<IActionResult> Put([FromBody]DSSModel client)
        {

            await _context.InserOrUpdateAsync<DSSModel>(client);
            return Ok();
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            await _context.DeleteAsync<DSSModel>(id);
            return Ok();
        }

        #endregion

    }
}