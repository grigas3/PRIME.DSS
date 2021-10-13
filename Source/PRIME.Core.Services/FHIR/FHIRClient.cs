#define WebClient
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;

namespace PRIME.Core.Services.FHIR
{
    public class LoginResult
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

    }


    public class FHIREndpoints
    {

        public string AuthorizationUrl { get; set; }
        public string AuthenticationUrl { get; set; }
        public string ResourceUrl { get; set; }

    }

    /// <summary>
    /// FHIR API Client
    /// </summary>
    public sealed class PrimeFhirClient
    {
        public static async Task<LoginResult> AuthenticateWithCode(string fhirAuthurl, string code)
        {
            var url = fhirAuthurl;
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            var requestBody = $"{{grant_type:'authorization_code',code:'{code}'}}";

            WebRequest request = WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json";
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
                return res;
            }
            catch (Exception ex)
            {

                Trace.WriteLine(ex.Message);
                //Log(e)
            }

            return null;

        }

        public static async Task<string> Authenticate(string fhirAuthurl, string username,string password)
        {
            var url = fhirAuthurl;

            var requestBody = $"grant_type=password&username={username}&password={password}";
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
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

        

        public static async Task<Bundle> GetPatients(string fhirurl, Dictionary<string, string> headers)
        {
            var url = fhirurl + "Patient";
            Hl7.Fhir.Model.Bundle bundle = null;

#if WebClient
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
           
            StreamReader str = null;

            try
            {
                url += "?_id=0";
                WebRequest request = WebRequest.Create(url);
                var response = await request.GetResponseAsync();
                str = new StreamReader(response.GetResponseStream());
                var jsonStr = str.ReadToEnd();
                FhirJsonParser fjp = new FhirJsonParser();
                bundle = fjp.Parse<Hl7.Fhir.Model.Bundle>(jsonStr);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);

            }
            finally
            {
                str?.Dispose();
            }

            return bundle;
#else
            FhirClient client = new FhirClient(url);

            Hl7.Fhir.Model.Bundle bundle = null;
            try
            {
                foreach (var header in headers)
                {
                    client.RequestHeaders.Add(header.Key, header.Value);
                }
                SearchParams paParams = new SearchParams("_id", "0");
                bundle = await client.SearchAsync(paParams);
                return bundle;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);

                throw ex;
            }
#endif



        }


        private const string AuthorizationExtension = "Authorization";
        private const string AuthenticationExtension = "Authentication";
        private const string ResourceExtension = "Resources";
        

        public static async Task<FHIREndpoints> GetMetadata(string fireurl)
        {
            FHIREndpoints endpoints = new FHIREndpoints();
#if WebClient
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            var url = fireurl + "api/fhir/metadata";
            StreamReader str = null;
            
            try
            {
                WebRequest request = WebRequest.Create(url);
             
             
                var response = await request.GetResponseAsync();
                str = new StreamReader(response.GetResponseStream());
                var jsonStr = str.ReadToEnd();
                FhirJsonParser fjp = new FhirJsonParser();

                var s = fjp.Parse<Hl7.Fhir.Model.CapabilityStatement>(jsonStr);

                
                if (s == null)
                    return new FHIREndpoints();


                endpoints.AuthenticationUrl =
                    s.Extension.FirstOrDefault(e => e.ElementId == AuthenticationExtension)?.Url;
                endpoints.AuthorizationUrl =
                    s.Extension.FirstOrDefault(e => e.ElementId == AuthorizationExtension)?.Url;
                endpoints.ResourceUrl = s.Extension.FirstOrDefault(e => e.ElementId == ResourceExtension)?.Url;

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);

            }

            return endpoints;


#else
            try
            {
                FhirClient client = new FhirClient(url);

                var statement = await client.GetAsync("api/fhir/metadata");



                var s = statement as CapabilityStatement;

                if (s == null)
                    return new FHIREndpoints();


                endpoints.AuthenticationUrl =
                    s.Extension.FirstOrDefault(e => e.ElementId == AuthenticationExtension)?.Url;
                endpoints.AuthorizationUrl =
                    s.Extension.FirstOrDefault(e => e.ElementId == AuthorizationExtension)?.Url;
                endpoints.ResourceUrl = s.Extension.FirstOrDefault(e => e.ElementId == ResourceExtension)?.Url;

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);

            }

            return endpoints;
#endif

        }

        public static async Task<Bundle> GetObservation(string fhirurl, string id, Dictionary<string, string> headers)
        {
            var url = fhirurl+"Observation";//.Replace("{patient}",id);
            Hl7.Fhir.Model.Bundle bundle = null;

#if WebClient

            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            StreamReader str = null;
            try
            {

                WebRequest request = WebRequest.Create(url);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var response = await request.GetResponseAsync();
                str = new StreamReader(response.GetResponseStream());
                var jsonStr = str.ReadToEnd();
                FhirJsonParser fjp = new FhirJsonParser();
                bundle = fjp.Parse<Hl7.Fhir.Model.Bundle>(jsonStr);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                //Log(e)
            }
            finally
            {
                str?.Dispose();
            }

#else

     
            FhirClient client = new FhirClient(url);

        
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
#endif
            
            return bundle;

        }

        private class MyClient : FhirClient
        {

            public MyClient(string url) : base(url)
            {
                
            }
        }

        public static async Task<Bundle> GetBundle(string fhirurl,string id,Dictionary<string,string> headers)
        {
            var url = fhirurl+"bundle";//.Replace("{patient}",id);
            Hl7.Fhir.Model.Bundle bundle = null;

#if WebClient

            ServicePointManager
                .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            StreamReader str = null;
            try
            {

                WebRequest request = WebRequest.Create(url+"?_id="+id);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var response = await request.GetResponseAsync();
                str = new StreamReader(response.GetResponseStream());
                var jsonStr = str.ReadToEnd();
                FhirJsonParser fjp = new FhirJsonParser();
                bundle = fjp.Parse<Hl7.Fhir.Model.Bundle>(jsonStr);
               
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                //Log(e)
            }
            finally
            {
                str?.Dispose();
            }

#else 
             var client = new MyClient(url);


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


#endif
            return bundle;

        }


    }
}
