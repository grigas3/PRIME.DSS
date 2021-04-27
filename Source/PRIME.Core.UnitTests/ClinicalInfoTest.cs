using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PRIME.Core.Common.Extensions;
using PRIME.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.UnitTests
{

    [TestClass]
    public class ClinicalInfoTest
    {
        [TestMethod]

        public void TestClinicalInfo1()
        {



            //            Current medication: levodopa and DA
            //Tremor at hands: mild, 0
            //Tremor at rest: mild, 0
            //Rigidity: moderate
            //Gait: moderate
            //Bradykinesia: moderate
            //Dyskinesias duration: mild
            //Dyskinesia intensity: mild
            //Offs duration: moderate
            //FoG: moderate
            //Activity: mild, 80 % independent
            //NMSS: severe, score is 85
            //Cognition: mild
            //Hallucinations: mild, score is 0
            //BIS - 11: mild
            var info = JsonConvert.SerializeObject(new ClinicalInfoCollection(){



                            new ClinicalInfo()
                            {
                                Value="moderate",
                                Code="STBRAD30",
                                Name="bradykinesia",
                                CreatedBy="Test",
                                Timestamp=DateTime.Now.ToUnixTimestamp(),
                                Priority="Low",
                                Category="Motor",



                            },


                            new ClinicalInfo()
                            {
                                Value="mild",
                                Code="STTRMR30",
                                Name="tremor at hands",
                                CreatedBy="Test",
                                Timestamp=DateTime.Now.ToUnixTimestamp(),
                             Priority="Low",
                                Category="Motor",


                            },


                            new ClinicalInfo()
                            {
                                Value="mild",
                                Code="STTRMP30",
                                Name="tremor at rest",
                                CreatedBy="Test",
                                Priority="Low",
                                Category="Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()



                            },


                            new ClinicalInfo()
                            {
                                Value="mild",
                                Code="STDYSS30",
                                Name="dyskinesia intensity",
                                CreatedBy="Test",
                                Priority="Low",
                                Category="Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },



                            new ClinicalInfo()
                            {
                                Value="mild",
                                Code="STDYSD30",
                                Name="dyskinesia duration",
                                CreatedBy="Test",
                                Priority="Low",
                                Category="Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },

                            new ClinicalInfo()
                            {
                                Value="moderate",
                                Code="STUPDRSG",
                                Name="gait",
                                CreatedBy="Test",
                                Priority="High",
                                Category="Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },





                            new ClinicalInfo()
                            {
                                Value="moderate",
                                Code="STFOG",
                                Name="freezing of gait",
                                CreatedBy="Test",
                                     Priority="High",
                                Category="Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },

                            new ClinicalInfo()
                            {
                                Value="moderate",
                                Code="STOFFDUR",
                                Name="offs duration",
                                CreatedBy="Test",
                                     Priority="High",
                                Category="Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },


                                  new ClinicalInfo()
                            {
                                Value="mild",
                                Code="HALLUC",
                                Name="hallucinations",
                                CreatedBy="Test",
                                     Priority="Low",
                                Category="Non-Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },

                             new ClinicalInfo()
                            {
                                Value="mild",
                                Code="BIS11",
                                Name="BIS-11",
                                CreatedBy="Test",
                                 Priority ="Low",
                                Category="Non-Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },
                               new ClinicalInfo()
                            {
                                Value="mild",
                                Code="MOOD",
                                Name="Mood",
                                CreatedBy="Test",
                                     Priority="Low",
                                Category="Non-Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },
                               new ClinicalInfo()
                               {
                                   Value = "mild",
                                   Code = "COGNITION",
                                   Name = "Cognition",
                                   CreatedBy = "Test",
                                   Priority = "Low",
                                   Category = "Non-Motor",
                                   Timestamp = DateTime.Now.ToUnixTimestamp()


                               },

                                 new ClinicalInfo()
                            {
                                Value="severe",
                                Code="NMSS",
                                Name="NMSS",
                                CreatedBy="Test",

                                     Priority="Low",
                                Category="Non-Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            },
                         
                               new ClinicalInfo()
                            {
                                Value="high",
                                Code="activity",
                                Name="Activity",
                                CreatedBy="Test",
                                     Priority="Low",
                                Category="Non-Motor",
                                Timestamp=DateTime.Now.ToUnixTimestamp()


                            } });



            Assert.IsNotNull(info);


                           
        }
    }
}
