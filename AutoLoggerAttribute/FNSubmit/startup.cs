using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.ServiceBus;
using System;

[assembly: FunctionsStartup(typeof(FNSubmit.Startup))]

namespace FNSubmit
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var root = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") ?? $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
            var config = new ConfigurationBuilder().SetBasePath(root)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddScoped<IServiceBusSrvc>((s) =>
            {
                return new ServiceBusSrvc(config["sb.connString"], config["sb.qName"]);
            });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            var config = builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            //builder.Services.AddHttpClient();

            //builder.Services.AddSingleton<IMyService>((s) => {
            //    return new MyService();
            //});
        }
    }
}