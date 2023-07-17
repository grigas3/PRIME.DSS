using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    class DummyRepositoryServiceTests
    {

        [TestMethod]
        public async Task AddGetTest()
        {
            
            DummyRepositoryService repositoryService=new DummyRepositoryService();

            var t=await repositoryService.GetAsync<DSSModel>();
            Assert.AreEqual(0,t.Count());


            DSSModel model = new DSSModel() {Id = 1,Code="TEST"};

            await repositoryService.InserOrUpdateAsync(model);
            var t1 = await repositoryService.GetAsync<DSSModel>();
            Assert.AreEqual(1, t1.Count());

            var m1=await repositoryService.FindAsync<DSSModel>(1);

            Assert.IsNotNull(m1);

            Assert.AreEqual("TEST",m1.Code);
        }

    }
}
