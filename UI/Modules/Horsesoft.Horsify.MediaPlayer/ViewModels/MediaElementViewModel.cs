using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class MediaElementViewModel : HorsifyBindableBase
    {
        private IEventAggregator _eventAggregator;        

        public MediaElementViewModel(IEventAggregator eventAggregator, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
        }

        #region Properties

        private Uri _audioSource = null;
        public Uri AudioSource
        {
            get
            {
                return _audioSource;
            }
            set
            {
                SetProperty(ref _audioSource, value);
            }
        }

        private AllJoinedTable _previousSong;
        private int? _previousSongRating;
        #endregion

    }
}
