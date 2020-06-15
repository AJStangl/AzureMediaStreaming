using System;
using System.Threading.Tasks;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.Context.Assets;
using AzureMediaStreaming.Context.Authorization;
using AzureMediaStreaming.Context.Authorization.Models;
using AzureMediaStreaming.Settings;
using EnsureThat;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            #region - MVC and Client Applications
            services.AddMvc(options => { options.EnableEndpointRouting = false; });
            services.AddControllersWithViews();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            #endregion

            # region - Application Services and Configuratios

            services.AddTransient<IAzureMediaServicesClient>(x => GetAzureMediaServicesClient());
            services.AddTransient<IAzureMediaMethods, AzureMediaMethods>();
            services.AddTransient<IAzureStreamingService, AzureStreamingService>();
            services.AddTransient<IAssetContext, AssetContext>();
            services.Configure<ClientSettings>(options =>
                _configuration.GetSection(nameof(ClientSettings)).Bind(options));

            #endregion

            #region - Context Pools and Database

            services.AddDbContextPool<AssetContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("AssetDatabase"));
            });

            services.AddDbContext<AuthorizationContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("AssetDatabase"));
            });

            #endregion

            # region - Authorization and JWT
            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, AuthorizationContext>();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AuthorizationContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.Configure<JwtBearerOptions>(
                IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                options =>
                {
                    Func<TokenValidatedContext, Task> onTokenValidated = options.Events.OnTokenValidated;

                    options.Events.OnTokenValidated = async context => { await onTokenValidated(context); };
                });
            #endregion

            #region - Cors and Antiforgery
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
            #endregion

            #region - Azure and Telemetry
            services.AddApplicationInsightsTelemetry();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseDeveloperExceptionPage();

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

            app.UseAuthentication();
            app.UseIdentityServer();
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