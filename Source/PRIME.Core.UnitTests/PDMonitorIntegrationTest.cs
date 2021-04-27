using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.Services.FHIR;


namespace PRIME.Core.UnitTests
{

    [TestClass]
    public class PDMonitorIntegrationTest
    {


        private FhirProxyConfiguration CreateProxy()
        {

            return new FhirProxyConfiguration()
            {
              Headers = new List<FhirHeader>(){},//new FhirHeader(){Key= "Content-Type",Value="application/json"}},
                UserName = "spyros",
                Password = "s123!@#",
                RequiresAuthentication = true,
                Url = "https://localhost:44341/api/fhir/bundle",
                AuthUrl = "https://localhost:44341/token"
            };
        }

        [TestMethod]
        public async Task TestFhirInit()
        {
            int patientId = 1677;

            FhirConditionRepository rep=new FhirConditionRepository(CreateProxy());

            await rep.Init(patientId.ToString());


            Assert.IsTrue(rep.HasBundle());

        }
        [TestMethod]
        public async Task TestPDMonitorOFF()
        {
            int patientId = 1677;
            FhirConditionRepository rep = new FhirConditionRepository(CreateProxy());

            await rep.Init(patientId.ToString());

            var off = rep.HasCondition("OFF", "http://www.pdmonitorapp.com",(e) =>
            {
                if (e is decimal? && (e as decimal?).HasValue)
                {
                    return (e as decimal?).Value > 15;
                }
                return false;
            });

            Assert.IsTrue(off.HasValue);
            

      

        }

    }
}
