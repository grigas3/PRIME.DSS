using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS.Treatment;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class VariableController : Controller
    {

        private readonly IRepositoryService _context;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="logger">Logger</param>
        public VariableController(IRepositoryService context)
        {
            _context = context;
        }

        public class VariableEntry
        {
            public string Code { get; set; }
            public string CodeNameSpace { get; set; }
            public bool Exists { get; set; }
            public string Aggregator { get; set; }
            public string CDSS { get; set; }
            public System.Collections.Generic.List<string> Models { get; set; }

            public string ModelStr
            {
                get { return string.Join(",", Models); }
            }

            public string Client { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        ///     Get DSS Config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            List<DSSModel> models = null;
            List<AggrModel> aggr = null;

            if (!string.IsNullOrEmpty(id))
            {
                int idn = int.Parse(id);
                 models = (await _context.GetAsync<DSSModel>(e=>e.CDSClientId== idn)).ToList();
                 aggr = (await _context.GetAsync<AggrModel>(e => e.CDSClientId == idn)).ToList();
            }

            else
            {
                models = (await _context.GetAsync<DSSModel>()).ToList();
                aggr = (await _context.GetAsync<AggrModel>()).ToList();
            }

            System.Collections.Generic.List<VariableEntry> entries=new System.Collections.Generic.List< VariableEntry>();

            foreach (var model in models)
            {

                if (!(string.IsNullOrEmpty(model.Config)))
                {
                    var classifier = TreatmentClassifier.FromJson(model.Config);
                    foreach (var c in classifier.Variables)
                    {
                        var codeNamespace = "PRIME";
                        if (!string.IsNullOrEmpty(c.CodeNameSpace))
                        {
                            codeNamespace = c.CodeNameSpace;
                        }

                        if (entries.Any(e => e.Code == c.Code && e.CodeNameSpace == codeNamespace))
                        {
                            var prevEntry =
                                entries.FirstOrDefault(e => e.Code == c.Code && e.CodeNameSpace == codeNamespace);
                            prevEntry?.Models.Add(model.Name);
                            continue;
                        }


                        VariableEntry entry = new VariableEntry()
                        {
                            Code = c.Code,
                            Name=c.Name,
                            CodeNameSpace = codeNamespace,
                            Models = new List<string>() {model.Name},
                            Client = model.CDSClientName,
                        };

                        if (aggr.Any(e =>e.CDSClientId==model.CDSClientId&& e.Code == c.Code))
                        {
                            entry.Exists = true;
                            entry.Aggregator = aggr.FirstOrDefault((e => e.Code == c.Code))?.Name;
                        }
                        if (models.Any(e => e.CDSClientId == model.CDSClientId && e.Code == c.Code))
                        {
                            entry.Exists = true;
                            entry.CDSS = models.FirstOrDefault((e => e.Code == c.Code))?.Name;
                        }

                        entries.Add(entry);
                    }
                }
            }



            return Ok(entries);
                



        }
    }
}