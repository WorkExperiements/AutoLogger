using Microsoft.Extensions.Logging;
using Services.LogAnalytics;
using Services.LogAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubmitApp
{
    public static class LogExtensions
    {
        public static async Task LogEvent(this ILogger logger, EventLogEntry logEntry, LogAnalyticsSvc logAnalyticsSvc)
        {
            await logAnalyticsSvc.LogEventAsync(logEntry, "logName");
        }
    }
}
