using System.ServiceProcess;

namespace Horsesoft.Music.Horsify.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HorsifyService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
