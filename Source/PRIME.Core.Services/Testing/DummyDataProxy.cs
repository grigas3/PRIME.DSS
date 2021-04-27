using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRIME.Core.Common.Extensions;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Models;

namespace PRIME.Core.Services.Testing
{
    /// <summary>
    ///     Dummy Data Proxy to load data from a file
    /// </summary>
    public class DummyDataProxy : IDataProxy
    {
        private readonly List<PDObservation> _observations = new List<PDObservation>();


        /// <summary>
        ///     Void Constructor
        /// </summary>
        public DummyDataProxy()
        {
            //Init with default file
            Init("1", "symptoms.txt");
        }

        /// <summary>
        ///     Constructor with specific patient and file
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="file"></param>
        public DummyDataProxy(string patientId, string file)
        {
            Init(patientId, file);
        }


        /// <summary>
        ///     Insert T item to repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> Insert<T>(T item) where T : class
        {
            _observations.Add(item as PDObservation);
            return true;
        }

        /// <summary>
        ///     Get T items from Repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortdir"></param>
        /// <param name="lastmodified"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Get<T>(int take, int skip, string filter, string sort,
            string sortdir = "false", long lastmodified = -1) where T : class
        {
            if (typeof(T) == typeof(PDObservation))
            {
                var aggrTotal = false;
                var query = _observations.AsQueryable();
                if (!string.IsNullOrEmpty(filter))
                {
                    var filterObj = JsonConvert.DeserializeObject<Filter>(filter);

                    if (!string.IsNullOrEmpty(filterObj.patientId))
                        query = query.Where(e => e.PatientId == filterObj.patientId);

                    if (!string.IsNullOrEmpty(filterObj.codeId)) query = query.Where(e => e.CodeId == filterObj.codeId);
                    aggrTotal = filterObj.aggr == "total";
                }


                var data = query.ToList();

                //Handle Only total aggegation
                //Other types are omitted
                if (aggrTotal && data.Count > 0)
                {
                    var fdata = data.First();
                    fdata.Value = data.Select(e => e.Value).DefaultIfEmpty(0).Average();
                    return (IEnumerable<T>) new List<PDObservation> {fdata};
                }

                return (IEnumerable<T>) data;
            }

            if (typeof(T) == typeof(PDPatient))
            {
                var patientId = "";
                if (!string.IsNullOrEmpty(filter))
                {
                    var filterObj = JsonConvert.DeserializeObject<Filter>(filter);

                    if (!string.IsNullOrEmpty(filterObj.patientId))
                        patientId = filterObj.patientId;
                }


                return (IEnumerable<T>) new List<PDPatient>
                {
                    CreateDummyPatient(patientId)
                };
            }

            throw new NotSupportedException();
        }


        /// <summary>
        ///     Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string id) where T : class
        {
            if (typeof(T) == typeof(PDPatient))
                return CreateDummyPatient(id) as T;
            throw new NotSupportedException();
        }

        private void Init(string patientId, string file)
        {
            StreamReader str = null;
            try
            {
                var observations = new List<PDObservation>();
                str = new StreamReader(file);
                var line = string.Empty;

                line = str.ReadLine();
                var headers = line.Split('\t');


                while ((line = str.ReadLine()) != null)
                {
                    var vals = line.Split('\t');
                    for (var i = 1; i < vals.Length; i++)
                        _observations.Add(new PDObservation
                        {
                            Timestamp = (long) (double.Parse(vals[0]) * 1000),
                            PatientId = patientId,
                            CodeId = headers[i],
                            Value = double.Parse(vals[i])
                        });
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                str?.Close();
            }
        }

        private PDPatient CreateDummyPatient(string patientId)
        {
            return new PDPatient
            {
                Id = patientId,
                ClinicalInfo = JsonConvert.SerializeObject(new ClinicalInfoCollection
                {
                    new ClinicalInfo
                    {
                        Value = "moderate",
                        Code = "COGNITION",
                        Name = "Congition",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "moderate",
                        Code = "BIS11",
                        Name = "BIS-11",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "moderate",
                        Code = "MOOD",
                        Name = "Mood",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "moderate",
                        Code = "HALLUC",
                        Name = "hallucinations",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "moderate",
                        Code = "NMSS",
                        Name = "NMSS",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "moderate",
                        Code = "NMSS",
                        Name = "NMSS",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "normal",
                        Code = "Employment",
                        Name = "Employment",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    },
                    new ClinicalInfo
                    {
                        Value = "high",
                        Code = "activity",
                        Name = "Activity",
                        CreatedBy = "Test",
                        Timestamp = DateTime.Now.ToUnixTimestamp()
                    }
                })
            };
        }


        #region Helpers

        private class Filter
        {
            public string patientId { get; set; }
            public string codeId { get; set; }

            public string aggr { get; set; }
        }

        #endregion
    }
}