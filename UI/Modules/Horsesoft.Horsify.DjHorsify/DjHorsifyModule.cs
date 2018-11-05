using Horsesoft.Horsify.DjHorsify.Views;
using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Horsesoft.Music.Horsify.Base;

namespace Horsesoft.Horsify.DjHorsify
{
    public class DjHorsifyModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public DjHorsifyModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterTypeForNavigation<CreateFilterView>("CreateFilterView");
            _container.RegisterTypeForNavigation<EditFilterView>("EditFilterView");

            _regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(DjHorsifyView));
            _regionManager.RegisterViewWithRegion(Regions.DjHorsifyFilterScreenRegion, typeof(DjHorsifyFilterScreenView));
        }
    }
}