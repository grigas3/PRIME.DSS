using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using PRIME.Core.Aggregators;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS;
using PRIME.Core.DSS.Treatment;
using PRIME.Core.Models;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// Controller for Validating Aggregation Model
    /// </summary>
    [Route("api/v1/[controller]")]
    public class DSSValidate : Controller
    {
        private readonly ICDSService _cdsService;
        private readonly IAggregator _aggregator;
        private readonly IRepositoryService _repository;
        public DSSValidate(ICDSService cdsService, IRepositoryService rep, IAggregator aggr)
        {
            _repository = rep;
            _aggregator = aggr;
            _cdsService = cdsService;
        }

        public class ValidationResult
        {
            public int Index { get; set; }
            public string Input { get; set; }
            public string Output { get; set; }
            public double ActualOutput { get; set; }
            public double ExpectedOutput { get; set; }
            public bool Valid { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {

            var model = await this._repository.FindAsync<DSSModel>(int.Parse(id));

            if (model == null)
                return BadRequest("Model is empty");

            if (model.Config == null)
                return BadRequest("Model Config is empty");

            var aggrModels = (await this._repository.GetAsync<AggrModel>(e => e.CDSClientId == model.CDSClientId)).ToList();
            List<AggrConfig> aggrConfigs = new List<AggrConfig>();
            foreach (var c in aggrModels)
            {
                if (!string.IsNullOrEmpty(c.Config))
                    aggrConfigs.Add(AggrConfig.FromString(c.Config));

            }

            List<string> codes = new List<string>();
            List<string> namespaces = new List<string>();

            var config = TreatmentClassifier.FromJson(model.Config);
            foreach (var c in config.Variables)
            {

                var s = aggrConfigs.FirstOrDefault(e => e.OutputCode == c.Code);
                if (s == null)
                {
                    codes.Add(c.Code);
                    namespaces.Add("PRIME");
                }
                else
                {
                    codes.Add(s.Code);
                    if (!string.IsNullOrEmpty(s.CodeNameSpace))
                        namespaces.Add(s.CodeNameSpace);
                    else
                    {
                        namespaces.Add("PRIME");
                    }
                }

            }
            codes.Add(model.Code);
            namespaces.Add("PRIME");

            var stream = new MemoryStream();
            using (var writeFile = new StreamWriter(stream))
            {

                writeFile.WriteLine(string.Join(";", codes));
                writeFile.WriteLine(string.Join(";", namespaces));


            }

            return File(stream.ToArray(), "text/csv", "Template.csv");

        }

        public class ValidationReport
        {
            public List<ValidationResult> results { get; set; }

            public int FalsePositives { get; set; }
            public int FalseNegatives { get; set; }
            public int TruePositives { get; set; }
            public int TrueNegatives { get; set; }
        }

        private double parseValue(string value)
        {
            if (string.IsNullOrEmpty((value)))
                return 0;

            if (value.ToLower() == "true")
                return 1;
            if (value.ToLower() == "false")
                return 0;

            if (value.ToLower() == "mild")
                return 2;
            if (value.ToLower() == "moderate")
                return 1;
            if (value.ToLower() == "severe")
                return 0;
            if (value.ToLower() == "low")
                return 1;

            if (value.ToLower() == "high")
                return 0;

            if (value.ToLower() == "maybe")
                return 0;
            if (value.ToLower() == "no_change")
                return 0;
            if (value.ToLower() == "change")
                return 1;
            return double.Parse((value));

        }

        [HttpPost]
        public async Task<ActionResult> Post(string id, string code,IFormFile file)
        {
            ValidationReport result=new ValidationReport();
            
            result.results = new List<ValidationResult>();
            
            
            var model = await this._repository.FindAsync<DSSModel>(int.Parse(id));

            if (model == null)
                return BadRequest("Model is empty");

            if (model.Config == null)
                return BadRequest("Model Config is empty");

           // var aggrModels = (await this._repository.GetAsync<AggrModel>(e => e.CDSClientId == model.CDSClientId)).ToList();

            var aggrModels=new List<AggrModel>();
    //foreach ()
            {

                var formFile = file;
                if (formFile.Length > 0)
                {
                    StreamReader str = new StreamReader(formFile.OpenReadStream());

                    string line = "";
                    var header = await str.ReadLineAsync();
                    string[] codes = null;
                    if (header != null)
                    {
                        codes = header.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (codes == null)
                        return BadRequest("No Header for codes");

                    var nheader = await str.ReadLineAsync();
                    string[] namespaces = null;
                    if (nheader != null)
                    {
                        namespaces = header.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (namespaces == null)
                        return BadRequest("No Header for namespaces");

                    if (codes.Length != namespaces.Length)
                    {
                        return BadRequest("Codes and namespaces have different length");
                    }

                   



                    int lineCount = 0;
                    while ((line = await str.ReadLineAsync()) != null)
                    {
                        var crepository = new BaseCDSController.ConditionRepository();
                        var values = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); 
                        List<PDObservation> observations = new List<PDObservation>();
                        int count = 0;
                        double expectedValue = 0;
                        foreach (var c in codes)
                        {
                            if (c != model.Code)
                            {
                                crepository.AddCondition(c, namespaces[count], parseValue(values[count]));
                                observations.Add(new PDObservation()
                                { Code = c, Value = parseValue(values[count]), CodeNameSpace = "PRIME" });

                            }

                            else
                            {
                                expectedValue = parseValue(values[count]);
                            }

                            count++;

                        }

                        if (!string.IsNullOrEmpty(code))
                        {
                            model.Code = code;

                        }

                       var res=await CDSSMainEvaluator.EvaluateDSSAsync(_cdsService, _aggregator, _repository, crepository, "1",
                           aggrModels,   model);


                       var cards = res.Item1 as IEnumerable<Card>;
                        var stats = res.Item2;
                        var outputs = res.Item3;
                        var evalue=(int)expectedValue;
                        var avalue = (int) (cards.Any(e => e.Code.ToLower() == model.Code.ToLower())
                            ? cards.First(e => e.Code.ToLower() == model.Code.ToLower()).Score
                            : 0);
                        result.FalseNegatives += (evalue == 1 && avalue == 0)?1:0;
                        result.FalsePositives += (evalue == 0 && avalue == 1) ? 1 : 0;
                        result.TrueNegatives += (evalue == 0 && avalue == 0) ? 1 : 0;
                        result.TruePositives += (evalue == 1 && avalue == 1) ? 1 : 0;
                        result.results.Add(new ValidationResult()
                        {

                            Input = string.Join(", ", observations.Select(e => e.Code + ": " + e.Value.ToString())),
                            ExpectedOutput = expectedValue,
                            Output= string.Join(", ", outputs.Select(e => e.Code + ": " + e.Value.ToString())),//
                            ActualOutput = 
                            cards.Any(e => e.Code.ToLower() == model.Code.ToLower()) ? cards.First(e => e.Code.ToLower() == model.Code.ToLower()).Score : 0,
                            Index = lineCount++
                        });

                    }
                }

            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(result);


        }
    }
}