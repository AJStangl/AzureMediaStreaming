using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.Context;
using AzureMediaStreaming.Settings;
using EnsureThat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Azure.Management.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;

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
            services.AddMvc(options => { options.EnableEndpointRouting = false; });
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            services.Configure<ClientSettings>(options =>
                _configuration.GetSection(nameof(ClientSettings)).Bind(options));
            services.AddTransient<IAzureMediaServicesClient>(x => GetAzureMediaServicesClient());
            services.AddTransient<IAzureMediaMethods, AzureMediaMethods>();
            services.AddTransient<IAzureStreamingService, AzureStreamingService>();
            services.AddTransient<IAssetContext, AssetContext>();

            services.AddAntiforgery(o =>
            {
                o.SuppressXFrameOptionsHeader = true;
                o.Cookie.SameSite = SameSiteMode.None;
                o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            services.AddCors(x =>
            {
                x.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.Build();
                });
            });
            services.AddApplicationInsightsTelemetry();

            services.AddDbContextPool<AssetContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("AssetDatabase"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseDeveloperExceptionPage();
            // app.UseExceptionHandler("/Error");

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });


            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseSpaStaticFiles();

                if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });
        }

        private AzureMediaServicesClient GetAzureMediaServicesClient()
        {
            var clientSettings = _configuration.GetSection(nameof(ClientSettings)).Get<ClientSettings>();
            EnsureArg.IsNotNull(clientSettings, nameof(clientSettings));

            var clientCredential =
                new ClientCredential(clientSettings?.AadClientId, clientSettings?.AadSecret);
            var serviceClientCredentials = ApplicationTokenProvider.LoginSilentAsync(clientSettings?.AadTenantId,
                clientCredential, ActiveDirectoryServiceSettings.Azure).Result;
            return new AzureMediaServicesClient(clientSettings.ArmEndpoint, serviceClientCredentials)
            {
                SubscriptionId = clientSettings.SubscriptionId
            };
        }
    }
}