using System;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("console.log")
                .WriteTo.Console()
                .CreateLogger();

            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(builder =>
                {
                    builder
                        .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                        .AddForgeAlternativeEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseKestrel(options =>
                    {
                        long sizeinMB = 500;
                        long size = sizeinMB * 1024 * 1024;
                        options.Limits.MaxRequestBodySize =  size;
                    });
                    var port = Environment.GetEnvironmentVariable("PORT");
                    // In case deployed to Heroku, we need to listen on port given by Heroku, not the default one
                    if (!string.IsNullOrEmpty(port))
                    {
                        webBuilder.UseUrls("http://*:" + port);
                        Log.Logger.Information($"PORT environment variable defined to:{port}");
                    }
                });
        }
    }
}
