using Horsesoft.Horsify.PlaylistsModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Horsify.PlaylistsModule.ViewModels;

namespace Horsesoft.Horsify.PlaylistsModule
{
    public class PlaylistsModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public PlaylistsModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            //Register the main playlist view to the ContentRegion
            _regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(PlaylistsView));

            //Register the saved playlists with left side
            //_regionManager.AddToRegion(Regions.PlaylistsRegion, new SavedPlaylistsView());            

            //Register the tab control view model so it gets a new one each time
            //_container.RegisterType<PlaylistTabViewModel>(new TransientLifetimeManager());
        }
    }
}