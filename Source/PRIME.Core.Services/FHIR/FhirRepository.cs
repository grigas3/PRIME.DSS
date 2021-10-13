using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Itenso.TimePeriod;
using Task = System.Threading.Tasks.Task;

namespace PRIME.Core.Services.FHIR
{
    /// <summary>
    /// Fhir Repository
    /// </summary>
    public class FhirRepository
    {
      


        public async Task Fetch(string id, FhirProxyConfiguration proxyConfiguration)
        {

            var headers = new Dictionary<string, string>();
            foreach (var header in proxyConfiguration.Headers)
            {
                headers.Add(header.Key, header.Value);
            }

            if (proxyConfiguration.RequiresAuthentication)
            {
                var token = await PrimeFhirClient.Authenticate(proxyConfiguration.AuthUrl,
                    proxyConfiguration.UserName,
                    proxyConfiguration.Password);

                if (token == null)
                    throw new Exception("Authentication error");
                headers.Add("Authorization", "Bearer " + token);
            }

            var bundle = await PrimeFhirClient.GetBundle(proxyConfiguration.Url, id, headers);

            Insert(bundle);


        }

        private readonly Dictionary<Tuple<string,string>,object> _values=new Dictionary<Tuple<string, string>, object>();

        List<Tuple<string, string,object>> GetPatientProperties(Patient p)
        {
            var retList=new List<Tuple<string, string, object>>();
            var t = p.BirthDateElement.ToDateTimeOffset();
            if (t.HasValue)
            {
                retList.Add(Tuple.Create("Age","PRIME", (object)((int)(new DateDiff(DateTime.Now, t.Value.Date).ElapsedYears))));
            }
            return retList;

        }

        private void Insert(Bundle bundle)
        {
            foreach (var s in bundle.Entry)
            {
                if (s.Resource == null)
                    continue;

                if ((s.Resource is Patient))
                {
                    var props=GetPatientProperties(s.Resource as Patient);
                    foreach (var p in props)
                    {

                        _values.Add(Tuple.Create(p.Item1, p.Item2), p.Item3);
                    }


                }

                else if ((s.Resource is Observation) )
                {
                    var r = false;
                    var obs = s.Resource as Observation;
                    if (obs.Value == null)
                        continue;

                    if (obs.Status.HasValue && (obs.Status == ObservationStatus.Cancelled || obs.Status == ObservationStatus.EnteredInError || obs.Status == ObservationStatus.Amended))
                    {
                        continue;

                    }
                    var e = (Hl7.Fhir.Model.Quantity)obs.Value;
                    if (!e.Value.HasValue)
                        continue;

                    _values.Add(
                        Tuple.Create(obs.Code.Coding.FirstOrDefault()?.Code, obs.Code.Coding.FirstOrDefault()?.System),
                        Decimal.ToDouble(e.Value.Value));
                        
                  



                }
                else if ((s.Resource is Condition) )
                {
                    var obs = s.Resource as Condition;

                    if (obs == null)
                        return;
                    _values.Add(
                        Tuple.Create(obs.Code.Coding.FirstOrDefault()?.Code, obs.Code.Coding.FirstOrDefault()?.System),
                        1);
                }
                
            }
        }


    }
}