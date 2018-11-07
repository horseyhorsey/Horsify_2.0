using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class VolumeControl : ProgressBar
    {
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!this.IsMouseCaptured)
                this.CaptureMouse();            
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.IsMouseCaptured)
                this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Mouse.LeftButton == MouseButtonState.Pressed && this.IsMouseCaptured)
            {
                var mousePos = e.GetPosition(this).X;
                var ratio = mousePos / this.ActualWidth;
                Value = ratio * this.Maximum;
            }
        }
    }
}
