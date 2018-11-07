using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Commands;
using Prism.Logging;
using System;
using System.Linq;
using System.Windows.Input;

namespace Horsesoft.Music.Horsify.Base.ViewModels
{
    public abstract class HorsifySongPlayBindableBase : HorsifyBindableBase
    {
        protected readonly IQueuedSongDataProvider _queuedSongDataProvider;

        public DelegateCommand PlayCommand { get; set; }
        public ICommand QueueCommand { get; set; }

        public DelegateCommand<AllJoinedTable> PlaySongCommand { get; set; }
        public DelegateCommand<AllJoinedTable> QueueSongCommand { get; set; }        

        public HorsifySongPlayBindableBase(IQueuedSongDataProvider queuedSongDataProvider, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _queuedSongDataProvider = queuedSongDataProvider;

            PlayCommand         = new DelegateCommand(OnPlay);
            PlaySongCommand     = new DelegateCommand<AllJoinedTable>(OnPlay);
            QueueSongCommand    = new DelegateCommand<AllJoinedTable>(OnQueueSong);
            QueueCommand        = new DelegateCommand(OnQueue);
        }

        /// <summary>
        /// Adds the incoming song into the queue with <see cref="IQueuedSongDataProvider"/>
        /// </summary>
        /// <remarks>
        /// Checks song isn't in the queue before adding.
        /// </remarks>
        /// <param name="song">The song.</param>
        protected virtual void QueueSong(AllJoinedTable song)
        {
            if (!_queuedSongDataProvider.QueueSongs.Any(x => x == song))
            {
                _queuedSongDataProvider.QueueSongs.Add(song);
                Log($"Added song to queue.", Category.Debug);
            }            
        }

        /// <summary>
        /// Called when [queue song]. Adds the song to the <see cref="IQueuedSongDataProvider" />
        /// </summary>
        /// <param name="song">The search string.</param>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void OnQueueSong(AllJoinedTable song = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [queue].
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void OnQueue()
        {            
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [play].
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void OnPlay()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [play].
        /// </summary>
        /// <param name="song">All joined table.</param>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void OnPlay(AllJoinedTable song = null)
        {
            throw new NotImplementedException();
        }
    }
}
