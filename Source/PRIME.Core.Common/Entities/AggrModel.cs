using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using PRIME.Core.Common.Entities;

namespace PRIME.Core.Context.Entities
{
    /// <summary>
    /// Aggregation Model for Creating Meta-observations
    /// </summary>
    public  class AggrModel:BaseEntity
    {
        #region CDS Client
        public int? CDSClientId { get; set; }

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
        /// Meta-Observation Code
        /// </summary>
        [StringLength(10)]
        public string Code { get; set; }

        /// <summary>
        /// DSS Version
        /// </summary>
        [StringLength(10)]
        public string Version { get; set; }

        /// <summary>
        /// Aggregation Model Config in Json Format
        /// </summary>
        public string Config { get; set; }


        public string CDSClientName
        {
            get { return CDSClient != null ? CDSClient.Name : "-"; }
        }


    }
}
