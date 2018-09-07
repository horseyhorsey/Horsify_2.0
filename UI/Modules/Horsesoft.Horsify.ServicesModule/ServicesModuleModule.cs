using Prism.Modularity;
using Prism.Regions;
using System;
using Microsoft.Practices.Unity;
using Prism.Unity;

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
            //container.RegisterInstance<HorsifyService.IHorsifySongService>(
            //    new HorsifyService.HorsifySongServiceClient("BasicHttpBinding_IHorsifySongService"),
            //    new ContainerControlledLifetimeManager());
        }

        public void Initialize()
        {
            //_container.RegisterTypeForNavigation<ViewA>();
        }
        
    }
}