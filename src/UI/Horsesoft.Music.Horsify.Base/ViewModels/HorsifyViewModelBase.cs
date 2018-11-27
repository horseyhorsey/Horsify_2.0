using Prism.Commands;
using Prism.Logging;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Music.Horsify.Base.ViewModels
{
    public class HorsifyViewModelBase : HorsifyBindableBase
    {
        protected IRegionManager _regionManager;

        public ICommand NavigateViewCommand { get; private set; }

        public HorsifyViewModelBase(IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _regionManager = regionManager;

            //Navigate to a view
            NavigateViewCommand =
                new DelegateCommand<string>(
                    navName => _regionManager.RequestNavigate(Regions.ContentRegion, navName));
        }
    }
}
