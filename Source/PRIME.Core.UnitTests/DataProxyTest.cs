using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core;

using PRIME.Core.Models;
using System.Threading.Tasks;
using PRIME.Core.Services;
using PRIME.Core.UnitTests;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Extensions;
using PRIME.Core.Common.Testing;
using PRIME.Core.Common.Models;

namespace PRIME.JobRunner2.Tests.Controllers
{
    [TestClass]
    public class DataProxyTest
    {

      

        [TestMethod]
        public void TestObservations()
        {
          
            IDataProxy proxy = new DataProxy(new DummyCredentialProvider());
            var observations = proxy.Get<PDObservation>( 10, 0, "{patientid:\"5900aa2a2f2cd563c4ae3027\",deviceid:\"\",codeid:\"PDTFTS_MAX\",datefrom:0,dateto:0,aggr:\"total\"}", null).Result;
            Assert.IsTrue(observations.Count() ==1);

        }
        [TestMethod]
        public async Task TestTimeObservations()
        {
           

               IDataProxy proxy = new DataProxy(new DummyCredentialProvider());
            

            var observations = await proxy.Get<PDObservation>(  10, 0, "{patientid:\"5900aa2a2f2cd563c4ae3027\",deviceid:\"\",codeid:\"STBRADS30\",datefrom:0,dateto:0,aggr:\"time\"}", null);
            Assert.IsTrue(observations.Count()>0);

        }

        [TestMethod]
        public async Task TestLastMonthObservations()
        {
            IDataProxy proxy = new DataProxy(new DummyCredentialProvider());            
            var observations = await proxy.Get<PDObservation>(  10, 0, String.Format("{{patientid:\"{1}\",deviceid:\"\",codeid:\"PDTFTS_MAX\",datefrom:0,dateto:0,aggr:\"total\"}}", DateTime.Now.AddMonths(-1).ToUnixTimestamp(), "5900aa2a2f2cd563c4ae3027"), null);
            Assert.IsTrue(observations.Count() == 1);

        }



        [TestMethod]
        public void TestGetPatient()
        {
            IDataProxy proxy = new DataProxy(new DummyCredentialProvider());
          
            var patient = proxy.Get<PDPatient>("5762a1cd2f2cd5a244ca6855");
            Assert.IsTrue(patient != null);

        }

   



    }
}
