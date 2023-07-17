using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.DSS;
using PRIME.Core.DSS.Dexi;

namespace PRIME.Core.UnitTests
{

    [TestClass]
    public class DexiTests
    {

        [TestMethod]
        public void Test1()
        {
            var modelFile = "./DexiModels/ModelHow.dxi";
            var config=DSSConfig.CreateFromModel(modelFile, "Change");
            

            Assert.IsTrue(true);

        }







    }
}
