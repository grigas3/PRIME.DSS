using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRIME.Core.Aggregators;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.Models;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// Controller for Validating Aggregation Model
    /// </summary>
    [Route("api/v1/[controller]")]
    public class AggrValidate:Controller
    {

        private readonly IAggregator _aggregator;
        private readonly IRepositoryService _repository;
        public AggrValidate(IRepositoryService rep,IAggregator aggr)
        {
            _repository = rep;
            _aggregator = aggr;
        }

        public class ValidationResult
        {
            public int Index { get; set; }
            public string Input { get; set; }
            public double Output { get; set; }
            public double ExpectedOutput { get; set; }
            public bool Valid { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> Post(string id,IFormFile file)
        {

            List<ValidationResult> results = new List<ValidationResult>();
            var model=await this._repository.FindAsync<AggrModel>(int.Parse(id));

            if (model == null)
                return BadRequest("Model is empty");

            if (model.Config == null)
                return BadRequest("Model Config is empty");

            
            //foreach ()
            {

                var formFile = file;
                if (formFile.Length > 0)
                {
                    StreamReader str=new StreamReader(formFile.OpenReadStream());
                    
                    string line = "";
                    var header = await str.ReadLineAsync();
                    string[] codes = null;
                    if (header != null)
                    {
                        codes = header.Split(';');
                    }

                    if (codes == null)
                        return BadRequest("No Header");

                    var config = AggrConfig.FromString(model.Config);
                    int lineCount = 0;
                    while ((line = await str.ReadLineAsync()) != null)
                    {
                        var values = line.Split(new char[]{';'});
                        List<PDObservation> observations=new List<PDObservation>();
                        int count = 0;
                        double expectedValue = 0;
                        foreach (var c in codes)
                        {
                            if (c != model.Code)
                            {
                                observations.Add(new PDObservation(){ Code=c,Value=double.Parse(values[count++]),CodeNameSpace="PRIME"});
                            }

                            else
                            {
                                expectedValue = double.Parse(values[count++]);
                            }

                        }

                        var res = await _aggregator.RunSingle("1", config.Code, null, observations,
                            model.Config);

                        results.Add(new ValidationResult()
                        {

                            Input=string.Join(", ", observations.Select(e=>e.Code+": "+e.Value.ToString())),
                            ExpectedOutput= expectedValue,
                            Output=res.Value,
                            Index=lineCount++
                        });
                    }

                    
                   
                }

            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(results);


        }
    }
}
