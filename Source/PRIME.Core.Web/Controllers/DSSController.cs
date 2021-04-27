﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS;
using PRIME.Core.Web.Entities;
using PRIME.Core.Web.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// DSS Controller
    /// </summary>
    [Route("api/v1/[controller]")]
    public class DSSController : Controller
    {
        #region Private Declarations        
        private readonly Context.DSSContext _context;
        private readonly ILogger _logger;
        
        #endregion

        #region Controllers
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>        
        /// <param name="logger">Logger</param>
        public DSSController(Context.DSSContext context,ILogger<DSSController> logger)
        {          
            _context = context;
            _logger = logger;
            
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get List of dss models
        /// </summary>
        /// <returns>List of dss models</returns>

        [HttpGet("list")]
        public IActionResult Get()
        {
            var list = _context.Set<DSSModel>().ToList();
            return Ok(list);

        }

        /// <summary>
        /// Get dss model
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of dss models</returns>
        [HttpGet]
        [Produces("application/json", "application/xml", Type = typeof(DSSModel))]
        public IActionResult Get(int id)
        {
            var item = _context.Find<DSSModel>(id);

            if (item == null)
                return NotFound("DSS Model not found");
            //Return item
            return Ok(item);

        }
     
        /// <summary>
        /// Get DSS Config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("config/{id}")]
        public IActionResult Config(string id)
        {

            //If Id is empty then we load the default model
            if (string.IsNullOrEmpty(id))
            {

                //FOR Test
                var ret = ModelExtensions.GetDSSConfig("modelyesno.json");
                return Ok(ret);
            }

            else
            {

                var model = _context.Find<DSSModel>(int.Parse(id));

                if (model == null)
                    return NotFound("DSS Model not found");

                var ret = model.GetConfig();
                return Ok(ret);

            }

        }


        /// <summary>
        /// Get DSS Definition Schema
        /// </summary>
        /// <returns></returns>
        [HttpGet("schema")]
        public IActionResult GetSchema()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(DSSConfig));

            return Json(schema.ToString());
        }

        /// <summary>
        /// Post DSS Model
        /// Call: POST api/dss/5
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(DSSModel model)
        {

            try
            {
                var newModel = await _context.AddAsync(model);
                var ret = await _context.SaveChangesAsync();

            }
            catch(Exception ex)
            {

                _logger.LogError(ex, "Post DSS Model failed");
                return BadRequest("Post DSS Model failed");
            }
            

            return Ok(model);
            
        }
        #endregion


        #region Testing Methods

        /// <summary>
        /// Create Dummy Data
        /// </summary>
        /// <returns></returns>
        // GET api/dss/dummy
        [HttpGet("dummy")]
        public IActionResult CreateDummy()
        {
           
            AddDummyDSS(_context);

            return Ok();
        }
             

        private void AddDummyDSS(Context.DSSContext context)
        {
            string config = string.Empty;
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

                context.Set<DSSModel>().Add(new DSSModel()
                {

                    Name = "YesNo",
                    Code= "MedicationChange",
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


    }
}