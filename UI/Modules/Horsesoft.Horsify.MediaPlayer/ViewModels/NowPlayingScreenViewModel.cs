using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Horsify.Audio;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{

    public interface INowPlayingScreenViewModel
    {
        ICommand RunSearchCommand { get; set; }
    }

    public class NowPlayingScreenViewModel : BindableBase, INowPlayingScreenViewModel
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        public ICommand RunSearchCommand { get; set; }

        public NowPlayingScreenViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            RunSearchCommand = new DelegateCommand<string>(OnRunSearchCommand);

            //Update SelectedSong
            _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
            .Subscribe(song =>
            {
                //TODO: This is being fired twice!
                SelectedSong = song;                
            }, ThreadOption.UIThread);

            //When song time changes
            _eventAggregator.GetEvent<OnMediaTimeChangedEvent<SongTime>>()
            .Subscribe(songTime =>
            {
                CurrentSongTime = songTime.Duration;
                SongTotalSeconds = CurrentSongTime.TotalSeconds;

                CurrentSongPosition = songTime.CurrentSongTime;
                SongPosition = CurrentSongPosition.TotalSeconds;
            }, ThreadOption.UIThread);
        }

        private void OnRunSearchCommand(string searchType)
        {
            NavigationParameters navParams = NavigationHelper.CreateSearchFilterNavigation(SelectedSong, searchType);
            _regionManager.RequestNavigate("ContentRegion", "SearchedSongsView", navParams);
        }

        #region Properties

        private AllJoinedTable _currentSong;
        public AllJoinedTable SelectedSong
        {
            get { return _currentSong; }
            set { SetProperty(ref _currentSong, value); }
        }

        private TimeSpan _currentSongPosition;
        /// <summary>
        /// Gets or Sets the CurrentSongPosition
        /// </summary>
        public TimeSpan CurrentSongPosition
        {
            get { return _currentSongPosition; }
            set { SetProperty(ref _currentSongPosition, value); }
        }

        private TimeSpan _songTime;
        /// <summary>
        /// Gets or Sets the CurrentSongTime
        /// </summary>
        public TimeSpan CurrentSongTime
        {
            get { return _songTime; }
            set { SetProperty(ref _songTime, value); }
        }

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
