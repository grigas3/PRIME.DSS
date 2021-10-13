using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PRIME.Core.DSS.Fuzzy;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class ManagePDTests
    {

        private static FuzzyRule CreateManagePDDefinitionBaseline()
        {
            return new FuzzyRule()
            {
                Fuzzy=false,
                
                OrRules = new List<OrRule>()
                {
                    
                    new OrRule("ELIGIBLE")
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){Name="DBS Age" ,Property = "AGE",Operator = "LessThan",Value=70},
                        new FuzzyFunc(){Name="Diagnosed With Parkinson's", Property = "PARKINSON",Operator = "Equals",Value=1},
                        }
                    },
                   


                }

            };
        }
        private static FuzzyRule CreateManagePDDefinitionNoCat1()
        {
            return new FuzzyRule()
            {
                Fuzzy = true,
                OrRules = new List<OrRule>()
                {

                    new OrRule("NOS")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "ORALLEVODOPA",Not=true},
                        new FuzzyFunc(){ Property = "OFFTIME",Not=true},
                        new FuzzyFunc(){ Property = "FLUCTUATIONS",Not=true},
                        new FuzzyFunc(){ Property = "DYSKINESIA",Not=true},
                        new FuzzyFunc(){ Property = "ADL",Not=true},
                        }

                    },
                


                }

            };
        }

        private static FuzzyRule CreateManagePDDefinitionCat1()
        {
            return new FuzzyRule()
            {
                Fuzzy = true,
                OrRules = new List<OrRule>()
                {

                    new OrRule("ORALLEVODOPA")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "ORALLEVODOPA"},
                        }

                    },
                    new OrRule("OFFTIME")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "OFFTIME"},
                        }

                    },
                    new OrRule("FLUCTUATIONS")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "FLUCTUATIONS"},
                        }
                    },
                    new OrRule("DYSKINESIA")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "DYSKINESIA"},
                        }

                    },
                    new OrRule("ADL")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "ADL"},
                        }

                    },


                }

            };
        }


        private static FuzzyRule CreateManagePDDefinitionCat3()
        {
            return new FuzzyRule()
            {
                Fuzzy = true,
                OrRules = new List<OrRule>()
                {

                    new OrRule("NONMOTOROFF")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "NONMOTOROFF"},
                        }

                    },
                    new OrRule("OFFTIME2")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "OFFTIME2"},
                        }

                    },
                    new OrRule("FLUCTUATIONS2")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "FLUCTUATIONS2"},
                        }
                    },
                    new OrRule("DYSKINESIA2")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "DYSKINESIA2"},
                        }

                    },
                    new OrRule("ADL2")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){Name="ADL2", Property = "ADL2"},}

                    },
                    new OrRule("FALLS")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){Name="FALLS", Property = "FALLS"},}

                    },

                    new OrRule("DYSTONIA")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "DYSTONIA"},
                        }

                    },
                    new OrRule("FOG")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "FOG"},
                        }

                    },
                    new OrRule("ICD")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "ICD"},
                        }

                    },
                    new OrRule("HALLUCINATIONS")
                    {  AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "HALLUCINATIONS"},
                        }

                    },
                }

            };
        }

        private static FuzzyCollection CreateManagePDCat2()
        {
            return new FuzzyCollection(new List<string>()
            {
                "AGE","PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",

                "OFFTIME2", "FLUCTUATIONS2", "DYSKINESIA2", "ADL2","NONMOTOROFF","FALLS","DYSTONIA","FOG","ICD","HALLUCINATIONS"

            })
            {
                
               
              
                Rules = new List<FuzzyRule>(){
                    CreateManagePDDefinitionBaseline(),



                    new FuzzyRule(){
                        Fuzzy =true,
                OrRules = new List<OrRule>()
                {

                    new OrRule("Oral Levodopa")
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "ORALLEVODOPA"},
                        new FuzzyFunc(){ Property = "OFFTIME2",Not=true},
                        new FuzzyFunc(){ Property = "NONMOTOROFF",Not=true},
                        new FuzzyFunc(){ Property = "FLUCTUATIONS2",Not=true},
                        new FuzzyFunc(){ Property = "DYSKINESIA2",Not=true},
                        new FuzzyFunc(){ Property = "ADL2",Not=true},
                        new FuzzyFunc(){ Property = "FALLS",Not=true},
                        new FuzzyFunc(){ Property = "DYSTONIA",Not=true},
                        new FuzzyFunc(){ Property = "FOG",Not=true},
                        new FuzzyFunc(){ Property = "ICD",Not=true},
                        new FuzzyFunc(){ Property = "HALLUCINATIONS",Not=true},
                        }

                    },
                    new OrRule("OFF Time")
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "OFFTIME"},
                        new FuzzyFunc(){ Property = "OFFTIME2",Not=true},
                        new FuzzyFunc(){ Property = "NONMOTOROFF",Not=true},
                        new FuzzyFunc(){ Property = "FLUCTUATIONS2",Not=true},
                        new FuzzyFunc(){ Property = "DYSKINESIA2",Not=true},
                        new FuzzyFunc(){ Property = "ADL2",Not=true},
                        new FuzzyFunc(){ Property = "FALLS",Not=true},
                        new FuzzyFunc(){ Property = "DYSTONIA",Not=true},
                        new FuzzyFunc(){ Property = "FOG",Not=true},
                        new FuzzyFunc(){ Property = "ICD",Not=true},
                        new FuzzyFunc(){ Property = "HALLUCINATIONS",Not=true},
                        }

                    },
                    new OrRule("Fluctuations")
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "FLUCTUATIONS"},
                        new FuzzyFunc(){ Property = "OFFTIME2",Not=true},
                        new FuzzyFunc(){ Property = "NONMOTOROFF",Not=true},
                        new FuzzyFunc(){ Property = "FLUCTUATIONS2",Not=true},
                        new FuzzyFunc(){ Property = "DYSKINESIA2",Not=true},
                        new FuzzyFunc(){ Property = "ADL2",Not=true},
                        new FuzzyFunc(){ Property = "FALLS",Not=true},
                        new FuzzyFunc(){ Property = "DYSTONIA",Not=true},
                        new FuzzyFunc(){ Property = "FOG",Not=true},
                        new FuzzyFunc(){ Property = "ICD",Not=true},
                        new FuzzyFunc(){ Property = "HALLUCINATIONS",Not=true},
                        }

                    },
                    new OrRule("DYSKINESIA")
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "DYSKINESIA"},
                        new FuzzyFunc(){ Property = "OFFTIME2",Not=true},
                        new FuzzyFunc(){ Property = "NONMOTOROFF",Not=true},
                        new FuzzyFunc(){ Property = "FLUCTUATIONS2",Not=true},
                        new FuzzyFunc(){ Property = "DYSKINESIA2",Not=true},
                        new FuzzyFunc(){ Property = "ADL2",Not=true},
                        new FuzzyFunc(){ Property = "FALLS",Not=true},
                        new FuzzyFunc(){ Property = "DYSTONIA",Not=true},
                        new FuzzyFunc(){ Property = "FOG",Not=true},
                        new FuzzyFunc(){ Property = "ICD",Not=true},
                        new FuzzyFunc(){ Property = "HALLUCINATIONS",Not=true},

                        }
                    },
                    new OrRule("ADL")
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc(){ Property = "ADL"},

                        new FuzzyFunc(){ Property = "OFFTIME2",Not=true},
                        new FuzzyFunc(){ Property = "NONMOTOROFF",Not=true},
                        new FuzzyFunc(){ Property = "FLUCTUATIONS2",Not=true},
                        new FuzzyFunc(){ Property = "DYSKINESIA2",Not=true},
                        new FuzzyFunc(){ Property = "ADL2",Not=true},
                        new FuzzyFunc(){ Property = "FALLS",Not=true},
                        new FuzzyFunc(){ Property = "DYSTONIA",Not=true},
                        new FuzzyFunc(){ Property = "FOG",Not=true},
                        new FuzzyFunc(){ Property = "ICD",Not=true},
                        new FuzzyFunc(){ Property = "HALLUCINATIONS",Not=true},
                        }
                    },


                }
                }
                    }
            };
        }
        private static FuzzyCollection CreateManagePDCat1()
        {
            return new FuzzyCollection(new List<string>()
            {
                "AGE","PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",
                "OFFTIME2", "FLUCTUATIONS2", "DYSKINESIA2", "ADL2","NONMOTOROFF","FALLS","DYSTONIA","FOG","ICD","HALLUCINATIONS"

            })
            {
                Rules = new List<FuzzyRule>(){
                    CreateManagePDDefinitionBaseline(),
                    CreateManagePDDefinitionNoCat1()

                }
            };
        }


        private static FuzzyCollection CreateManagePDCat3()
        {
            return new FuzzyCollection(new List<string>()
            {
                "AGE","PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",
                "OFFTIME2", "FLUCTUATIONS2", "DYSKINESIA2", "ADL2","NONMOTOROFF","FALLS","DYSTONIA","FOG","ICD","HALLUCINATIONS"
            })
            {
               
             
              
                Rules = new List<FuzzyRule>(){
                    CreateManagePDDefinitionBaseline(),
                    CreateManagePDDefinitionCat1(),
                    CreateManagePDDefinitionCat3(),
                    }
            };
        }


        private static FuzzyCollection CreateManagePDDefinition()
        {
            return new FuzzyCollection(new List<string>() { "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL" })
            {
              
                
              
                Rules = new List<FuzzyRule>()
                {
                    new FuzzyRule(){
                        Fuzzy=false,
                OrRules = new List<OrRule>()
                {

                    new OrRule()
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="Oral Levodopa",
                            Property = "ORALLEVODOPA",
                            Operator = "GreaterThan",
                            Value =4
                        },
                        }
                    },
                    new OrRule()
                    { AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="OFF Time",
                            Property = "OFFTIME",
                            Operator = "GreaterThan",
                            Value =2.0
                        },
                        }
                    },
                    new OrRule()
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="Fluctuations",
                            Property = "FLUCTUATIONS",
                            Operator = "Equals",
                            Value =true
                        },
                        }

                    },
                    new OrRule()
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="Dyskinesia",
                            Property = "DYSKINESIA",
                            Operator = "Equals",
                            Value =true
                        },
                        }

                    },
                    new OrRule()
                    { AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Property = "ADL",
                            Operator = "GreaterThan",
                            Value =0.9
                        },
                        }
                    },


                }
                }
                    }

            };
        }


        private static FuzzyCollection CreateManagePDDefinitionFuzzy()
        {
            return new FuzzyCollection(new List<string>() { "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL" })
            {
                Rules = new List<FuzzyRule>()
                    {
                        
                        new FuzzyRule(){
                            Fuzzy=true,
                OrRules = new List<OrRule>()
                {

                    new OrRule("Oral Levodopa")
                    
                    {
                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="Oral Levodopa",
                            Property = "ORALLEVODOPA",
                          
                        }
                        }

                    },
                    new OrRule("OFF Time")
                    { AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="OFF Time",
                            Property = "OFFTIME",
                           
                        },
                        }

                    },
                    new OrRule("Fluctuations")
                    { AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="Fluctuations",
                            Property = "FLUCTUATIONS",
                     
                        },
                        }

                    },
                    new OrRule("Dyskinesia")
                    {

                        AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Name="Dyskinesia",
                            Property = "DYSKINESIA",
                           
                        },
                        }

                    },
                    new OrRule("ADL")
                    { AndVariables=new List<FuzzyFunc>(){
                        new FuzzyFunc()
                        {
                            Property = "ADL",
                            Operator = "GreaterThan",
                            
                        },
                        }
                    },

                    }
                }
                        }

            };
        }
        [TestMethod]
        public void ManagePD__General_Test1()
        {

            var profile = CreateManagePDDefinition();
            var d = new { ORALLEVODOPA = 0, OFFTIME = 2.1, DYSKINESIA = false, ADL = 0.0, FLUCTUATIONS = false };
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

            var res=FuzzyEngine.GetInference(profile, d);
            Assert.IsTrue(res.Result>0.5);
            
          
        }

        [TestMethod]
        public void ManagePD__General_Test2()
        {

            var profile = CreateManagePDDefinition();
            var d = new { ORALLEVODOPA = 0, OFFTIME = 0.1, DYSKINESIA = false, ADL = 0.0, FLUCTUATIONS = false };
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;
            var res = FuzzyEngine.GetInference(profile, d);
            Assert.IsTrue(res.Result < 0.5);
           
        }

        [TestMethod]
        public void ManagePD_General_Test3()
        {
            var profile = CreateManagePDDefinitionFuzzy();
            var d = new
            {
                ORALLEVODOPA = 0.5, OFFTIME = .0, DYSKINESIA = .0, ADL = 0.0, FLUCTUATIONS = 0.0,
                

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result ,0.5);

            Assert.AreEqual(res.Rule, "Oral Levodopa");
        
        }
        [TestMethod]
        public void ManagePD_Category2_Test2()
        {
            var profile = CreateManagePDCat2();
            var d = new
            {
                ORALLEVODOPA = 0.5,
                OFFTIME = .0,
                DYSKINESIA = .0,
                ADL = 0.0,
                FLUCTUATIONS = 0.0,
                OFFTIME2 = .0,
                FLUCTUATIONS2 = .0,
                DYSKINESIA2 = .0,
                ADL2 = 0.8,
                NONMOTOROFF = .0,
                FALLS = .0,
                DYSTONIA = .0,
                FOG = .0,
                ICD = .0,
                HALLUCINATIONS = .0

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.2,0.01);

            Assert.AreEqual(res.Rule, "Oral Levodopa");

        }

        [TestMethod]
        public void ManagePD_Category2_Test1()
        {
            var profile = CreateManagePDCat2();
            var d = new
            {
                ORALLEVODOPA = 0.5,
                OFFTIME = .0, DYSKINESIA = .0,
                ADL = 0.0, FLUCTUATIONS = 0.0 ,
                OFFTIME2 = .0,
                FLUCTUATIONS2 = .0,
                DYSKINESIA2 = .0,
                ADL2 = .0,
                NONMOTOROFF = .0,
                FALLS = .0,
                DYSTONIA = .0,
                FOG = .0,
                ICD = .0,
                HALLUCINATIONS = .0

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.5);

            Assert.AreEqual(res.Rule, "Oral Levodopa");

        }


        [TestMethod]
        public void ManagePD_Category2_Test3()
        {
            var profile = CreateManagePDCat2();
            var d = new
            {
                ORALLEVODOPA = 0.5,
                OFFTIME = .0,
                DYSKINESIA = .0,
                ADL = 0.0,
                FLUCTUATIONS = 0.0,
                OFFTIME2 = .0,
                FLUCTUATIONS2 = .0,
                DYSKINESIA2 = .0,
                ADL2 = .0,
                NONMOTOROFF = .0,
                FALLS = .0,
                DYSTONIA = .0,
                FOG = .0,
                ICD = .0,
                HALLUCINATIONS = .0,
                AGE=80

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.0);

         
        }


        [TestMethod]
        public void ManagePD_Category1_Test2()
        {
            var profile = CreateManagePDCat1();
            var d = new
            {
                ORALLEVODOPA = 0.1,
                OFFTIME = .0,
                DYSKINESIA = .0,
                ADL = 0.0,
                FLUCTUATIONS = 0.0,
            

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.9, 0.01);

            Assert.AreEqual(res.Variable, "ORALLEVODOPA");

        }



        /// <summary>
        /// Test with Not suitable age
        /// </summary>
        [TestMethod]
        public void ManagePD_Category1_Test3()
        {
            var profile = CreateManagePDCat1();
            var d = new
            {
                ORALLEVODOPA = 0.1,
                OFFTIME = .5,
                DYSKINESIA = .8,
                ADL = 0.0,
                AGE=80


            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.0, 0.01);

            
        }


        [TestMethod]
        public void ManagePD_Category1_Test1()
        {
            var profile = CreateManagePDCat1();
            var d = new
            {
                ORALLEVODOPA = 0.1,
                OFFTIME = .5,
                DYSKINESIA = .8,
                ADL = 0.0,
                AGE=50,
                PARKINSON=1

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.2, 0.01);

            Assert.AreEqual(res.Variable, "DYSKINESIA");

        }


        [TestMethod]
        public void ManagePD_Category3_Test1()
        {
            var profile = CreateManagePDCat3();
            var d = new
            {
                ORALLEVODOPA = 0.5,
                OFFTIME = .0,
                DYSKINESIA = .0,
                ADL = 0.0,
                FLUCTUATIONS = 0.0,
                OFFTIME2 = 0.0,
                FLUCTUATIONS2 = 0.0,
                DYSKINESIA2 = 0.0,
                ADL2 = 0.8,
                NONMOTOROFF = 0.0,
                FALLS = 0.0,
                DYSTONIA = 0.0,
                FOG = 0.0,
                ICD = 0.0,
                HALLUCINATIONS = 0.0

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.8, 0.01);

            Assert.AreEqual(res.Rule, "ADL2");

        }

        [TestMethod]
        public void ManagePD_Category3_Test2()
        {
            var profile = CreateManagePDCat3();
            var d = new
            {
                ORALLEVODOPA = 0.5,
                OFFTIME = .0,
                DYSKINESIA = .0,
                ADL = 0.0,
                FLUCTUATIONS = 0.0,
                OFFTIME2 = 0.0,
                FLUCTUATIONS2 = 0.0,
                DYSKINESIA2 = 0.5,
                ADL2 = 0.0,
                NONMOTOROFF = 0.0,
                FALLS = 1.0,
                DYSTONIA = 0.0,
                FOG = 0.0,
                ICD = 0.0,
                HALLUCINATIONS = 0.0

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 1);

            Assert.AreEqual(res.Rule, "FALLS");

        }


        [TestMethod]
        public void ManagePD_Category3_Test4()
        {
            var profile = CreateManagePDCat3();
            var d = new
            {
                ORALLEVODOPA = 0.5,
                OFFTIME = .0,
                DYSKINESIA = .0,
                ADL = 0.0,
                FLUCTUATIONS = 0.0,
                OFFTIME2 = 0.0,
                FLUCTUATIONS2 = 0.0,
                DYSKINESIA2 = 0.5,
                ADL2 = 0.0,
                NONMOTOROFF = 0.0,
                FALLS = 1.0,
                DYSTONIA = 0.0,
                FOG = 0.0,
                ICD = 0.0,
                HALLUCINATIONS = 0.0,
                PARKINSON=0

            };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0);

            

        }

        [TestMethod]
        public void ManagePD_MissingVariables_Test1()
        {
            var profile = CreateManagePDDefinitionFuzzy();
            var d = new { ORALLEVODOPA = 0.5, OFFTIME = .0, ADL = 0.0, FLUCTUATIONS = 0.0 };

            var res = FuzzyEngine.GetInference(profile, d);

            Assert.AreEqual(res.Result, 0.5);
            
            Assert.AreEqual(res.Rule, "Oral Levodopa");
            Assert.AreEqual(res.MissingVariables.Count, 1);
            Assert.AreEqual(res.MissingVariables[0], "DYSKINESIA");

        }

        [TestMethod]
        public void TestSerialize()
        {

            var profile1 = CreateManagePDCat1();
            var prettyString = JsonConvert.SerializeObject(profile1, Formatting.Indented);
            var fileWriter = new StreamWriter("managepdCat1.json");
            fileWriter.WriteLine(prettyString);
            fileWriter.Close();
            var profile2 = CreateManagePDCat1();
             prettyString = JsonConvert.SerializeObject(profile2, Formatting.Indented);
             fileWriter = new StreamWriter("managepdCat2.json");
            fileWriter.WriteLine(prettyString);
            fileWriter.Close();
            var profile3 = CreateManagePDCat1();
             prettyString = JsonConvert.SerializeObject(profile3, Formatting.Indented);
             fileWriter = new StreamWriter("managepdCat3.json");
            fileWriter.WriteLine(prettyString);
            fileWriter.Close();

        }


        private void Validate(string file, FuzzyCollection profile)
        {
            StreamReader str = new StreamReader(file);
            var line = "";

            while ((line = str.ReadLine()) != null)
            {

                var l = line.Split(('\t'));
                Dictionary<string, object> values = new Dictionary<string, object>();
                int count = 0;
                foreach (var v in profile.Variables)
                {

                    if (count < 2)
                        values.Add(v.Code, int.Parse(l[count]));
                    else
                        values.Add(v.Code, double.Parse(l[count]));
                    count++;
                }

                bool expectedRes = l[count] == "1";
                var res = FuzzyEngine.GetInference(profile, values);

                Assert.AreEqual(expectedRes, res.Result > 0.5);

            }

            str.Close();
            str.Dispose();
        }


        /// <summary>
        /// Using an artificial dataset validate Category 1
        /// </summary>
        [TestMethod]
        public void ValidationCat1()
        {

            FuzzyCollection profile = CreateManagePDCat1();
            string file = Path.Combine(Environment.CurrentDirectory,"TestData\\Validation\\cat1Validation1.txt");

            Validate(file, profile);


        }
        /// <summary>
        /// Using an artificial dataset validate Category 1
        /// </summary>
        [TestMethod]
        public void ValidationCat2()
        {

            FuzzyCollection profile = CreateManagePDCat2();
            string file = Path.Combine(Environment.CurrentDirectory, "TestData\\Validation\\cat1Validation2.txt");

            Validate(file, profile);


        }

        /// <summary>
        /// Using an artificial dataset validate Category 1
        /// </summary>
        [TestMethod]
        public void ValidationCat3()
        {

            FuzzyCollection profile = CreateManagePDCat3();
            string file = Path.Combine(Environment.CurrentDirectory, "TestData\\Validation\\cat1Validation3.txt");

            Validate(file, profile);


        }
        [TestMethod]
        public void TestDeSerialize()
        {

            var profile1 = File.ReadAllText("managepdCat1.json");
            var prettyString = JsonConvert.DeserializeObject(profile1);
            var profile2 = File.ReadAllText("managepdCat2.json");
            prettyString = JsonConvert.DeserializeObject(profile2);
            var profile3 = File.ReadAllText("managepdCat3.json");
            prettyString = JsonConvert.DeserializeObject(profile2);
            Assert.IsTrue((true));
        }


    }
}
