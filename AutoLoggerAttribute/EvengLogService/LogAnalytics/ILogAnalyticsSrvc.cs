using Microsoft.AspNetCore.Http;
using Services.LogAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.LogAnalytics
{
    public interface ILogAnalyticsSrvc
    {
        Task<HttpResponseMessage> LogEventAsync(EventLogEntry eventLogEntry, string customLogName);
    }
}
