using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    /// <summary>
    /// Standard issue clock using a Dispatcher timer to update every second....
    /// </summary>
    public class ClockTextBlock : TextBlock
    {
        private DispatcherTimer _clockTimer;

        public ClockTextBlock()
        {            
            _clockTimer = new DispatcherTimer();
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Tick += _clockTimer_Tick;
            _clockTimer.Start();
        }

        private void _clockTimer_Tick(object sender, EventArgs e)
        {
            this.Text = DateTime.Now.ToShortTimeString();
        }        
    }
}
