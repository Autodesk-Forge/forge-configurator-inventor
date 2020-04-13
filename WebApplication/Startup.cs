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
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Startup
    {
        private const string ForgeSectionKey = "Forge";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            // NOTE: eventually we might want to use `AddForgeService()`, but right now it might break existing stuff
            // https://github.com/Autodesk-Forge/forge-api-dotnet-core/blob/master/src/Autodesk.Forge.Core/ServiceCollectionExtensions.cs
            services.Configure<ForgeConfiguration>(Configuration.GetSection(ForgeSectionKey));
            services.AddSingleton<ResourceProvider>();
            services.AddSingleton<IForge, Forge>(); // ER: TODO: this works, but is it correct???
            services.AddSingleton<FdaClient>();
            services.AddTransient<Initializer>();
            services.AddSingleton<DesignAutomationClient>(provider =>
                                    {
                                        var forge = provider.GetService<IForge>();
                                        var httpMessageHandler = new ForgeHandler(Options.Create(forge.Configuration))
                                        {
                                            InnerHandler = new HttpClientHandler()
                                        };
                                        var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
                                        return new DesignAutomationClient(forgeService);
                                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Initializer initializer, ILogger<Startup> logger)
        {
            if(Configuration.GetValue<bool>("clear"))
            {
                logger.LogInformation("-- Clean up --");
                initializer.Clear().Wait();
            }

            if(Configuration.GetValue<bool>("initialize"))
            {
                logger.LogInformation("-- Initialization --");
                initializer.Initialize().Wait();
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
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
