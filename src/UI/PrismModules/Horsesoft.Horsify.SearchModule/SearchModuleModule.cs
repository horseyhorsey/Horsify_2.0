using Horsesoft.Horsify.SearchModule.Views;
using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Horsesoft.Music.Horsify.Base;

namespace Horsesoft.Horsify.SearchModule
{
    public class SearchModuleModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public SearchModuleModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterTypeForNavigation<SearchView>();
            _container.RegisterTypeForNavigation<SongSelectedView>();
            _container.RegisterTypeForNavigation<AToZSearchView>();
            _container.RegisterTypeForNavigation<InstantSearch>();

            //MAIN CONTENT
            _regionManager.RegisterViewWithRegion(
                Regions.ContentRegion, typeof(SearchedSongsView));

            _regionManager.RegisterViewWithRegion(
                Regions.SongTemplateSwitchRegion, typeof(SongTemplateSwitchView));
        }
    }
}