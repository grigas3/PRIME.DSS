using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class FhirConditionRepositoryTests
    {

        private FhirProxyConfiguration CreateDummy()
        {

            return new FhirProxyConfiguration();
        }


        private Bundle CreateTestBundle1()
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
            DateTime d1 = DateTime.Now.AddDays((-5));

            for (int i = 0; i < 5; i++)
            {
                entries.Add(new Bundle.EntryComponent()
                {
                    FullUrl = "https://test.com/4323",
                    Resource = new Hl7.Fhir.Model.Observation()
                    {
                        Id = $"measurement-1212",
                        Status = ObservationStatus.Final,
                        Value = new Quantity()
                        {
                            Code = "DYSKINESIA",
                            Unit = "UPDRS",
                            System = "http://www.pdmonitorapp.com",
                            Value = 1
                        },
                        Code = new CodeableConcept("http://www.pdmonitorapp.com", "DYSKINESIA"),
                        ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>()
                        {
                            new Hl7.Fhir.Model.Observation.ReferenceRangeComponent()
                            {
                                Text = "UPDRS Range",
                                Low = new Quantity(0, "UPDRS"),
                                High = new Quantity(4, "UPDRS")
                            }
                        },
                        Performer = new List<ResourceReference>()
                        {
                            new ResourceReference("http://www.pdmonitorapp.com", "Jane Doe")
                        },
                        Issued = DateTime.UtcNow
                    },
                });

            }

         



            var bundle= new Bundle()
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


            return bundle;



        }

        [TestMethod]
        public void TestCreateBundle1()
        {
            var bundle = CreateTestBundle1();

            var s=JsonConvert.SerializeObject(bundle);
            File.WriteAllText("fhirDys.json",s);
        }


        private Bundle CreateBundle1()
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
                            Code ="OFFTIME",
                            Unit = "UPDRS",
                            System = "http://mds-updrs.org",
                            Value = 45
                        },
                        Code=new CodeableConcept("http://mds-updrs.org", "OFFTIME"),
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
                            Code = "DYSTIME",
                            Unit = "UPDRS",
                            System = "http://mds-updrs.org",
                            Value = 05
                        },
                        Code = new CodeableConcept("http://mds-updrs.org", "DYSTIME"),
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
                        ClinicalStatus = new CodeableConcept("http://terminology.hl7.org/CodeSystem/condition-clinical", "active")
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

        private Bundle CreateBundle2()
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
                        System = "http://mds-updrs.org",
                        Value = 45
                    },
                    
                    Code = new CodeableConcept("http://mds-updrs.org", "OFFTIME"),
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
                        Code = "OFFTIME",
                        Unit = "UPDRS",
                        System = "http://mds-updrs.org",
                        Value = 05
                    },
                    Code = new CodeableConcept("http://mds-updrs.org", "OFFTIME"),
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
                    Issued = DateTime.UtcNow.AddDays(-5)
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


        private Bundle CreateBundle3()
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
                        System = "http://mds-updrs.org",
                        Value = 05
                    },

                    Code = new CodeableConcept("http://mds-updrs.org", "OFFTIME"),
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
                        Code = "OFFTIME",
                        Unit = "UPDRS",
                        System = "http://mds-updrs.org",
                        Value = 45
                    },
                    Code = new CodeableConcept("http://mds-updrs.org", "OFFTIME"),
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
                    Issued = DateTime.UtcNow.AddDays(-5)
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

        [TestMethod]
        public void TestMotorSymptoms()
        {

            FhirConditionRepository rep = new FhirConditionRepository(CreateDummy());

            rep.SetBundle(CreateBundle1());

            var pd = rep.HasCondition("G20", "ICD10");

            var off = rep.HasCondition("OFFTIME", "http://mds-updrs.org", (e) =>
            {
                if (e is decimal? && (e as decimal?).HasValue)
                {
                    return (e as decimal?).Value > 15;
                }

                return false;


            });

            var dys = rep.HasCondition("DYSTIME", "http://mds-updrs.org", (e) =>
            {
                if (e is decimal? && (e as decimal?).HasValue)
                {

                    return (e as decimal?).Value > 15;

                }

                return false;


            });

            Assert.IsTrue(pd.HasValue);
            Assert.IsTrue(pd.Value);

            Assert.IsTrue(off.HasValue);
            Assert.IsTrue(off.Value);

            Assert.IsTrue(dys.HasValue);
            Assert.IsFalse(dys.Value);

        }


        [TestMethod]
        public void TestRecentOff()
        {

            FhirConditionRepository rep = new FhirConditionRepository(CreateDummy());

            rep.SetBundle(CreateBundle2());

      
            var off = rep.HasCondition("OFFTIME", "http://mds-updrs.org", (e) =>{
            if (e is decimal? && (e as decimal?).HasValue)
            {
                return (e as decimal?).Value > 15;
            }

            return false;


        });

              Assert.IsTrue(off.HasValue);
            Assert.IsTrue(off.Value);

            


        }


        [TestMethod]
        public void TestRecentNoOff()
        {

            FhirConditionRepository rep = new FhirConditionRepository(CreateDummy());

            rep.SetBundle(CreateBundle3());


            var off = rep.HasCondition("OFFTIME", "http://mds-updrs.org", (e) =>
            {
                if (e is decimal? && (e as decimal?).HasValue)
                {
                    return (e as decimal?).Value > 15;
                }

                return false;


            });

        

        Assert.IsTrue(off.HasValue);
            Assert.IsFalse(off.Value);




        }



    }
}
