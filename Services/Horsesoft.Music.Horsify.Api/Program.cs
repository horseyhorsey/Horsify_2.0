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

            var pathToContentRoot = Directory.GetCurrentDirectory();

            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            pathToContentRoot = Path.GetDirectoryName(pathToExe);
            var host = new WebHostBuilder()
          .UseConfiguration(config)
            .UseKestrel()
            
          .UseContentRoot(pathToContentRoot) /// Route of this directory
            .UseIISIntegration()
          .UseStartup<Startup>()
          .Build();

            //host.Run();
            if (args.Contains("--debug") || args.Contains("--console"))
            {
                host.Run();
            }
            else
            {
                //host.Run();
                host.RunAsMyService();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args);
        }
    }
}
