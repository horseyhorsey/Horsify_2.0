using System.ServiceModel;
using System.ServiceProcess;

namespace Horsesoft.Music.Horsify.Service
{
    public partial class HorsifyService : ServiceBase
    {
        internal static ServiceHost _host = null;

        public HorsifyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (_host != null)
                _host.Close();

            _host = new ServiceHost(typeof(Horsesoft.Horsify.SongService.HorsifySongService));
            _host.Open();
        }

        protected override void OnStop()
        {
            if (_host != null)
            {
                _host.Close();
                _host = null;
            }                
        }
    }
}
