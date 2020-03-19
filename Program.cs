using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IoConfigDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    var port = Environment.GetEnvironmentVariable("PORT");
                    // In case deployed to Heroku, we need to listen on port given by Heroku, not the default one
                    if (!string.IsNullOrEmpty(port))
                    {
                        webBuilder.UseUrls("http://*:" + port);
                    }
                })
                .ConfigureLogging(logging => 
                {
                    logging.AddConsole();
                })
                ;
    }
}
