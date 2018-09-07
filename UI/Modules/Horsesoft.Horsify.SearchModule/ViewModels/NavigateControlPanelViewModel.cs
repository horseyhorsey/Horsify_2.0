using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class NavigateControlPanelViewModel : HorsifyBindableBase
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        public DelegateCommand HelpWindowCommand { get; private set; }
        public InteractionRequest<INotification> ShutdownNotificationRequest { get; private set; }

        #region Commands
        public ICommand NavigateViewCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand ShutdownCommand { get; set; }
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

            #region Commands Setup
            MinimizeCommand = new DelegateCommand(OnMinimize);
            ShutdownCommand = new DelegateCommand(OnShutdown);
            #endregion

            #region Notification for help
            ShutdownNotificationRequest = new InteractionRequest<INotification>();
            #endregion

        }


        private void OnShutdown()
        {

            var canShutdown = false;
            this.ShutdownNotificationRequest.Raise(new Notification { Content = "Notification Message", Title = "Notification" }
                , r =>
                {
                    canShutdown = (bool)r.Content;
                    if (canShutdown)
                    {
                        Log($"Sending shutdown", Category.Info, Priority.None);
                        _eventAggregator.GetEvent<ShutdownEvent>().Publish();
                    }
                });

        }

        private void OnMinimize()
        {
            Log($"Sending minimize", Category.Info, Priority.None);
            _eventAggregator.GetEvent<MinimizeEvent>().Publish();
        }
    }
}
