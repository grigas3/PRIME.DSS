using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace PRIME.Core.Services.FHIR
{
    /// <summary>
    /// 
    /// </summary>
    public class FhirPatientQuery 
    {
        #region Readonly Properties
        private readonly FhirProxyConfiguration _proxyConfiguration;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="proxyConfiguration"></param>
        public FhirPatientQuery(FhirProxyConfiguration proxyConfiguration)
        {
            _proxyConfiguration = proxyConfiguration;
        }


     
        /// <summary>
        /// Init Repository
        /// This is a Patient bundle with all patient information inside
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IEnumerable<Patient>> Get()
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in _proxyConfiguration.Headers)
            {
                headers.Add(header.Key, header.Value);
            }

            if (_proxyConfiguration.RequiresAuthentication)
            {
                string token = _proxyConfiguration.Token;

                if (string.IsNullOrEmpty(token))
                {
                    token = await PrimeFhirClient.Authenticate(_proxyConfiguration.AuthUrl,
                        _proxyConfiguration.UserName,
                        _proxyConfiguration.Password);
                }

                if (token == null)
                    throw new Exception("Authentication error");
                headers.Add("Authorization", "Bearer " + token);
            }

            List<Patient> patient=new List<Patient>();
            var bundle = await PrimeFhirClient.GetPatients(_proxyConfiguration.Url,  headers);
            var resources = bundle.GetResources();
            foreach (var r in resources)
            {
                if (r is Patient)
                {
                    patient.Add(r as Patient);
                }
                
            }

            return patient;
        }

      
    }
}