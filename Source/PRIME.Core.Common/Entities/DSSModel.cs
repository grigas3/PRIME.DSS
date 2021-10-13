using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PRIME.Core.Common.Entities;
using PRIME.Core.Web.Entities;

namespace PRIME.Core.Context.Entities
{
    /// <summary>
    /// DSS model    
    /// </summary>
    public class DSSModel:BaseEntity
    {
        #region CDS Client
        /// <summary>
        /// CDS Client Id
        /// </summary>
        public int? CDSClientId { get; set; }


        /// <summary>
        /// CDS Client
        /// </summary>
        [JsonIgnore]
        public virtual CDSClient CDSClient { get; set; }
        #endregion


        /// <summary>
        /// DSS Name
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// DSS Description
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }

        /// <summary>
        /// DSS Version
        /// </summary>
        [StringLength(10)]
        public string Version { get; set; }

        /// <summary>
        /// DSS Config in Json Format
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Data to consider in the model
        /// </summary>
        public int AggregationPeriodDays { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get;  set; }


        /// <summary>
        /// Treatment Suggestion
        /// Otherwise is a model for a condition or implication
        /// </summary>
        public bool TreatmentSuggestion { get; set; }


        public string CDSClientName
        {
            get { return CDSClient!=null? CDSClient.Name:"-"; }
        }
        

    }
}