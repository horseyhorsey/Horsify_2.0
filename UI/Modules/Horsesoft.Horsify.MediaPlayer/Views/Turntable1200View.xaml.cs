using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Horsesoft.Horsify.MediaPlayer.Views
{
    /// <summary>
    /// Interaction logic for TurntableView.xaml
    /// </summary>
    public partial class Turntable1200View : UserControl
    {
        BeginStoryboard beginVinylStoryboard;
        BeginStoryboard beginVinylImageStoryboard;
        bool _animInitialized = false;

        public Turntable1200View()
        {
            InitializeComponent();

            ///Grab the storyboards from the control
            beginVinylStoryboard = (BeginStoryboard)FindResource("RotateVinyl_Begin");
            beginVinylImageStoryboard = (BeginStoryboard)FindResource("RotateVinylImage_Begin");
            this.Loaded += Turntable1200View_Loaded;
        }

        /// <summary>
        /// Initializes the Storyboards if not initailized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Turntable1200View_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.TurntableEnabled.IsChecked == true)
            {
                if (!_animInitialized)
                {                    
                    beginVinylStoryboard.Storyboard.Begin();
                    beginVinylImageStoryboard.Storyboard.Begin();
                    _animInitialized = true;
                }
            }
            else
            {
                beginVinylStoryboard.Storyboard.Stop();
                beginVinylImageStoryboard.Storyboard.Stop();
            }
        }

        /// <summary>
        /// Resumes the turntable animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurntableEnabled_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            beginVinylStoryboard.Storyboard.Resume();
            beginVinylImageStoryboard.Storyboard.Resume();
        }

        /// <summary>
        /// Pauses the turntable animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurntableEnabled_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            beginVinylStoryboard.Storyboard.Pause();
            beginVinylImageStoryboard.Storyboard.Pause();
        }
    }
}
