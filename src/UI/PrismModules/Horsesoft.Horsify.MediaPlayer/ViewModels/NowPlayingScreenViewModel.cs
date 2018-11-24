using Horsesoft.Horsify.MediaPlayer.Model;
using Horsesoft.Music.Data.Model.Horsify.Audio;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{

    public interface INowPlayingScreenViewModel
    {
        ICommand RunSearchCommand { get; set; }
    }

    public class NowPlayingScreenViewModel : MediaControlViewModelBase
    {
        private IRegionManager _regionManager;

        public ICommand RunSearchCommand { get; set; }

        public NowPlayingScreenViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ILoggerFacade loggerFacade, IHorsifyMediaController horsifyMediaController, MediaControl mediaControl) : base(loggerFacade, horsifyMediaController, eventAggregator, mediaControl)
        {
            _regionManager = regionManager;

            RunSearchCommand = new DelegateCommand<string>(OnRunSearchCommand);
        }

        private void OnRunSearchCommand(string searchType)
        {
            NavigationParameters navParams = NavigationHelper.CreateSearchFilterNavigation(MediaControlModel.SelectedSong, searchType);
            _regionManager.RequestNavigate(Regions.ContentRegion, ViewNames.SearchedSongsView, navParams);
        }

        #region Properties

        private double _totalSeconds;
        public double SongTotalSeconds
        {
            get { return _totalSeconds; }
            set { SetProperty(ref _totalSeconds, value); }
        }

        private double _songPosition;
        public double SongPosition
        {
            get { return _songPosition; }
            set { SetProperty(ref _songPosition, value); }
        }
        #endregion
    }
}
