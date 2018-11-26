using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Horsify.MediaPlayer.Views;
using Horsesoft.Music.Horsify.Base.Model;

namespace Horsesoft.Horsify.MediaPlayer
{
    public class MediaPlayerModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public MediaPlayerModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {

            //Create singleton MediaControl model so it can be shared
            _container.RegisterInstance(new MediaControl(), new ContainerControlledLifetimeManager());

            _regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(SongPlaying3dView));                        
            _regionManager.RegisterViewWithRegion(Regions.SongPlayingViewRightRegion, typeof(Turntable1200View));            

            //Now playing screen
            _regionManager.RegisterViewWithRegion(Regions.NowPlayingScreenRegion, typeof(NowPlayingScreenView));

            _regionManager.RegisterViewWithRegion(Regions.MediaControlRegion, typeof(MediaControlView));
            _regionManager.RegisterViewWithRegion(Regions.VolumeControlRegion, typeof(VolumeControlView));
        }
    }
}