using System.Net.Http;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Startup
    {
        private const string ForgeSectionKey = "Forge";
        private const string AppBundleZipPathsKey = "AppBundleZipPaths";
        private const string DefaultProjectsSectionKey = "DefaultProjects";
        private const string InviteOnlyModeKey = "InviteOnlyMode";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllersWithViews()
                .AddJsonOptions(options =>
                                {
                                    options.JsonSerializerOptions.IgnoreNullValues = true;
                                });

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddHttpClient();

            // NOTE: eventually we might want to use `AddForgeService()`, but right now it might break existing stuff
            // https://github.com/Autodesk-Forge/forge-api-dotnet-core/blob/master/src/Autodesk.Forge.Core/ServiceCollectionExtensions.cs
            services.Configure<ForgeConfiguration>(Configuration.GetSection(ForgeSectionKey));
            services.AddSingleton<ResourceProvider>();
            services.AddSingleton<IPostProcessing, PostProcessing>();
            services.AddSingleton<IForgeOSS, ForgeOSS>();
            services.Configure<AppBundleZipPaths>(Configuration.GetSection(AppBundleZipPathsKey));
            services.AddSingleton<FdaClient>();
            services.Configure<DefaultProjectsConfiguration>(Configuration.GetSection(DefaultProjectsSectionKey));
            services.AddTransient<Initializer>();
            services.AddTransient<Arranger>();
            services.AddTransient<ProjectWork>();
            services.AddTransient<DtoGenerator>();
            services.AddSingleton<DesignAutomationClient>(provider =>
                                    {
                                        var forge = provider.GetService<IForgeOSS>();
                                        var httpMessageHandler = new ForgeHandler(Options.Create(forge.Configuration))
                                        {
                                            InnerHandler = new HttpClientHandler()
                                        };
                                        var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
                                        return new DesignAutomationClient(forgeService);
                                    });
            services.AddSingleton<Publisher>();
            services.AddScoped<UserResolver>(); // TODO: use interface
            services.AddSingleton<LocalCache>();
            services.AddSingleton<Uploads>();
            services.Configure<InviteOnlyModeConfiguration>(Configuration.GetSection(InviteOnlyModeKey));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Initializer initializer, ILogger<Startup> logger, LocalCache localCache, IOptions<ForgeConfiguration> forgeConfiguration)
        {
            if(Configuration.GetValue<bool>("clear"))
            {
                logger.LogInformation("-- Clean up --");
                // retrieve used Forge Client Id and Client Id where it is allowed to delete user buckets
                string clientIdCanDeleteUserBuckets = Configuration.GetValue<string>("clientIdCanDeleteUserBuckets");
                string clientId = forgeConfiguration.Value.ClientId;
                // only on allowed Client Id remove the user buckets
                bool deleteUserBuckets = (clientIdCanDeleteUserBuckets == clientId);
                initializer.ClearAsync(deleteUserBuckets).Wait();
            }

            if(Configuration.GetValue<bool>("initialize"))
            {
                logger.LogInformation("-- Initialization --");
                initializer.InitializeAsync().Wait();
            }

            if(Configuration.GetValue<bool>("bundles"))
            {
                logger.LogInformation("-- Initialization of AppBundles and Activities --");
                initializer.InitializeBundlesAsync().Wait();
            }

            if (env.IsDevelopment())
            {
                logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // expose local cache as static files
            localCache.Serve(app);

            app.UseSpaStaticFiles();
            app.UseMiddleware<TokenHandler>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Controllers.JobsHub>("/signalr/connection");
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
    }
}
