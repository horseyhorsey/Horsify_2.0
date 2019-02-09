using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System.IO;
using System.Windows;
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

            //Let the importer create db ... TODO: init Db before importer or jukebox is run from somwhere else.
            if (!System.IO.File.Exists(@"C:\ProgramData\Horsify\Horsify.db"))
            {
                var msg = "Run the importer to initialize a database.";
                System.Windows.MessageBox.Show(msg);
                Log(msg);
                throw new FileNotFoundException(msg);
            }

            //Change Content regions view from here
            eventAggregator.GetEvent<OnNavigateViewEvent<string>>()
                .Subscribe(viewName =>
           {
               Log($"Requesting navigation > {viewName}", Category.Info, Priority.None);
               _regionManager.RequestNavigate("ContentRegion", viewName);
           });

            ///VolCtrl is either - or +
            ///Converts + to -1 and - to -2
            ChangeVolumeCommand = new DelegateCommand<string>((volCtrl) =>
            {
                double value = volCtrl == "+" ? -1 : -2;
                eventAggregator.GetEvent<OnMediaChangedVolumeEvent<double>>().Publish(value);
            });

        }
    }
}
