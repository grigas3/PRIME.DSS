using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.DSS;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class DSSRunnerTest
    {

        [TestMethod]
        public void Test1()
        {

            var m = File.ReadAllText(Path.Combine(Environment.CurrentDirectory,"./TestData/Dexi/modelyesno.json"));
            var config = DSSConfig.FromString(m);

            Dictionary<string,string> values=new Dictionary<string, string>();
            values.Add("NMSS", "mild");
            var dssValues=DSSRunner.Run(config,values);

            Assert.IsTrue(dssValues.Count()>0);

        }
    }

}