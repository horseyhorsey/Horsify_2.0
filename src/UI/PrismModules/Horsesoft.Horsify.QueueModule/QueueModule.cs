using Horsesoft.Horsify.QueueModule.Views;
using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Horsesoft.Music.Horsify.Base;

namespace Horsesoft.Horsify.QueueModule
{
    public class QueueModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public QueueModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            //_container.RegisterTypeForNavigation<QueueItem>();

            _regionManager.RegisterViewWithRegion(Regions.QueueControlRegion, typeof(QueueControlView));
            _regionManager.RegisterViewWithRegion(Regions.QueueListRegion, typeof(QueueListView));
            _regionManager.RegisterViewWithRegion(Regions.QueuePanelRegion, typeof(QuickQueueView));            
        }
    }
}