using Newtonsoft.Json;
using PRIME.Core.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PRIME.Core.Context.Entities
{
    public class DSSModelInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }

    }

    /// <summary>
    /// CDSS Provider
    /// </summary>
    public class CDSClient:BaseEntity
    {
        public CDSClient()
        {
          //  this.Capabilities=new HashSet<CDSSCapability>();
            this.Aggregators=new HashSet<AggrModel>();
            this.DSSModels = new HashSet<DSSModel>();
        }
        /// <summary>
        /// Name of CDS Client
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of CDS Client
        /// </summary>
        public string Description { get; set; }
   
        /// <summary>
        /// CDS Client Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// CDS Client Url
        /// </summary>
        public string AuthorizationUrl { get; set; }


        /// <summary>
        /// CDS Client Url
        /// </summary>
        public string AuthenticationUrl { get; set; }

        /// <summary>
        /// CDS Client Url
        /// </summary>
        public string ResourceUrl { get; set; }
        /// <summary>
        /// CDS Client Code
        /// </summary>
        public string Code { get; set; }

        [JsonIgnore]
        public virtual ICollection<DSSModel> DSSModels { get; set; }

        [NotMapped]
        public List<DSSModelInfo> Models
        {
            get
            {
                return DSSModels.Select(e => new DSSModelInfo()
                {
                    Name=e.Name,
                    Id=e.Id,
                    Code=e.Code

                }).ToList();
            }
        }


        [JsonIgnore]
        public virtual ICollection<AggrModel> Aggregators { get; set; }

    }
}
