﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Web.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// DSS Value
    /// This Controller expose two methods
    /// one get method to run DSS evaluation for a specific patient
    /// and one post method to get evaluation based on DSSInput values from a form
    /// </summary>
    [Route("api/v1/[controller]")]
    public class DSSEvalController : Controller
    {
        #region Private  declaration
        private readonly IDSSRunner _dssRunner;
        private readonly Context.DSSContext _context;
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DSS Context</param>
        /// <param name="dSSRunner">DSS Runner</param>
        public DSSEvalController(Context.DSSContext context,IDSSRunner dSSRunner)
        {
            _dssRunner = dSSRunner;
            _context = context;
        }

        /// <summary>
        /// Get DSS Evaluation. Based on the DSS definition it will use the last N day observations
        /// to get a DSS output.
        /// Sample call
        /// GET api/dss/execute/5?patientId=2
        /// </summary>
        /// <param name="id">DSS model by id</param>
        /// <param name="code">DSS model by code If ID is provided it has priority againsty code</param>
        /// <param name="patientId">Patient id</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Get(string id,string code, string patientId)
        {
            DSSModel item = null;

            if (!string.IsNullOrEmpty(id))
            {
                int key = int.Parse(id);
                item = _context.Find<DSSModel>(key);
                if (item == null)
                    return NotFound("DSS Model not found");
            }
            else if(!string.IsNullOrEmpty(code))
            {
                int key = int.Parse(id);
                item = _context.Set<DSSModel>().FirstOrDefault(e => e.Code == code);
                if (item == null)
                    return NotFound("DSS Model not found");

            }
            else
            {
                return NotFound("DSS Model not found");
            }

            //Run DSS
            var res = await _dssRunner.Run(patientId, item.Config);
            //_dssRunner.Run(id)
            return Ok(res);
        }


        /// <summary>
        /// Get DSS Config
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>

        [HttpGet("input/{id}")]
        public async Task<IActionResult> Input(int id, string patientId)
        {
            var item = _context.Find<DSSModel>(id);
            if (item == null)
                return NotFound("DSS Model not found");

            //Run DSS
            var res = await _dssRunner.GetInputValues(patientId, item.Config);
            //_dssRunner.Run(id)
            return Ok(res);




        }
        /// <summary>
        /// Post a form with DSS Model Values
        /// </summary>
        /// <param name="dssInput"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]DSSInput dssInput)
        {
          

            if (dssInput!=null&&!string.IsNullOrEmpty(dssInput.Input))
            {
                //Deserialize inputs
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(dssInput.Input);
             
                //Find DSS Model
                var item = _context.Find<DSSModel>(int.Parse(dssInput.ModelId));
                if (item == null)
                    return NotFound("DSS Model not found");

                //Run DSS 
                var ret = _dssRunner.Run(item.Config, values);
                return Ok(ret);

            }
            else
            {

                return BadRequest();
            }

            



        }




    }
}