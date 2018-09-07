using Horsesoft.Music.Horsify.Importer.UI.WPF.Views;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Engine.Import;
using System;
using Prism.Regions;
using Horsesoft.Music.Horsify.Importer.UI.WPF.Model;
using Prism.Logging;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            RegisterViews();
            Application.Current.MainWindow.Show();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Base.Logging.Logger("HorsifyImporter");
        }

        private void RegisterViews()
        {
            var regionManager = this.Container.Resolve<IRegionManager>();
            if (regionManager != null)
            {
                //Register SideBar View with Region
                regionManager.RegisterViewWithRegion(
                    "FileMenuRegion", typeof(FileMenuView));

                //Register SideBar View with Region
                regionManager.RegisterViewWithRegion(
                    "FileImportRegion", typeof(FileImportView));
            }

        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            //Importer Settings
            Container.RegisterInstance<IHorsifySettings>(new ImportSettings(),new ContainerControlledLifetimeManager());
            var settings = Container.Resolve<IHorsifySettings>();

            Container.RegisterInstance<IFileImport>(new FileImport(), new ContainerControlledLifetimeManager());            
            Container.RegisterInstance<ITagImport>(new TagImport(settings), new ContainerControlledLifetimeManager());
            Container.RegisterInstance<IHorsifyDbConnection>(new DbConnection(), new ContainerControlledLifetimeManager());
            Container.RegisterInstance(new TagImportOption(), new ContainerControlledLifetimeManager());

        }
    }
}
