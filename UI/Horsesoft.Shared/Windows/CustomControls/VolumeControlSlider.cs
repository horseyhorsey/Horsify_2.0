using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    /// <summary>
    /// VLC Slider
    /// https://stackoverflow.com/questions/8989854/volume-slider-like-vlc#8990225
    /// </summary>
    public class VolumeControlSlider : Slider
    {
        public VolumeControlSlider()
        {
            this.IsMoveToPointEnabled = true;
        }

        private Track track;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            track = Template.FindName("PART_Track", this) as Track;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && track != null)
            {
                Value = track.ValueFromPoint(e.GetPosition(track));
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            ((UIElement)e.OriginalSource).CaptureMouse();
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            ((UIElement)e.OriginalSource).ReleaseMouseCapture();
        }
    }
}
