using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIME.Core.Web.Controllers
{


    /// <summary>
    /// Medication Check Controller
    /// This Controller expose two methods
    /// one get method to run DSS evaluation for a specific patient
    /// and one post method to get evaluation based on DSSInput values from a form
    /// </summary>
    [Route("api/v1/[controller]")]
    public class StitchController : Controller
    {
        #region Private  declaration
        private readonly IProteinCheckService _proteinCheckService;

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Protein check service</param>
        
        public StitchController(IProteinCheckService service)
        {
            _proteinCheckService = service;

        }

        /// <summary>
        /// Get Medication Evaluation. Based on the Me definition it will use the last N day observations
        /// to get a DSS output.
        /// Sample call
        /// GET api/dss/execute/5?patientId=2
        /// </summary>
        /// <param name="id">DSS model by id</param>
        /// <param name="code">DSS model by code If ID is provided it has priority againsty code</param>
        /// <param name="patientId">Patient id</param>
        /// <returns></returns>

        [HttpGet("match")]
        public async Task<IActionResult> Match(string term)
        {
            try
            {
                var res = await _proteinCheckService.MatchItems(term);
                
                return Ok(new {results=res});
            }
            catch (Exception ex)
            {

                return BadRequest();
            }

        }


        [HttpGet("interactions")]
        public async Task<IActionResult> Interactions(string term)
        {
            try
            {
                var res = await _proteinCheckService.Check(new List<string>(){ term });
                return Ok(new {results=res});
            }
            catch (Exception ex)
            {

                return BadRequest();
            }

        }




    }

    /// <summary>
    /// Medication Check Controller
    /// This Controller expose two methods
    /// one get method to run DSS evaluation for a specific patient
    /// and one post method to get evaluation based on DSSInput values from a form
    /// </summary>
    [Route("api/v1/[controller]")]
    public class MedCheckController : Controller
    {
        #region Private  declaration
        private readonly IMedCheckService _medCheckService;
        
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DSS Context</param>
        /// <param name="dSSRunner">DSS Runner</param>
        public MedCheckController(IMedCheckService dSSRunner)
        {
            _medCheckService = dSSRunner;
            
        }

        /// <summary>
        /// Get Medication Evaluation. Based on the Me definition it will use the last N day observations
        /// to get a DSS output.
        /// Sample call
        /// GET api/dss/execute/5?patientId=2
        /// </summary>
        /// <param name="id">DSS model by id</param>
        /// <param name="code">DSS model by code If ID is provided it has priority againsty code</param>
        /// <param name="patientId">Patient id</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Get(string genes,string drugs)
        {
            try
            {
                var res =await _medCheckService.Check(genes, drugs);
                return Ok(res);
            }
            catch(Exception ex)
            {

                return BadRequest();
            }

        }


   



    }
}