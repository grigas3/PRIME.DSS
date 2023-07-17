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
    public class AggregatorsTest
    {




        [TestMethod]
        public void SaveAggregation()
        {
            DummyAggrDefinitionProvider aggrProvider = new DummyAggrDefinitionProvider();
            var def = aggrProvider.GetConfigFromCode("UPDRS");

            AggrConfig.SaveToFile(def, "updrs.json");

            var defTarget=AggrConfig.LoadFromFile( "updrs.json");

            Assert.AreEqual(def.AggregationType, defTarget.AggregationType);


        }
        
        /// <summary>
        /// Test ONOFF Total aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task TotalOFFTime_Test()
        {

            string patientId = "234234234";

            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");


            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            var observations = await aggregator.Run(patientId, "STOFFDUR",null, null);

            Assert.IsTrue(observations.Count() > 0);
            var mean = observations.Select(e => e.Value).Average();
            Assert.IsTrue(Math.Abs(mean - 4*0.20) < 1, $"OFF Mean time {(mean)}");
                        
        }


        /// <summary>
        /// Test UPDRS Total aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task TotalUPDRS_Total_Test()
        {

            string patientId = "234234234";

            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");

            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            var observations=await aggregator.Run(patientId, "UPDRSTOTAL", null, null);

            Assert.IsTrue(observations.Count() > 0);
            var mean = observations.Select(e => e.Value).Average();
            Assert.IsTrue(Math.Abs(mean - 10.8584) < 0.1,$"UPDRS mean score is {mean}");


        }



        /// <summary>
        /// Mean Fluctuation Index Aggregation
        /// </summary>
        /// <returns></returns>
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        [DataRow(3, 1)]
        [DataRow(4, 1)]

        [TestMethod]
        public async Task Test_AnyThreshold(double value, double expValue)
        {

            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/daytimeSleepiness.json");
            PRIMEAggregator aggregator = new PRIMEAggregator(null);
            var res = await aggregator.RunSingle("1", "DAYSLEEP", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=value,
                    Code="DAYSLEEP",
                    CodeNameSpace = "www.prime.com"


                }
            }, defTarget);

            Assert.AreEqual(expValue,res.Value,  0.01);


        }

        /// <summary>
        /// Test UPDRS Day aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task TotalUPDRS_DayTest()
        {

            string patientId = "234234234";

            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");
            

            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            var observations = await aggregator.Run(patientId, "UPDRSDAY", null, null);

            Assert.IsTrue(observations.Count() > 0);
            var mean = observations.Select(e => e.Value).Average();
            Assert.IsTrue(Math.Abs(mean - 10.8584) < 0.1, $"UPDRS mean score is {mean}");




        }

        /// <summary>
        /// Test UPDRS Time Aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task TotalUPDRS_Time_Test()
        {

            string patientId = "234234234";

            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");

            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            var observations = await aggregator.Run(patientId, "UPDRS", null, null);

            Assert.IsTrue(observations.Count() > 0);
            var mean = observations.Select(e => e.Value).Average();
            Assert.IsTrue(Math.Abs(mean - 10.8584) < 0.1, $"UPDRS mean score is {mean}");

        }

        /// <summary>
        /// Test UPDRS Time Aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task TotalUPDRS_Max_Test()
        {

            string patientId = "234234234";

            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");


            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            var observations = await aggregator.Run(patientId, "UPDRS", null,null,"max");

            Assert.IsTrue(observations.Count() ==1);
            var max = observations.Select(e => e.Value).Max();
            Assert.IsTrue(Math.Abs(max - 18.4114) < 0.1, $"UPDRS max score is {max}");

        }



        /// <summary>
        /// Mean Fluctuation Index Aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task MFI_Test()
        {

            string patientId = "234234234";
            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");


            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            //var mfiObservation = await aggregator.Run(patientId, "STFLUCT", null, null, null);
            var mfiObservation = await aggregator.Run(patientId, "UPDRS", null, null, null, "mfi");



            Assert.IsTrue(mfiObservation.Count() == 1);
       
            var mfi = mfiObservation.Select(e => e.Value).Average();
          
            Assert.IsTrue(Math.Abs((mfi) - 7.552975) < 0.1, $"MFI score is {mfi}");

        }


        /// <summary>
        /// Mean Fluctuation Index Aggregation
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task MFI_Test2()
        {

            string patientId = "234234234";
            IDataProxy proxy = new DummyDataProxy(patientId, @".\TestData\symptoms.txt");


            GenericAggregator aggregator = new GenericAggregator(proxy, null, new DummyAggrDefinitionProvider());

            var mfiObservation = await aggregator.Run(patientId, "STFLUCT", null, null, null, null);
            //var mfiObservation = await aggregator.Run(patientId, "UPDRS", null, null, "mfi");



            Assert.IsTrue(mfiObservation.Count() == 1);

            var mfi = mfiObservation.Select(e => e.Value).Average();

            Assert.IsTrue(Math.Abs((mfi) - 7.552975) < 0.1, $"MFI score is {mfi}");

        }

        [TestMethod]
        public async Task AggrSigmoid_Test1()
        {
            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/off.json");
            PRIMEAggregator aggregator=new PRIMEAggregator(null);
            var res=await aggregator.RunSingle("1", "OFF", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=4,
                    Code="OFF",
                    CodeNameSpace = "www.prime.com"


                }
            },defTarget);

            Assert.AreEqual(res.Value,0.82,0.01);

        }

        [TestMethod]
        public async Task AggrSigmoid_Test5()
        {
            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/off4.json");
            PRIMEAggregator aggregator = new PRIMEAggregator(null);
            var res = await aggregator.RunSingle("1", "OFFTIME", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=100,
                    Code="OFFTIME",
                    CodeNameSpace = "www.prime.com"
                }
            }, defTarget);
            Assert.AreEqual(2, res.Value, 0.01);
        }



        [TestMethod]
        public async Task AggrSigmoid_Test2()
        {
            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/off2.json");
            PRIMEAggregator aggregator = new PRIMEAggregator(null);
            var res = await aggregator.RunSingle("1", "OFFTIME", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=90,
                    Code="OFFTIME",
                    CodeNameSpace = "www.prime.com"
                }
            }, defTarget);
            Assert.AreEqual(1,res.Value,  0.01);
        }

        [TestMethod]
        public async Task AggrSigmoid_Test3()
        {
            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/off2.json");
            PRIMEAggregator aggregator = new PRIMEAggregator(null);
            var res = await aggregator.RunSingle("1", "OFFTIME", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=0,
                    Code="OFFTIME",
                    CodeNameSpace = "www.prime.com"
                }
            }, defTarget);

            Assert.AreEqual(0, res.Value, 0.01);

        }


        [TestMethod]
        public async Task AggrSigmoid_Test4()
        {
            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/off2.json");
            PRIMEAggregator aggregator = new PRIMEAggregator(null);

            var res = await aggregator.RunSingle("1", "OFFTIME", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=30,
                    Code="OFFTIME",
                    CodeNameSpace = "www.prime.com"


                }
            }, defTarget);

            Assert.AreEqual(0.5, res.Value, 0.01);

        }


        [DataRow(-100, 0.0)]
        [DataRow(0,0.38)]
        [DataRow(4, 0.82)]
        [DataRow(100, 1)]
        [TestMethod]
        public async Task AggrSigmoid_TestMulti(double value,double expValue)
        {


            var defTarget = AggrConfig.LoadFromFile("./TestData/Aggregators/off.json");



            PRIMEAggregator aggregator = new PRIMEAggregator(null);

            var res = await aggregator.RunSingle("1", "OFF", "www.prime.com", new List<PDObservation>()
            {
                new PDObservation()
                {
                    Value=value,
                    Code="OFF",
                    CodeNameSpace = "www.prime.com"


                }
            }, defTarget);

            Assert.AreEqual(res.Value,expValue, 0.01);

        }

    }
}
