using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Horsesoft.Music.Horsify.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            var builder = CreateWebHostBuilder(args);
            builder.UseConfiguration(config);
            builder.UseStartup<Startup>()
                .UseIISIntegration().UseKestrel();

            builder = (WebHostBuilder)builder;
            var host = builder.Build();

            //host.Run();
            if (Debugger.IsAttached || args.Contains("--debug") && !args.Contains("--console"))
            {
                host.Run();
            }
            else
            {
                //host.Run();
                host.Run();
            }            
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args);            
        }
    }
}
