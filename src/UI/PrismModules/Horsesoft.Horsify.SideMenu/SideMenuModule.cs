using Prism.Modularity;
using Prism.Regions;
using Microsoft.Practices.Unity;
using Horsesoft.Horsify.SideMenu.Views;
using Horsesoft.Music.Horsify.Base;

namespace Horsesoft.Horsify.SideMenu
{
    public class SideMenuModule : IModule
    {
        private IRegionManager _regionManager;
        //private IUnityContainer _container;

        public SideMenuModule(IUnityContainer container, IRegionManager regionManager)
        {
            //_container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            //Register SideBar View with Region
            _regionManager.RegisterViewWithRegion(
                Regions.SidePanelRegion, typeof(SideBarView));

            _regionManager.RegisterViewWithRegion(
                Regions.NavigateViewsRegion, typeof(NavigateControlPanelView));
        }
    }
}