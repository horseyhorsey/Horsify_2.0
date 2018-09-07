using System;
using System.ServiceModel;

namespace Horsesoft.Music.Horsify.SongServiceHostConsole
{
    class Program
    {
        static void Main(string[] args)
        {            
            using (var host = new ServiceHost(typeof(Horsesoft.Horsify.SongService.HorsifySongService)))
            {
                host.Opening += (s, e) => {
                    Console.WriteLine("Opening connection");
                };

                host.Closing += (s, e) => {
                    Console.WriteLine("Closing connection");
                };

                host.Faulted += (s, e) => {
                    Console.WriteLine("Faulted");
                };

                host.Open();
                Console.WriteLine($"Horsify hosting Started @ {DateTime.Now.ToString()}");
                Console.ReadLine();
            }
        }
    }
}
