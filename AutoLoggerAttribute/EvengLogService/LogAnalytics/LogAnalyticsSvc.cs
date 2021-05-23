using Microsoft.AspNetCore.Http;
using Services.LogAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.LogAnalytics
{
    public class LogAnalyticsSvc : ILogAnalyticsSrvc
    {
        private string _workspaceId;
        private string _workspaceKey;
        private string _partialLogAnalyticsUrl;

        public void Init(string workspaceId, string workspaceKey, string partialLogAnalyticsUrl)
        {
            _workspaceId = workspaceId;
            _workspaceKey = workspaceKey;
            _partialLogAnalyticsUrl = partialLogAnalyticsUrl;
        }
        public async Task<HttpResponseMessage> LogEventAsync(EventLogEntry eventLogEntry, string customLogName)
        {
            // Create a hash for the API signature
            var datestring = DateTime.UtcNow.ToString("r");
            var json = JsonSerializer.Serialize(eventLogEntry);

            var jsonBytes = Encoding.UTF8.GetBytes(json);
            string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
            string hashedString = BuildSignature(stringToHash, _workspaceKey);
            string signature = "SharedKey " + _workspaceId + ":" + hashedString;

            return await PostData(signature, datestring, json, customLogName);
        }

        private static string BuildSignature(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }

        private async Task<HttpResponseMessage> PostData(string signature, string date, string json, string logName)
        {
            try
            {
                string url = $"https://{_workspaceId}{_partialLogAnalyticsUrl}"; //".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Log-Type", logName);
                client.DefaultRequestHeaders.Add("Authorization", signature);
                client.DefaultRequestHeaders.Add("x-ms-date", date);

                HttpContent httpContent = new StringContent(json, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(new Uri(url), httpContent);
                return response;
                
            }
            catch (Exception)
            {
                throw;  
            }
        }
    }
}
