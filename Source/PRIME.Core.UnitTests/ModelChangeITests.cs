using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.Aggregators;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS;
using PRIME.Core.Services.FHIR;
using Task = System.Threading.Tasks.Task;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class ModelChangeITests
    {

        private static List<AggrModel> CreateAggregators()
        {

            List<Tuple<string, string>> models = new List<Tuple<string, string>>()
            {
                Tuple.Create("OFFTIME", @".\TestData\Aggregators\offModelI.json"),
                //Tuple.Create("DYSTIME", @".\TestData\Aggregators\dyskinesia2.json"),
                //Tuple.Create("GAIT", @".\TestData\Aggregators\gait.json"),
                //Tuple.Create("BRAD", @".\TestData\Aggregators\brad.json"),
                //Tuple.Create("TREMOR", @".\TestData\Aggregators\tremor.json"),
                //Tuple.Create("COGDISORDER", @".\TestData\Aggregators\cogDisorder.json"),
                //Tuple.Create("DAYSLEEP", @".\TestData\Aggregators\daytimeSleepiness.json"),
                //Tuple.Create("COGDISORDER", @".\TestData\Aggregators\cogDisorder.json"),
                //Tuple.Create("HALLUCINATIONS", @".\TestData\Aggregators\hallucinations.json"),
                //Tuple.Create("IMPULSIVITY", @".\TestData\Aggregators\impulsivity.json"),
                //Tuple.Create("DEPRESSION", @".\TestData\Aggregators\depression.json"),
                //Tuple.Create("AGE", @".\TestData\Aggregators\age.json"),
            };
            List<AggrModel> aggr = new List<AggrModel>();
            int count = 1;
            foreach (var c in models)
            {
                aggr.Add(CreateAggregator(c.Item1,c.Item2,count++));

            }
            return aggr;
        }
        private static AggrModel CreateAggregator(string code,string file,int id)
        {
            return new AggrModel()
            {
                Id = id,
                Code = code,
                CDSClientId = 1,

                Config = File.ReadAllText(Path.Combine(Environment.CurrentDirectory,
                   file))
            };
        }


        private static DSSModel CreateModel()
        {
            return new DSSModel()
            {
                Id = 1,
                AggregationPeriodDays = 5,
                Code = "TEST",
                CDSClientId = 1,
                TreatmentSuggestion = true,
                Config = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @".\TestData\Dexi\modelyesno.json"))
            };
        }

        private static CDSClient CreateClient()
        {
            return new CDSClient()
            {
                Code = "TEST",
                Id = 1,
                Aggregators = CreateAggregators(),
                DSSModels = new List<DSSModel>()
                {
                    CreateModel()
                }
            };
        }

        private static async Task<IRepositoryService> CreateRep()
        {
            IRepositoryService rep = new DummyRepositoryService();
            var aggr = CreateAggregators();
            foreach (var a  in aggr)
            {
                await rep.InserOrUpdateAsync(a);

            }
            //Insert CDSS Client
            await rep.InserOrUpdateAsync(CreateClient());
            
       
            await rep.InserOrUpdateAsync(CreateModel());
            return rep;
        }
        private static DummyFHIRConditionRepository CreateConditions()
        {
            DummyFHIRConditionRepository repository = new DummyFHIRConditionRepository();


            repository.SetBundle(CreateBundleNormal());

            return repository;
        }

        private static DummyFHIRConditionRepository CreateConditionsNormal()
        {
            DummyFHIRConditionRepository repository = new DummyFHIRConditionRepository();


            repository.SetBundle(CreateBundleNormal());

            return repository;
        }
        private static DummyFHIRConditionRepository CreateConditionsAbNormal()
        {
            DummyFHIRConditionRepository repository = new DummyFHIRConditionRepository();


            repository.SetBundle(CreateBundleAbnormal());

            return repository;
        }

        private static DummyFHIRConditionRepository CreateConditionsMedAbNormal()
        {
            DummyFHIRConditionRepository repository = new DummyFHIRConditionRepository();


            repository.SetBundle(CreateBundleMedAbnormal());

            return repository;
        }
        private static Bundle CreateBundleAbnormal()
        {

            Hl7.Fhir.Model.Patient patient = new Patient()
            {
                BirthDateElement = new Date(1971, 12),
                Gender = AdministrativeGender.Male

            };


            List<Bundle.EntryComponent> entries = new List<Bundle.EntryComponent>()
            {
                new Bundle.EntryComponent()
                {
                    FullUrl = "https://test.com/123",
                    Resource = patient,
                }
            };



            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "OFFTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = 60
                    },
                    Code = new CodeableConcept("PRIME", "OFFTIME"),
                    ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                    {
                        new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                        {
                            Text = "low",
                            Low = new Quantity(2, "??"),
                            High = new Quantity(8, "??")
                        }
                    },
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "GAIT",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)2.5
                    },
                    Code = new CodeableConcept("PRIME", "GAIT"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });
            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "BRAD",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)2.5
                    },
                    Code = new CodeableConcept("PRIME", "BRAD"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "TREMOR",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)0.15
                    },
                    Code = new CodeableConcept("PRIME", "TREMOR"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"asasd2314234",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "employment",
                        Unit = "",
                        System = "",
                        Value = 1
                    },
                    Code = new CodeableConcept("prime", "employment"),
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"asasd2314234",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "living alone",
                        Unit = "",
                        System = "",
                        Value = 1
                    },
                    Code = new CodeableConcept("prime", "living alone"),
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "DYSTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = 60
                    },
                    Code = new CodeableConcept("PRIME", "DYSTIME"),
                    ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                    {
                        new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                        {
                            Text = "low",
                            Low = new Quantity(2, "??"),
                            High = new Quantity(8, "??")
                        }
                    },
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });
            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Condition()
                {
                    Id = $"measurement-1212",
                    Code = new CodeableConcept("ICD10", "G20"),
                    Severity = new CodeableConcept("pdmonitor", "High"),
                    ClinicalStatus = new CodeableConcept("http://terminology.hl7.org/CodeSystem/condition-clinical",
                        "active")
                },
            });

            return new Bundle()
            {
                Id = "Patient-XX-1212",
                Identifier = new Identifier()
                {
                    System = "test",
                    Value = "123"
                },
                Type = Bundle.BundleType.Document,
                Entry = entries
            };
        }

        private static Bundle CreateBundleMedAbnormal()
        {

            Hl7.Fhir.Model.Patient patient = new Patient()
            {
                BirthDateElement = new Date(1971, 12),
                Gender = AdministrativeGender.Male

            };


            List<Bundle.EntryComponent> entries = new List<Bundle.EntryComponent>()
            {
                new Bundle.EntryComponent()
                {
                    FullUrl = "https://test.com/123",
                    Resource = patient,
                }
            };



            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "OFFTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = 40
                    },
                    Code = new CodeableConcept("PRIME", "OFFTIME"),
                    ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                    {
                        new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                        {
                            Text = "low",
                            Low = new Quantity(2, "??"),
                            High = new Quantity(8, "??")
                        }
                    },
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "GAIT",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)1.5
                    },
                    Code = new CodeableConcept("PRIME", "GAIT"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });
            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "BRAD",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)2.5
                    },
                    Code = new CodeableConcept("PRIME", "BRAD"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "TREMOR",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)0.3
                    },
                    Code = new CodeableConcept("PRIME", "TREMOR"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"asasd2314234",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "employment",
                        Unit = "",
                        System = "",
                        Value = 1
                    },
                    Code = new CodeableConcept("prime", "employment"),
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"asasd2314234",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "living alone",
                        Unit = "",
                        System = "",
                        Value = 1
                    },
                    Code = new CodeableConcept("prime", "living alone"),
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "DYSTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = 60
                    },
                    Code = new CodeableConcept("PRIME", "DYSTIME"),
                    ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                    {
                        new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                        {
                            Text = "low",
                            Low = new Quantity(2, "??"),
                            High = new Quantity(8, "??")
                        }
                    },
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });
            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Condition()
                {
                    Id = $"measurement-1212",
                    Code = new CodeableConcept("ICD10", "G20"),
                    Severity = new CodeableConcept("pdmonitor", "High"),
                    ClinicalStatus = new CodeableConcept("http://terminology.hl7.org/CodeSystem/condition-clinical",
                        "active")
                },
            });

            return new Bundle()
            {
                Id = "Patient-XX-1212",
                Identifier = new Identifier()
                {
                    System = "test",
                    Value = "123"
                },
                Type = Bundle.BundleType.Document,
                Entry = entries
            };
        }

        private static Bundle CreateBundleNormal()
        {

            Hl7.Fhir.Model.Patient patient = new Patient()
            {
                BirthDateElement = new Date(1981, 12),
                Gender = AdministrativeGender.Male

            };


            List<Bundle.EntryComponent> entries = new List<Bundle.EntryComponent>()
            {
                new Bundle.EntryComponent()
                {
                    FullUrl = "https://test.com/123",
                    Resource = patient,
                }
            };


          
            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "OFFTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = 0
                    },
                    Code = new CodeableConcept("PRIME", "OFFTIME"),
                    ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                    {
                        new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                        {
                            Text = "low",
                            Low = new Quantity(2, "??"),
                            High = new Quantity(8, "??")
                        }
                    },
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "GAIT",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)1.5
                    },
                    Code = new CodeableConcept("PRIME", "GAIT"),
                  
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "DYS",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)1.5
                    },
                    Code = new CodeableConcept("PRIME", "GAIT"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "DYSTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)30
                    },
                    Code = new CodeableConcept("PRIME", "GAIT"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "BRAD",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)1.5
                    },
                    Code = new CodeableConcept("PRIME", "BRAD"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "TREMOR",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = (decimal)0.05
                    },
                    Code = new CodeableConcept("PRIME", "TREMOR"),

                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"asasd2314234",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "employment",
                        Unit = "",
                        System = "",
                        Value = 1
                    },
                    Code = new CodeableConcept("prime", "employment"),
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });

            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"asasd2314234",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "living alone",
                        Unit = "",
                        System = "",
                        Value = 1
                    },
                    Code = new CodeableConcept("prime", "living alone"),
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });


            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Observation()
                {
                    Id = $"measurement-1212",
                    Status = ObservationStatus.Final,
                    Value = new Quantity()
                    {
                        Code = "DYSTIME",
                        Unit = "UPDRS",
                        System = "PRIME",
                        Value = 0
                    },
                    Code = new CodeableConcept("PRIME", "DYSTIME"),
                    ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                    {
                        new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                        {
                            Text = "low",
                            Low = new Quantity(2, "??"),
                            High = new Quantity(8, "??")
                        }
                    },
                    Performer = new List<ResourceReference>()
                    {
                        new ResourceReference("https://test.com/practitioner", "Jane Doe")
                    },
                    Issued = DateTime.UtcNow
                },
            });
            entries.Add(new Bundle.EntryComponent()
            {
                FullUrl = "https://test.com/4323",
                Resource = new Hl7.Fhir.Model.Condition()
                {
                    Id = $"measurement-1212",
                    Code = new CodeableConcept("ICD10", "G20"),
                    Severity = new CodeableConcept("pdmonitor", "High"),
                    ClinicalStatus = new CodeableConcept("http://terminology.hl7.org/CodeSystem/condition-clinical",
                        "active")
                },
            });

            return new Bundle()
            {
                Id = "Patient-XX-1212",
                Identifier = new Identifier()
                {
                    System = "test",
                    Value = "123"
                },
                Type = Bundle.BundleType.Document,
                Entry = entries
            };
        }

        /// <summary>
        /// Test Reporisotry
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCDSChangeModelI()
        {

            var condRepository = CreateConditions();
            IRepositoryService rep = await CreateRep();
            IAggregator aggregator = new PRIMEAggregator(null);
            CDSService service = new CDSService(rep);
            var r = await CDSSMainEvaluator.EvaluateAsync(service, aggregator, rep, condRepository, "1", 1, "TEST",
                true);
            Assert.AreEqual(13, r.Item2.NumberOfVariables);

            Assert.AreEqual(1, (r.Item1 as IEnumerable<Card>).Count());
            Assert.AreEqual(16, r.Item2.VariablesEvaluated);
        }

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

    
        /// <summary>
        /// Test Reporisotry
        /// </summary>
        /// <returns></returns>
        [TestMethod] public async Task TestCDSChangeAbnormalModelI()
        {

            var condRepository = CreateConditionsAbNormal();
            IRepositoryService rep = await CreateRep();
            IAggregator aggregator = new PRIMEAggregator(null);
            CDSService service = new CDSService(rep);
            var r = await CDSSMainEvaluator.EvaluateAsync(service, aggregator, rep, condRepository, "1", 1, "TEST",
                true);
            Assert.AreEqual(13, r.Item2.NumberOfVariables);

            Assert.AreEqual(0, (r.Item1 as IEnumerable<Card>).Count());
            Assert.AreEqual(16, r.Item2.VariablesEvaluated);
        }
        /// <summary>
        /// Test Reporisotry
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCDSChangeMedAbnormalModelI()
        {

            var condRepository = CreateConditionsMedAbNormal();
            IRepositoryService rep = await CreateRep();
            IAggregator aggregator = new PRIMEAggregator(null);
            CDSService service = new CDSService(rep);
            var r = await CDSSMainEvaluator.EvaluateAsync(service, aggregator, rep, condRepository, "1", 1, "TEST",
                true);
            Assert.AreEqual(13, r.Item2.NumberOfVariables);

            Assert.AreEqual(1, (r.Item1 as IEnumerable<Card>).Count());
            Assert.AreEqual(16, r.Item2.VariablesEvaluated);
        }

       
    }
}
