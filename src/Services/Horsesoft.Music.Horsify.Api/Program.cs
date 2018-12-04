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
            bool isService = true;
            if (args.Contains("--console"))
            {
                isService = false;
            }

            var pathToContentRoot = Directory.GetCurrentDirectory();
            //var config = new ConfigurationBuilder().SetBasePath(pathToContentRoot)
            //    .AddJsonFile("hosting.json", optional: true)
            //    .Build();

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }

            //.UseConfiguration(config)
            var host = new WebHostBuilder()            
            //.UseUrls("http://*:8089/")
            .UseKestrel()
            .UseContentRoot(pathToContentRoot) /// Route of this directory
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

            if (isService)
            {
                host.RunAsMyService(); /// Custom service
            }
            else
            {
                host.Run();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args);
        }
    }
}
