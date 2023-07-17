using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class Netmed360IntegrationTest
    {


        private FhirProxyConfiguration CreateProxy()
        {

            return new FhirProxyConfiguration()
            {
                Headers = new List<FhirHeader>() { },//new FhirHeader(){Key= "Content-Type",Value="application/json"}},
                UserName = "superuser@prime",
                Password = "123456",
                RequiresAuthentication = true,
                Url = "https://195.251.192.85:441/api/fhir/",
                AuthUrl = "https://195.251.192.85:441/token"
            };
        }

        [TestMethod]
        public async Task TestFhirInit()
        {
            string patientId = "9987662b-1f7a-ed11-9e61-1cc1de335c14";

            FhirConditionRepository rep = new FhirConditionRepository(CreateProxy());

            await rep.Init(patientId);


            Assert.IsTrue(rep.HasBundle());
            

        }
        [TestMethod]
        public async Task TestPDMonitorOFF()
        {
            int patientId = 1677;
            FhirConditionRepository rep = new FhirConditionRepository(CreateProxy());

            await rep.Init(patientId.ToString());

            var off = rep.HasCondition("OFF", "http://www.pdmonitorapp.com", (e) =>
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