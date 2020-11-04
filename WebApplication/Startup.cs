/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System.Text.Json.Serialization;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MigrationApp;
using Serilog;
using WebApplication.Controllers;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ServiceConfigurator.ConfigureServices(Configuration, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Initializer initializer, 
            ILogger<Startup> logger, LocalCache localCache, IOptions<ForgeConfiguration> forgeConfiguration,
            Publisher publisher)
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
                // force polling check for initializer, because callbacks
                // cannot be used at this point (no controllers are running yet)
                var oldCheckType = publisher.CompletionCheck;
                publisher.CompletionCheck = CompletionCheck.Polling;

                initializer.InitializeAsync().Wait();

                // reset configured value of completion check method
                publisher.CompletionCheck = oldCheckType;
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

            // Use Serilog middleware to log ASP.NET requests. To not pollute logs with requests about
            // static file the middleware registered after middleware for serving static files.
            app.UseSerilogRequestLogging();

            app.UseMiddleware<HeaderTokenHandler>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<JobsHub>("/signalr/connection");
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
