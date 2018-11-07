using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Music.Horsify.WPF.Shell.ViewModels
{
    public class MainWindowViewModel : HorsifyBindableBase
    {        
        private IRegionManager _regionManager;

        public ICommand ChangeVolumeCommand { get; set; }

        private string _title = "Horsify";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(ILoggerFacade loggerFacade, IEventAggregator eventAggregator, IRegionManager regionManager) : base(loggerFacade)
        {
            Log("Constructing MainWindowViewModel", Category.Debug, Priority.None);

            _regionManager = regionManager;

            //Change Content regions view from here
            eventAggregator.GetEvent<OnNavigateViewEvent<string>>()
                .Subscribe(viewName =>
           {
               Log($"Requesting navigation > {viewName}", Category.Info, Priority.None);
               _regionManager.RequestNavigate("ContentRegion", viewName);
           });

            ///VolCtrl is either - or +
            ChangeVolumeCommand = new DelegateCommand<string>((volCtrl) =>
            {
                Log("Changing volume", Category.Debug, Priority.None);
                eventAggregator.GetEvent<OnMediaChangeVolumeEvent<string>>().Publish(volCtrl);
            });

        }
    }
}
