using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Newtonsoft.Json;

namespace PRIME.Core.Services.FHIR
{
    public class LoginResult
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

    }

    /// <summary>
    /// FHIR API Client
    /// </summary>
    internal sealed class PrimeFhirClient
    {


        public static async Task<string> Authenticate(string fhirAuthurl, string username,string password)
        {
            var url = fhirAuthurl;

            var requestBody = $"grant_type=password&username={username}&password={password}";

            WebRequest request = WebRequest.Create(url);

            request.Method = "POST";
            
            string resultString = null;
            try
            {
                var stream = await request.GetRequestStreamAsync();

                var bytes = Encoding.UTF8.GetBytes(requestBody);
                await stream.WriteAsync(bytes, 0, bytes.Length);
                var response = await request.GetResponseAsync();
                if (response == null)
                    return null;
                StreamReader str = new StreamReader(response.GetResponseStream());
                var jsonStr = str.ReadToEnd();
                var res = JsonConvert.DeserializeObject<LoginResult>(jsonStr);
                if (res != null)
                    resultString = res.Token;
            }
            catch (Exception ex)
            {

                Trace.WriteLine(ex.Message);
                //Log(e)
            }

            return resultString;

        }



        public static async Task<Bundle> GetBundle(string fhirurl,string id,Dictionary<string,string> headers)
        {
            var url = fhirurl;//.Replace("{patient}",id);
            WebRequest request = WebRequest.Create(url);

            FhirClient client = new FhirClient(url);
            
            Hl7.Fhir.Model.Bundle bundle = null;
            try
            {
                foreach (var header in headers)
                {
                    client.RequestHeaders.Add(header.Key, header.Value);
                }

                SearchParams paParams = new SearchParams("_id", id);
               bundle = await client.SearchAsync(paParams);


            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            //StreamReader str = null;
            //try
            //{
            //    var response = await request.GetResponseAsync();
            //    str = new StreamReader(response.GetResponseStream());
            //    var jsonStr = str.ReadToEnd();
            //    bundle = JsonConvert.DeserializeObject<Hl7.Fhir.Model.Bundle>(jsonStr);
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine(ex.Message);
            //    //Log(e)
            //}
            //finally
            //{
            //    str?.Dispose();
            //}
            return bundle;

        }


    }
}
