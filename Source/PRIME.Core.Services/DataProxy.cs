﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Common.Results;
using PRIME.Core.Models;

namespace PRIME.Core.Services
{
    /// <summary>
    ///     Data Proxy Implementation for Get/Insert data to PRIME Cloud Repository
    /// </summary>
    public class DataProxy : IDataProxy
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="credientialsProvider"></param>
        public DataProxy(IProxyCredientialsProvider credientialsProvider)
        {
            _credientialsProvider = credientialsProvider;
        }


        /// <summary>
        ///     Get Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortdir"></param>
        /// <param name="lastmodified"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Get<T>(int take, int skip, string filter, string sort,
            string sortdir = "false", long lastmodified = -1) where T : class
        {
            var uri = GetBaseUri<T>();

            //First Get Access token
            var accessToken = await GetAccessToken();
            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseAddress)
            };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // List data response.
            var response =
                await client.GetAsync(GetUrl(uri, take, skip, filter, sort,
                    sortdir)); // new StringContent(jsonRequest, Encoding.UTF8, "application/json")).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                try
                {
                    // Parse the response body. Blocking!
                    var jsonResponse = JsonConvert.DeserializeObject<IEnumerable<T>>(res);

                    return jsonResponse;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
            throw new Exception();
        }

        /// <summary>
        ///     Get A single Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string id) where T : class
        {
            var uri = GetBaseUri<T>();

            //First Get Access token
            var accessToken = await GetAccessToken();

            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseAddress)
            };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // List data response.
            var response =
                await client.GetAsync(GetUrl(uri,
                    id)); // new StringContent(jsonRequest, Encoding.UTF8, "application/json")).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                // Parse the response body. Blocking!
                var jsonResponse = JsonConvert.DeserializeObject<T>(res);

                return jsonResponse;
            }

            Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
            throw new Exception();
        }


     

        /// <summary>
        ///     This method is used to get access token from pdn
        /// </summary>
        /// <returns>Access token</returns>
        public async Task<string> GetAccessToken()
        {
            var loginUrl = BaseAddress + "/oauth/token";
            HttpClient client = null;

            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(loginUrl);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                //            client.DefaultRequestHeaders.Add("Content-Encoding", "application/x-www-form-urlencoded");

                var urlParameters = "username=" + _credientialsProvider.GetUserName() + "&password=" +
                                    _credientialsProvider.GetPassword() + "&grant_type=password";
                // List data response.
                var response = client.PostAsync(loginUrl, new StringContent(urlParameters)).Result; // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. Blocking!
                    //var res = response.Content.ReadAsAsync<LoginResult>().Result;
                    var str = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<LoginResult>(str);

                    return res.access_token;
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client?.Dispose();
            }

            return null;
        }

        private string GetUrl(string url, string id)
        {
            var str = new StringBuilder();
            str.Append(url);
            str.Append(string.Format("/{0}", id));

            return str.ToString();
        }

        /// <summary>
        ///     Get Base Uri based on type of Generic Template
        /// </summary>
        /// <typeparam name="T">Template</typeparam>
        /// <returns></returns>
        private string GetBaseUri<T>()
        {
            if (!uriDict.ContainsKey(typeof(T)))
                throw new NotSupportedException();

            return uriDict[typeof(T)];
        }

        private string GetUrl(string url, int take, int skip, string filter, string sort, string sortdir,
            long lastmodified = -1)
        {
            var str = new StringBuilder();
            str.Append(url);
            str.Append(string.Format("/find?take={0}&skip={1}&sort={2}&sortdir={3}", take, skip, sort, sortdir));
            //&sort = &sortdir = false & lastmodified = &_ = 1483996288701
            if (!string.IsNullOrEmpty(filter))
                str.Append(string.Format("&filter={0}", filter));
            else str.Append("&filter=");

            if (lastmodified > 0)
                str.Append(string.Format("&lastmodified={0}", lastmodified));
            else
                str.Append("&lastmodified=");

            return str.ToString();
        }

        #region Private declarations

        private const string BaseAddress = "https://PRIME.3dnetmedical.com";

        private readonly IProxyCredientialsProvider _credientialsProvider;

        private readonly Dictionary<Type, string> uriDict = new Dictionary<Type, string>
        {
            {typeof(PDObservation), "api/observations"},
            {typeof(PDPatient), "api/patients"}
        };

        #endregion
    }
}