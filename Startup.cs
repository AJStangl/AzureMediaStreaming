using System.Threading.Tasks;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.Settings;
using EnsureThat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Azure.Management.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using IAzureClient = Microsoft.Rest.Azure.IAzureClient;

namespace AzureMediaStreaming
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            services.AddRazorPages();
            services.Configure<ClientSettings>(options => _configuration.GetSection(nameof(ClientSettings)).Bind(options));
            services.AddTransient<IAzureMediaServicesClient>(x => GetAzureMediaServicesClient());
            services.AddTransient<IAzureMediaService, AzureMediaService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        private AzureMediaServicesClient GetAzureMediaServicesClient()
        {
            var clientSettings = _configuration.GetSection(nameof(ClientSettings)).Get<ClientSettings>();
            EnsureArg.IsNotNull(clientSettings, nameof(clientSettings));

            ClientCredential clientCredential = new ClientCredential(clientSettings?.AadClientId, clientSettings?.AadSecret);
            var serviceClientCredentials = ApplicationTokenProvider.LoginSilentAsync(clientSettings?.AadTenantId,
                clientCredential, ActiveDirectoryServiceSettings.Azure).Result;
            return new AzureMediaServicesClient(clientSettings.ArmEndpoint, serviceClientCredentials)
            {
                SubscriptionId = clientSettings.SubscriptionId,
            };
        }
    }
}