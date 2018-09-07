using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Horsesoft.Horsify.MediaPlayer.Views
{
    /// <summary>
    /// Interaction logic for TurntableView.xaml
    /// </summary>
    public partial class Turntable1200View : UserControl
    {
        private IEventAggregator _eventAggregator;

        public bool IsSeekingMedia { get; private set; }
        public bool IsSongPlaying { get; private set; }

        public Turntable1200View(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;

            this.Loaded += Turntable1200View_Loaded;            

            _eventAggregator.GetEvent<OnMediaStopped>()
                .Subscribe(OnSongStoppedPlaying);

            _eventAggregator
                .GetEvent<OnMediaPlay<AllJoinedTable>>()
                .Subscribe(song =>
                {
                    IsSongPlaying = true;
                    TurntableEnabled.IsChecked = IsSongPlaying;
                }, ThreadOption.UIThread);

            _eventAggregator.GetEvent<QueuedJobsCompletedEvent>()
            .Subscribe(() =>
            {
                OnSongStoppedPlaying();

            }, ThreadOption.UIThread);

            //Messenger.Default.Register<SongStoppedPlayingMessage>(this, x => OnSongStoppedPlaying());
            //Messenger.Default.Register<PlaySongMessage>(this, x => StartTurntableAfterSongStarted());            
        }

        private void Turntable1200View_Loaded(object sender, RoutedEventArgs e)
        {
            TurntableEnabled.IsChecked = true;

            if (!IsSongPlaying)
                TurntableEnabled.IsChecked = false;

        }

        /// <summary>
        /// Starts the turntable spinning when a song is changed. This is mainly from the queue view playing a song when the turntable is disabled by the user.
        /// </summary>
        private void StartTurntableAfterSongStarted()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.TurntableEnabled.IsChecked.HasValue)
                {
                    if (!this.TurntableEnabled.IsChecked.Value)
                        TurntableEnabled.IsChecked = true;
                }
            });            
        }

        /// <summary>
        /// Stop the turntable
        /// </summary>
        private void OnSongStoppedPlaying()
        {
            this.IsSongPlaying = false;
            this.TurntableEnabled.IsChecked = false;
        }

        /// <summary>
        /// Sets the IsSeeking flag to True
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timelineSlider_SeekStarted(object sender, DragStartedEventArgs e)
        {
            IsSeekingMedia = true;
            //Set seek Starting
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Publish(true);
        }

        /// <summary>
        /// Seeks the media elements position from the timer sliders value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timelineSlider_SeekCompleted(object sender, DragCompletedEventArgs e)
        {
            IsSeekingMedia = false;

            //Publish seek position
            _eventAggregator
                .GetEvent<OnMediaSetPositionEvent<TimeSpan>>()
                .Publish(TimeSpan.FromSeconds(Slider_Arm.Value));

            //Set seek complete
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Publish(false);
        }

        /// <summary>
        /// Sets the media wheel time text from the time sliders value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        /// <summary>
        /// Gets the current value from the mixValue maxValue range.        
        /// </summary>
        /// <param name="startTime">Start of the song</param>
        /// <param name="duration"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TimeSpan ConvertRange(
                    double minAngle,
                    double maxAngle,
                    TimeSpan minValue,
                    TimeSpan maxValue,
                    double value)
        {
            var originalRange =  minAngle - maxAngle; 
            var newRange = maxValue.TotalMilliseconds - minValue.TotalMilliseconds;
            var ratio = newRange / originalRange;
            var newValue = value * ratio;
            var finalValue = newValue + minValue.TotalMilliseconds;
            return TimeSpan.FromSeconds(finalValue);
        }

        private void TurntableEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (TurntableEnabled.IsChecked.HasValue)
            {
                //Pause
                if (!TurntableEnabled.IsChecked.Value)
                {
                    //Set seek complete
                    _eventAggregator
                        .GetEvent<OnMediaPlayPauseEvent<bool>>().Publish(true);
                    IsSongPlaying = false;
                }                    
                else
                {
                    _eventAggregator
                        .GetEvent<OnMediaPlayPauseEvent<bool>>().Publish(false);
                    IsSongPlaying = true;
                }
            }
        }
    }


}
