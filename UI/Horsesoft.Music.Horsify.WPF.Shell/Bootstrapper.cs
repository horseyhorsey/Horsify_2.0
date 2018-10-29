using Horsesoft.Music.Horsify.WPF.Shell.Views;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Prism.Regions;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Horsify.ServicesModule;
using Horsesoft.Horsify.DjHorsify.Model;
using Prism.Logging;

namespace Horsesoft.Music.Horsify.WPF.Shell
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            RegisterViews();
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Registers initial views with Prisms <see cref="IRegionManager"/> instance from the Container.
        /// </summary>
        protected void RegisterViews()
        {
            //Register views with regions
            var regionManager = this.Container.Resolve<IRegionManager>();
            if (regionManager != null)
            {
            
            }
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Base.Logging.Logger();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            moduleCatalog.AddModule(typeof(ServicesModuleModule));
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.SearchModule.SearchModuleModule));
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.MediaPlayer.MediaPlayerModule));            
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.SideMenu.SideMenuModule));
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.QueueModule.QueueModule));
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.DjHorsify.DjHorsifyModule));
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.SettingsModule.SettingsModuleModule));
            moduleCatalog.AddModule(typeof(Horsesoft.Horsify.PlaylistsModule.PlaylistsModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            //Horsify Main WCF Service
            Container.RegisterInstance<Horsesoft.Horsify.ServicesModule.HorsifyService.IHorsifySongService>(
                    new Horsesoft.Horsify.ServicesModule.HorsifyService.HorsifySongServiceClient("BasicHttpBinding_IHorsifySongService"),
                    new ContainerControlledLifetimeManager());

            var _logger = Container.Resolve<ILoggerFacade>();
            var _songService = Container.Resolve<Horsesoft.Horsify.ServicesModule.HorsifyService.IHorsifySongService>();
            Container.RegisterInstance<IDjHorsifyOption>(new DjHorsifyOption(), new ContainerControlledLifetimeManager());

            //Song data provider using the IHorsifySongService
            Container.RegisterInstance<ISongDataProvider>(new SongDataProvider(_songService, _logger), 
                new ContainerControlledLifetimeManager());

            //DJ horsify Service using the IHorsifySongService
            Container.RegisterInstance<IDjHorsifyService>(new DjHorsifyService(Container.Resolve<IDjHorsifyOption>(),
                                        _songService, _logger),
                new ContainerControlledLifetimeManager());

            // Horsify Playlist services
            Container.RegisterInstance<IHorsifyPlaylistService>(new HorsifyPlaylistService(
                _songService, _logger),
                new ContainerControlledLifetimeManager());            

            //Horsify Table repo using the IHorsifySongService
            Container.RegisterInstance<IHorsifyDataTableRepo>(new HorsifyDataTableRepo(
                    _songService),
                    new ContainerControlledLifetimeManager());

            //Queued Songs
            Container.RegisterInstance<IQueuedSongDataProvider>(new QueuedSongDataProvider(),new ContainerControlledLifetimeManager());

            //Song playing info
            Container.RegisterInstance<ISongPlayingInfo> (new SongPlayingInfo(),    
                new ContainerControlledLifetimeManager());
            
            //TODO: Search History Provider - Add history to DB
            Container.RegisterInstance<ISearchHistoryProvider>(new SearchHistoryProvider(),
                new ContainerControlledLifetimeManager());

            //Media controller
            Container.RegisterInstance<IHorsifyMediaController>(new HorsifyVlcMediaController(Properties.Settings.Default.VlcPath), 
                new ContainerControlledLifetimeManager());
        }
    }
}
