using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRIME.Core.Common.Enums;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Web.Entities;
using PRIME.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Utility;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// Data Import Controller
    /// </summary>
    
    public class DataImportController : Controller
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
        /// <param name="alertEvaluator"></param>        
        /// <param name="logger">Logger</param>
        public DataImportController(IRepositoryService context, ILogger<DSSController> logger)
        {          
            _context = context;
            _logger = logger;
           
            
        }
        #endregion
        private static List<AggrModel> CreateAggregators(int cdsClient)
        {

            List<Tuple<string, string,string>> models = new List<Tuple<string, string, string>>()
            {
                Tuple.Create("OFFTIME","OFF Time", @".\TestData\Aggregators\off2.json"),
                Tuple.Create("DYSTIME","DYS Time", @".\TestData\Aggregators\dyskinesia2.json"),
                Tuple.Create("GAIT","Gait", @".\TestData\Aggregators\gait.json"),
                Tuple.Create("BRAD", "Bradykinesia", @".\TestData\Aggregators\brad.json"),
                Tuple.Create("TREMOR","Tremor", @".\TestData\Aggregators\tremor2.json"),
                Tuple.Create("COGDIS","Cognitive disorder", @".\TestData\Aggregators\cogDisorder.json"),
                Tuple.Create("DAYSLEEP","Daytime sleepiness", @".\TestData\Aggregators\daytimeSleepiness.json"),
                Tuple.Create("COGDIS","Cognitive disorder", @".\TestData\Aggregators\cogDisorder.json"),
                Tuple.Create("HALLUC","Hallucinations ", @".\TestData\Aggregators\hallucinations.json"),
                Tuple.Create("IMPULS","Impulsivity", @".\TestData\Aggregators\impulsivity.json"),
                Tuple.Create("DEPRESS","Depression", @".\TestData\Aggregators\depression.json"),
                Tuple.Create("EMPLOY","Employment", @".\TestData\Aggregators\employ.json"),
                Tuple.Create("AGE","Age", @".\TestData\Aggregators\age.json"),
            };
            List<AggrModel> aggr = new List<AggrModel>();
            int count = 1;
            foreach (var c in models)
            {
                aggr.Add(CreateAggregator(c.Item1, c.Item2,c.Item3, cdsClient));

            }
            return aggr;
        }
        private static AggrModel CreateAggregator(string code,string descr, string file, int clientid)
        {
            return new AggrModel()
            {
                
                Code = code,
                Name=descr,
                Description = descr,
                Version = "1.0",
                CDSClientId = clientid,
                Config = System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory,
                    file))
            };
        }
        public async Task<ActionResult> ImportChangeAggregators(int clientId)
        {

            var aggr = CreateAggregators(clientId);
            foreach (var a in aggr)
            {
                await _context.InserOrUpdateAsync(a);

            }


            return Json("OK");


        }

    }
}