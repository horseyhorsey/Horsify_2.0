using Horsesoft.Horsify.MediaPlayer.ViewModels;
using Horsesoft.Music.Horsify.Base;
using Prism.Events;
using System;
using System.Windows.Controls;

namespace Horsesoft.Horsify.MediaPlayer.Views
{
    /// <summary>
    /// Interaction logic for MediaControlView
    /// </summary>
    public partial class MediaControlView : UserControl
    {
        private IEventAggregator _eventAggregator;

        public MediaControlView(IEventAggregator eventAggregator)
        {
            InitializeComponent();            
            //this.DataContext = nowPlayingScreenViewModel;
            _eventAggregator = eventAggregator;
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //Set seek Starting
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Publish(true);
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //Publish seek position
            _eventAggregator
                .GetEvent<OnMediaSetPositionEvent<TimeSpan>>()
                .Publish(TimeSpan.FromSeconds(Slider_Arm.Value));

            //Set seek complete
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Publish(false);
        }
    }
}
