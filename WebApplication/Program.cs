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

using System;
using System.IO;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder host = Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder
                        .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
                        .AddForgeAlternativeEnvironmentVariables();
                })
                .UseSerilog((context, logConfig) =>
                {
                    logConfig
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.File("console.log")
                        .WriteTo.Console();
                });

            var cmdLine = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            if (cmdLine.GetValue<bool>("migration") == true)
            {
                host.ConfigureServices((hostContext, services) =>
                {
                    ServiceConfigurator.ConfigureServices(hostContext.Configuration, services);
                });
            }
            else
            {
                host.ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>().UseKestrel(options =>
                        {
                            long sizeinMB = 500;
                            long size = sizeinMB * 1024 * 1024;
                            options.Limits.MaxRequestBodySize =  size;
                        });
                        var port = Environment.GetEnvironmentVariable("PORT");
                        // If deployed to a service like Heroku, need to listen on port defined in the environment, not the default one
                        if (!string.IsNullOrEmpty(port))
                        {
                            webBuilder.UseUrls("http://*:" + port);
                            Log.Logger.Information($"PORT environment variable defined to:{port}");
                        }
                    });
            }

            return host;
        }
    }
}