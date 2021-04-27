using System.Collections.Generic;

namespace PRIME.Core.Services.FHIR
{
    /// <summary>
    /// FHIR configuration
    /// </summary>
    public class FhirProxyConfiguration
    {
        /// <summary>
        /// Request Headers
        /// </summary>
        public IEnumerable<FhirHeader> Headers { get; set; }
        /// <summary>
        /// FHIR Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// FHIR authentication URL
        /// </summary>
        public string AuthUrl { get; set; }
        /// <summary>
        /// FHIR Username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// FHIR Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Requires authentication
        /// </summary>
        public bool RequiresAuthentication { get; set; }
    }
}