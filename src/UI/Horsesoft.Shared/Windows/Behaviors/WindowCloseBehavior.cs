using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Horsesoft.Horsify.Resource.Windows.Behaviors
{
    public class WindowCloseBehavior : Behavior<Window>
    {
        #region Storyboard dependency property
        public static readonly DependencyProperty StoryboardProperty =
    DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(WindowCloseBehavior), new PropertyMetadata(default(Storyboard)));

        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        } 
        #endregion

        /// <summary>
        /// Attach the OnClosing Window event
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += onWindowClosing;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onWindowClosing(object sender, CancelEventArgs e)
        {
            if (Storyboard == null)
            {
                return;
            }
            e.Cancel = true;
            AssociatedObject.Closing -= onWindowClosing;

            Storyboard.Completed += (o, a) => AssociatedObject.Close();
            Storyboard.Begin();
        }        
    }
}
