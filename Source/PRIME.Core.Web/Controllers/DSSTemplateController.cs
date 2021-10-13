using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRIME.Core.DSS.Fuzzy;
using PRIME.Core.DSS.Treatment;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class DSSTemplateController : Controller
    {
        private static FuzzyRule CreateManagePDDefinitionBaseline()
        {
            return new FuzzyRule
            {
                Fuzzy = true,
                Name="Baseline",
                OrRules = new List<OrRule>
                {
                    new OrRule("ELIGIBLE")
                    {
                        AndVariables = new List<FuzzyFunc>
                        {
                            //new FuzzyFunc {Name = "DBS Age", Property = "DBSAGE", Operator = "LessThan", Value = 70.0},

                            new FuzzyFunc {Name = "DBS Age", Property = "DBSAGE"},
                            new FuzzyFunc
                            {
                                Name = "Diagnosed With Parkinson's", Property = "PARKINSON"
                            }
                        }
                    }
                }
            };
        }
        private static FuzzyRule CreateManagePDDefinitionNoCat1()
        {
            return new FuzzyRule()
            {
                Fuzzy = true,
                Name = "ManagePD Not applicable",
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
                Name = "ManagePD Category 1",
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
                Name = "ManagePD Category 3",
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
            return new FuzzyCollection(new List<string>
            {
                "AGE", "PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",
                "OFFTIME2", "FLUCTUATIONS2", "DYSKINESIA2", "ADL2", "NONMOTOROFF", "FALLS", "DYSTONIA", "FOG", "ICD",
                "HALLUCINATIONS"
            })
            {
                Rules = new List<FuzzyRule>
                {
                    CreateManagePDDefinitionBaseline(),

                    new FuzzyRule
                    {
                        Fuzzy = true,
                        Name = "ManagePD Category 2",
                        OrRules = new List<OrRule>
                        {
                            new OrRule("Oral Levodopa")
                            {
                                AndVariables = new List<FuzzyFunc>
                                {
                                    new FuzzyFunc {Property = "ORALLEVODOPA"},
                                    new FuzzyFunc {Property = "OFFTIME2", Not = true},
                                    new FuzzyFunc {Property = "NONMOTOROFF", Not = true},
                                    new FuzzyFunc {Property = "FLUCTUATIONS2", Not = true},
                                    new FuzzyFunc {Property = "DYSKINESIA2", Not = true},
                                    new FuzzyFunc {Property = "ADL2", Not = true},
                                    new FuzzyFunc {Property = "FALLS", Not = true},
                                    new FuzzyFunc {Property = "DYSTONIA", Not = true},
                                    new FuzzyFunc {Property = "FOG", Not = true},
                                    new FuzzyFunc {Property = "ICD", Not = true},
                                    new FuzzyFunc {Property = "HALLUCINATIONS", Not = true}
                                }
                            },
                            new OrRule("OFF Time")
                            {
                                AndVariables = new List<FuzzyFunc>
                                {
                                    new FuzzyFunc {Property = "OFFTIME"},
                                    new FuzzyFunc {Property = "OFFTIME2", Not = true},
                                    new FuzzyFunc {Property = "NONMOTOROFF", Not = true},
                                    new FuzzyFunc {Property = "FLUCTUATIONS2", Not = true},
                                    new FuzzyFunc {Property = "DYSKINESIA2", Not = true},
                                    new FuzzyFunc {Property = "ADL2", Not = true},
                                    new FuzzyFunc {Property = "FALLS", Not = true},
                                    new FuzzyFunc {Property = "DYSTONIA", Not = true},
                                    new FuzzyFunc {Property = "FOG", Not = true},
                                    new FuzzyFunc {Property = "ICD", Not = true},
                                    new FuzzyFunc {Property = "HALLUCINATIONS", Not = true}
                                }
                            },
                            new OrRule("Fluctuations")
                            {
                                AndVariables = new List<FuzzyFunc>
                                {
                                    new FuzzyFunc {Property = "FLUCTUATIONS"},
                                    new FuzzyFunc {Property = "OFFTIME2", Not = true},
                                    new FuzzyFunc {Property = "NONMOTOROFF", Not = true},
                                    new FuzzyFunc {Property = "FLUCTUATIONS2", Not = true},
                                    new FuzzyFunc {Property = "DYSKINESIA2", Not = true},
                                    new FuzzyFunc {Property = "ADL2", Not = true},
                                    new FuzzyFunc {Property = "FALLS", Not = true},
                                    new FuzzyFunc {Property = "DYSTONIA", Not = true},
                                    new FuzzyFunc {Property = "FOG", Not = true},
                                    new FuzzyFunc {Property = "ICD", Not = true},
                                    new FuzzyFunc {Property = "HALLUCINATIONS", Not = true}
                                }
                            },
                            new OrRule("DYSKINESIA")
                            {
                                AndVariables = new List<FuzzyFunc>
                                {
                                    new FuzzyFunc {Property = "DYSKINESIA"},
                                    new FuzzyFunc {Property = "OFFTIME2", Not = true},
                                    new FuzzyFunc {Property = "NONMOTOROFF", Not = true},
                                    new FuzzyFunc {Property = "FLUCTUATIONS2", Not = true},
                                    new FuzzyFunc {Property = "DYSKINESIA2", Not = true},
                                    new FuzzyFunc {Property = "ADL2", Not = true},
                                    new FuzzyFunc {Property = "FALLS", Not = true},
                                    new FuzzyFunc {Property = "DYSTONIA", Not = true},
                                    new FuzzyFunc {Property = "FOG", Not = true},
                                    new FuzzyFunc {Property = "ICD", Not = true},
                                    new FuzzyFunc {Property = "HALLUCINATIONS", Not = true}
                                }
                            },
                            new OrRule("ADL")
                            {
                                AndVariables = new List<FuzzyFunc>
                                {
                                    new FuzzyFunc {Property = "ADL"},

                                    new FuzzyFunc {Property = "OFFTIME2", Not = true},
                                    new FuzzyFunc {Property = "NONMOTOROFF", Not = true},
                                    new FuzzyFunc {Property = "FLUCTUATIONS2", Not = true},
                                    new FuzzyFunc {Property = "DYSKINESIA2", Not = true},
                                    new FuzzyFunc {Property = "ADL2", Not = true},
                                    new FuzzyFunc {Property = "FALLS", Not = true},
                                    new FuzzyFunc {Property = "DYSTONIA", Not = true},
                                    new FuzzyFunc {Property = "FOG", Not = true},
                                    new FuzzyFunc {Property = "ICD", Not = true},
                                    new FuzzyFunc {Property = "HALLUCINATIONS", Not = true}
                                }
                            }
                        }
                    }
                }
            };
        }

        private static FuzzyCollection CreateManagePDCat1()
        {
            return new FuzzyCollection(new List<string>
            {
                "AGE", "PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",
                "OFFTIME2", "FLUCTUATIONS2", "DYSKINESIA2", "ADL2", "NONMOTOROFF", "FALLS", "DYSTONIA", "FOG", "ICD",
                "HALLUCINATIONS"
            })
            {
                Rules = new List<FuzzyRule>
                {
                    CreateManagePDDefinitionBaseline(),
                    CreateManagePDDefinitionNoCat1()
                }
            };
        }


        private static FuzzyCollection CreateManagePDCat3()
        {
            return new FuzzyCollection(new List<string>
            {
                "AGE", "PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",
                "OFFTIME2", "FLUCTUATIONS2", "DYSKINESIA2", "ADL2", "NONMOTOROFF", "FALLS", "DYSTONIA", "FOG", "ICD",
                "HALLUCINATIONS"
            })
            {
                Rules = new List<FuzzyRule>
                {
                    CreateManagePDDefinitionBaseline(),
                    CreateManagePDDefinitionCat1(),
                    CreateManagePDDefinitionCat3()
                }
            };
        }


        private static FuzzyCollection CreateManagePDCatS()
        {
            return new FuzzyCollection(new List<string>
            {
                "AGE", "PARKINSON",
                "ORALLEVODOPA", "OFFTIME", "FLUCTUATIONS", "DYSKINESIA", "ADL",
              
            })
            {
                Rules = new List<FuzzyRule>
                {
                    CreateManagePDDefinitionBaseline(),
                    CreateManagePDDefinitionCat1(),
                    
                }
            };
        }

        private static TreatmentClassifier CreateClassifier(string name,string code,FuzzyCollection collection)
        {
            return new TreatmentClassifier()
            {
                Name = name,
                Code = code,
                CodeNamespace = "PRIME",
                RuleModel = collection,
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id,string name,string code)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("ID is empty");

            if (id.ToLower() == "managepd1")
            {
                return Ok(CreateClassifier(name,code,CreateManagePDCat1()));
            }
            else if (id.ToLower() == "managepds")
            {
                return Ok(CreateClassifier(name, code, CreateManagePDCatS()));
            }
            else if (id.ToLower() == "managepd2")
            {
                return Ok(CreateClassifier(name, code, CreateManagePDCat2()));
            }
            else if (id.ToLower() == "managepd3")
            {
                return Ok(CreateClassifier(name, code, CreateManagePDCat3()));
            }
            else
            {
                return Ok();
            }
        }
    }
}