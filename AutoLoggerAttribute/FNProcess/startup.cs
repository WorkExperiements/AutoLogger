using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(FNProcess.Startup))]

namespace FNProcess
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //var root = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") ?? $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
            //var config = new ConfigurationBuilder().SetBasePath(root)
            //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables()
            //    .Build();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            //var root = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") ?? $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
            builder.ConfigurationBuilder
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}