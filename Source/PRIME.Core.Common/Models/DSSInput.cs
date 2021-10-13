using System.Collections.Generic;
using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.Common.Models
{

    public class DSSVariable:ICondition
    {
        public double Value { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CodeNameSpace { get; set; }
    }

    /// <summary>
    /// PRIME DSS Input
    /// This is a helper class used to serialize DSS input from a Html Form to test the DSS functionality
    /// </summary>
    public class DSSInput
    {
        /// <summary>
        /// The DSS Model Id stored in the repository
        /// </summary>
        public string Id { get; set; }

        public string ClientId { get; set; }
        public string ModelId { get; set; }

        public string PatientId { get; set; }
        /// <summary>
        /// Json Representation of Dictionary with key and value
        /// </summary>
        public string Input { get; set; }
        
        public List<DSSVariable> Variables { get; set; }

        public string BundleJson { get; set; }
    }
}
