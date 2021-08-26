using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Practice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).ConfigureAppConfiguration((hostContext, builder) =>
            {
                // Add other providers for JSON, etc.

                if (hostContext.HostingEnvironment.IsDevelopment())
                {
                    builder.AddUserSecrets<Program>();
                }
            }).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var port = Environment.GetEnvironmentVariable("PORT");
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (port != null)
                    {
                        webBuilder.UseUrls("http://*:" + port);
                    }                    
                });
        }
    }
}