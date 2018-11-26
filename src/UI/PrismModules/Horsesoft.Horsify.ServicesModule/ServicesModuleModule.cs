using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Horsesoft.Music.Engine;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Data.Model.Horsify;

namespace Horsesoft.Horsify.ServicesModule
{
    public class ServicesModuleModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public ServicesModuleModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            RegisterServices(_container);
        }

        private void RegisterServices(IUnityContainer container)
        {            
            _container.RegisterType<ISongDataProvider, SongDataProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IQueuedSongDataProvider, QueuedSongDataProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IPlaylistService, HorsifyPlaylistService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDjHorsifyService, DjHorsifyService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISongPlayingInfo, SongPlayingInfo>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IVoiceControl, VoiceControl>(new ContainerControlledLifetimeManager());
        }

        public void Initialize()
        {
            //_container.RegisterTypeForNavigation<ViewA>();
        }
        
    }
}