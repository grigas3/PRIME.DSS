﻿using System;
using System.IO;
using System.Net;

namespace PRIME.Core.Service.Notification
{
    /// <summary>
    ///     Google Push Notification Service
    /// </summary>
    public static class GCMNotification
    {
        /// <summary>
        ///     Send a Google Push Notifcation
        /// </summary>
        /// <param name="regId">Device ID</param>
        /// <param name="patientId">Patient ID</param>
        /// <param name="message">Message</param>
        /// <param name="appID">Application ID (from Google)</param>
        /// <param name="senderID">Sender ID (from Google)</param>
        /// <returns></returns>
        public static string Notify(string regId, string patientId, string message, string appID, string senderID)
        {
            var result = "error";
            var applicationID = appID; // "AIzaSyBphWpNpMnM-yhD5lpC3T4vTXBC2pY4gRI";

            var SENDER_ID = senderID; // "netmed360test";
            var httpWebRequest = (HttpWebRequest) WebRequest.Create("https://android.googleapis.com/gcm/send");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
            httpWebRequest.Headers.Add(string.Format("Sender: key={0}", SENDER_ID));
            StreamWriter streamWriter = null;
            StreamReader streamReader = null;
            try
            {
                streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());

                var json = "{\"to\":\"" + regId + "\"," +
                           "\"data\": { \"message\" : \"" + message + "\","
                           + "\"patientId\" : \"" + patientId + "\"}" +
                           "}";
                //   Console.WriteLine(json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                streamReader = new StreamReader(httpResponse.GetResponseStream());

                result = streamReader.ReadToEnd();
                //    Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Dispose();

                if (streamWriter != null)
                    streamWriter.Dispose();
            }

            return result;
        }
    }
}