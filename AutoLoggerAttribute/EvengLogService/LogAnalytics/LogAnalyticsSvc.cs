using Services.LogAnalytics.Models;
using System;
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

        public LogAnalyticsSvc(string workspaceId, string workspaceKey, string partialLogAnalyticsUrl)
        {
            _workspaceId = workspaceId;// config["LogAnalytics.workspaceId"];
            _workspaceKey = workspaceKey; // config["LogAnalytics.workspaceKey"];
            _partialLogAnalyticsUrl = partialLogAnalyticsUrl;// config["LogAnalytics.partialLogAnalyticsUrl"];
        }
        
        public async Task<HttpResponseMessage> LogEventAsync(EventLogEntry eventLogEntry, string customLogName)
        {
            // Create a hash for the API signature
            
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
            
        }
    }
}
