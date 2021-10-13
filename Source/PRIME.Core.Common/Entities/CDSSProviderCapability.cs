using PRIME.Core.Common.Entities;
using PRIME.Core.Web.Entities;

namespace PRIME.Core.Context.Entities
{

    /// <summary>
    /// CDSS Provider Capabilities
    /// </summary>
    public class CDSSCapability:BaseEntity
    {
        public string Capability { get; set; }
        public string Code { get; set; }

        public int CDSSProviderId { get; set; }
        public virtual  CDSClient CdsProvider { get; set; }
    }
}