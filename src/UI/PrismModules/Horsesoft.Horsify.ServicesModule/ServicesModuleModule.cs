using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Horsify.Speech;
using System.Windows;
using System.Configuration;
using System.Reflection;

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
            try
            {
                //TODO: Get vlc path and options
                //true runs two Vlc instances
                InjectionConstructor ic = new InjectionConstructor("", false);

                _container.RegisterType<IHorsifyMediaController, HorsifyVlcMediaController>("", new ContainerControlledLifetimeManager(), ic);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't initialize VLC.");
                throw ex;
            }
        }

        public void Initialize()
        {
            //_container.RegisterTypeForNavigation<ViewA>();
        }
        
    }
}