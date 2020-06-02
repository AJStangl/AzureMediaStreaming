using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, builder) =>
                    {
                        var environment = context.HostingEnvironment;
                        builder
                            .SetBasePath(environment.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true)
                            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                            .AddEnvironmentVariables();
                    });

                    webBuilder.ConfigureLogging((context, builder) =>
                    {
                        var config = context.Configuration;
                        builder.AddAzureWebAppDiagnostics();
                        builder.AddApplicationInsights(config["ApplicationInsights:InstrumentationKey"]);
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}