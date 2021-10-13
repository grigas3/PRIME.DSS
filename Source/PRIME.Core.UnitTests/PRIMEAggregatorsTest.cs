using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.Aggregators;
using PRIME.Core.Aggregators.Testing;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Services.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PRIME.Core.Models;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class PRIMEAggregatorsTest
    {

        

        /// <summary>
        /// Test ONOFF Total aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task TotalHighDyskinesia_Test()
        {

            string patientId = "234234234";


            PRIMEAggregator aggregator = new PRIMEAggregator( null);

            var observations = new List<PDObservation>()
            {
                new PDObservation() {Code = "DYSKINESIA",CodeNameSpace = "www.pdmonitorapp.com",Value = 6},
                new PDObservation() {Code = "DYSKINESIA",CodeNameSpace = "www.pdmonitorapp.com", Value = 10}

            };

            var r=await aggregator.Run(patientId, "DYSKINESIA","PRIME",observations, File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData\\Aggregators\\")+"dyskinesia.json"));
            Assert.IsTrue(r.Count() == 1);
            var mean = r.Select(e => e.Value).Average();
            Assert.AreEqual(1, mean, 0.01);

        }

        [TestMethod]
        public async Task TotalNoDyskinesia_Test()
        {

            string patientId = "234234234";

            PRIMEAggregator aggregator = new PRIMEAggregator(null);

            var observations = new List<PDObservation>()
            {
                new PDObservation() {Code = "DYSKINESIA",CodeNameSpace = "www.pdmonitorapp.com",Value = 2},
                new PDObservation() {Code = "DYSKINESIA",CodeNameSpace = "www.pdmonitorapp.com", Value = 3}

            };

            var r = await aggregator.Run(patientId, "DYSKINESIA", "PRIME", observations, File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData\\Aggregators\\") + "dyskinesia.json"));
            Assert.IsTrue(r.Count() == 1);
            var mean = r.Select(e => e.Value).Average();
            Assert.AreEqual(0, mean, 0.01);

        }

    }
}
