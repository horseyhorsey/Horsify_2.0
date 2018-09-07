using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Importer.UI.WPF.Model;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF.ViewModels
{
    public class FileMenuViewModel : ImportViewModelBase
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private bool _isScanRunning = false;
        #endregion

        #region Constructors
        public FileMenuViewModel(IHorsifySettings horsifySettings, TagImportOption tagImportOption, IEventAggregator eventAggregator) 
            : base(horsifySettings)
        {
            _eventAggregator = eventAggregator;
            TagImportOption = tagImportOption;

            //Commands
            ScanMusicCommand = new DelegateCommand(OnScanMusic, () => !_isScanRunning);
            StopScanMusicCommand = new DelegateCommand(OnStopScanMusic, () => _isScanRunning);

            //Listen for scan completed event and update commands
            _eventAggregator.GetEvent<ScanSongsEndedEvent>().Subscribe(() =>
            {
                _isScanRunning = false;
                UpdateScanCommandsCanExecute();
            });
        } 
        #endregion

        #region Properties
        private TagImportOption _tagImportOption;
        public TagImportOption TagImportOption
        {
            get { return _tagImportOption; }
            set { SetProperty(ref _tagImportOption, value); }
        }         
        #endregion

        #region Commands
        public DelegateCommand ScanMusicCommand { get; set; }
        public DelegateCommand StopScanMusicCommand { get; set; }
        #endregion

        #region Private Methods
        private void OnScanMusic()
        {
            if (!_isScanRunning)
            {
                _eventAggregator.GetEvent<StartScanSongsEvent>().Publish();
                _isScanRunning = true;
                UpdateScanCommandsCanExecute();
            }
        }

        private void OnStopScanMusic()
        {
            if (_isScanRunning)
            {
                _eventAggregator.GetEvent<StopScanSongsEvent>().Publish();
                _isScanRunning = false;
                UpdateScanCommandsCanExecute();
            }
        }

        private void UpdateScanCommandsCanExecute()
        {
            ScanMusicCommand.RaiseCanExecuteChanged();
            StopScanMusicCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
