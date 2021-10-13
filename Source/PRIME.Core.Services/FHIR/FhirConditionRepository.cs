using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRIME.Core.Services.FHIR;

namespace PRIME.Core.Services.FHIR
{
    /// <summary>
    /// Fhir Condition Repositroy
    /// </summary>
    public class FhirConditionRepository : BaseFhirConditionRepository
    {
        private readonly FhirProxyConfiguration _proxyConfiguration;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="proxyConfiguration"></param>
        public FhirConditionRepository(FhirProxyConfiguration proxyConfiguration)
        {
            _proxyConfiguration = proxyConfiguration;
        }

        public override async Task Init(string id)
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in _proxyConfiguration.Headers) headers.Add(header.Key, header.Value);

            if (_proxyConfiguration.RequiresAuthentication)
            {
                var token = _proxyConfiguration.Token;
                if (string.IsNullOrEmpty(token))
                    token = await PrimeFhirClient.Authenticate(_proxyConfiguration.AuthUrl,
                        _proxyConfiguration.UserName,
                        _proxyConfiguration.Password);

                if (token == null)
                    throw new Exception("Authentication error");
                headers.Add("Authorization", "Bearer " + token);
            }

            var bundle = await PrimeFhirClient.GetBundle(_proxyConfiguration.Url, id, headers);

            SetBundle(bundle);
        }
    }
}