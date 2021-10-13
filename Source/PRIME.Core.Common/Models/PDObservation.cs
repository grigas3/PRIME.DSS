﻿using PRIME.Core.Common.Interfaces;
using System.Collections.Generic;

namespace PRIME.Core.Models
{
    /// <summary>
    /// PD_Manager Observation
    /// This observation is used internaly in PD_Manager for fast and low footprint  data exchange.
    /// This observation has to to with symptom evaluations performed in mobile phone or other IoT devices
    /// The code is unique and the Code has all the necessairy information to convert this information
    /// to a full observation according to FHIR specificiation.
    /// </summary>
   
    public class PDObservation : BasePDObservation, IObservation
    {
        //For new Observations the id should be newid
        private string _id = "newid";
        /// <summary>
        /// Id
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Value Weight
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Observation Category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Description of variable input
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Code Namespace
        /// </summary>
        public string CodeNameSpace { get; set; }
    }

    /// <summary>
    /// Base PD Observation
    /// </summary>
    public class BasePDObservation
    {

        /// <summary>
        /// Patient Id
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public long Timestamp { get; set; }
    }


    /// <summary>
    /// Base class for Fast Aggregations allowing incremental update
    /// </summary>
    public class BaseAggPDObservation : BasePDObservation
    {
        // [JsonIgnore]
        /// <summary>
        /// E[XX]
        /// </summary>
        public double Q2 { get; set; }

        // [JsonIgnore]
        /// <summary>
        /// E[XX]
        /// </summary>
        public double Q1 { get; set; }

        // [JsonIgnore]
        /// <summary>
        /// Number of samples
        /// </summary>
        public int N { get; set; }

        /// <summary>
        /// Timestamp of first observation
        /// </summary>
        public long sTimestamp { get; set; }
    }


    /// <summary>
    /// Collection of PD Observations
    /// </summary>
    public class PDObservationCollection:List<PDObservation>
        {

        }
}