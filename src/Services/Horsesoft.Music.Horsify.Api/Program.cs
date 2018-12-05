using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
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

            //X509Certificate2 cert = null;
            //if (cert == null)
            //{
            //    cert = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "example.pfx"), "password");
            //    Console.WriteLine($"Cert Thumb Print: {cert.Thumbprint}");
            //}

            //.UseConfiguration(config)
            var host = new WebHostBuilder()
            .UseUrls("http://*:40752")
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
