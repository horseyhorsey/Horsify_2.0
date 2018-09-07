using Horsesoft.Music.Data.Model.Horsify.Audio;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Horsesoft.Horsify.MediaPlayer.Views
{
    /// <summary>
    /// Interaction logic for MediaElementView.xaml
    /// </summary>
    public partial class MediaElementView : UserControl, IMediaPlayer
    {
        //static IHorsifyLogger logger = new HorsfiyLogger();

        private bool IsSeekingMedia;
        private bool isPlaying;
        private DispatcherTimer timer = new DispatcherTimer();
        private IEventAggregator _eventAggregator;

        public MediaElementView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            //Media element settings
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.Volume = 1.0;
            mediaElement.Stop();
            mediaElement.MediaFailed += MediaElement_MediaFailed;

            //Sub/Pub to volume changed events
            _eventAggregator
                .GetEvent<OnMediaChangeVolumeEvent<string>>().Subscribe(x =>
            {
                
                //Turn slider Volume up and down
                if (x == "-")
                {
                    if (mediaElement.Volume > 0)
                    {
                        mediaElement.Volume -= 0.1;
                    }
                }
                else if (x == "+")
                {
                    if (mediaElement.Volume < 1.0)
                        mediaElement.Volume += 0.1;
                }

                _eventAggregator
                .GetEvent<OnMediaChangedVolumeEvent<double>>().Publish(mediaElement.Volume);
            });

            //Sub/Pub to volume changed events
            _eventAggregator
                .GetEvent<SetVolumeEvent>().Subscribe(x =>
                {
                    mediaElement.Volume = x;
                    //_eventAggregator
                    //.GetEvent<OnMediaChangedVolumeEvent<double>>().Publish(mediaElement.Volume);
                });

            //Sets the media position from an event.
            _eventAggregator
                .GetEvent<OnMediaSetPositionEvent<TimeSpan>>().Subscribe(x =>
                {
                    mediaElement.Position = x;
                });

            //Sets the IsSeeking variable
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Subscribe(x =>
              {
                  IsSeekingMedia = x;
              });

            //Sets position of the track.
            _eventAggregator
                .GetEvent<OnMediaSetPositionEvent<TimeSpan>>().Subscribe(x => mediaElement.Position = x);

            _eventAggregator
                        .GetEvent<OnMediaPlayPauseEvent<bool>>().Subscribe(OnPlayPause);
        }

        private void OnPlayPause(bool obj)
        {
            if (obj)
            {
                if (mediaElement.CanPause)
                    mediaElement.Pause();
            }
            else
            {
                mediaElement.Play();
            }
        }

        public SongTime SongTime { get; private set; } = new SongTime();

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //logger.LogError("MediaElement: Failed to play media");
            //logger.LogException(e.ErrorException);            

            _eventAggregator
                .GetEvent<OnAdvanceQueue>().Publish();
        }

        private void ChangeMediaVolume(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            SetSeekBarToZero();

            try
            {
                //timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                isPlaying = false;
                OnIMediaPlayerPlay();
            }
            catch (Exception)
            {

            }
        }

        private void SetSeekBarToZero()
        {
            var ts = new TimeSpan(0, 0, 0, 0);
            //media_wheelTime.Text = ts.ToString();
            //timelineSlider.Value = 0;
        }

        #region Timer
        private void StartTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        private void StopTimer()
        {
            timer.Tick -= timer_Tick;
            timer.Stop();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mediaElement.Source != null) &&
                (mediaElement.NaturalDuration.HasTimeSpan) &&
                (!IsSeekingMedia))
            {
                //timelineSlider.Minimum = 0;
                //timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                //timelineSlider.Value = mediaElement.Position.TotalSeconds;

                //SignalR
                //MainViewModel._hubPublisher.OnMediaElementTimerTick(mediaElement.Position.TotalMinutes);

                //TODO - Publish the current songs playing time
                //Messenger.Default.Send(new SongTimeChangedMessage(mediaElement.Position, mediaElement.NaturalDuration.TimeSpan));

                SongTime.CurrentSongTime = mediaElement.Position;
                SongTime.Duration = mediaElement.NaturalDuration.TimeSpan;
                _eventAggregator
                    .GetEvent<OnMediaTimeChangedEvent<SongTime>>().Publish
                    (SongTime);                
                return;
            }

            if (!mediaElement.NaturalDuration.HasTimeSpan)
            {                
                this.StopTimer();
                this.isPlaying = false;

                //TODO: Play next song in queue
                //Messenger.Default.Send(new SongPlayerErrorMessage(null, $"PlayerBehind: File has no duration: {mediaElement.Source.AbsolutePath}"));
                //Messenger.Default.Send(new PlayQueuedSongMessage());
            }
        }
        #endregion

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            SetSeekBarToZero();
            StopTimer();
            mediaElement.Stop();
            isPlaying = false;
            mediaElement.Position = TimeSpan.Zero;

            _eventAggregator
                .GetEvent<SkipQueueEvent>().Publish();
        }

        void IMediaPlayer.Pause()
        {
            mediaElement.Pause();
            isPlaying = false;
            StopTimer();
        }

        void IMediaPlayer.Play()
        {
            OnIMediaPlayerPlay();         
        }

        private void OnIMediaPlayerPlay()
        {
            if (!isPlaying)
            {
                StartTimer();

                //Skip to 1 minute
                //if (SkipTo1Minute.IsChecked.Value)
                //    mediaElement.Position = TimeSpan.FromMinutes(1.1);

                mediaElement.Play();
                isPlaying = true;
            }
        }

        private void OnIMediaPlayerStop()
        {
            mediaElement.Stop();
            isPlaying = false;
            StopTimer();
        }

        void IMediaPlayer.SetPosition(int framePos)
        {
            throw new NotImplementedException();
        }

        void IMediaPlayer.Stop()
        {
            OnIMediaPlayerStop();
        }

        TimeSpan IMediaPlayer.GetCurrentTime()
        {
            return mediaElement.Position;
        }

        TimeSpan IMediaPlayer.GetCurrentPosition()
        {
            return mediaElement.Position;
        }
    }
}
