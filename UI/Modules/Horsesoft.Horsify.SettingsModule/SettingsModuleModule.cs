using Horsesoft.Horsify.SettingsModule.Views;
using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;

namespace Horsesoft.Horsify.SettingsModule
{
    public class SettingsModuleModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public SettingsModuleModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(SettingsView));
        }
    }
}