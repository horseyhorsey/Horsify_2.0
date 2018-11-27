using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Horsify.SideMenu.ViewModels
{
    public class NavigateControlPanelViewModel : HorsifyBindableBase
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        public DelegateCommand HelpWindowCommand { get; private set; }

        #region Commands
        public ICommand NavigateViewCommand { get; set; }
        public string Title { get; private set; }
        #endregion

        public NavigateControlPanelViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            //Navigate to a view
            NavigateViewCommand =
                new DelegateCommand<string>(
                    navName => _regionManager.RequestNavigate("ContentRegion", navName));
        }
    }
}
