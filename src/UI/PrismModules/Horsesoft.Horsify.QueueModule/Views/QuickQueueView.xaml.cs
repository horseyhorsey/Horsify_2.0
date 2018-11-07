using System.Windows.Controls;
using System.Windows.Input;

namespace Horsesoft.Horsify.QueueModule.Views
{
    /// <summary>
    /// Interaction logic for QuickQueueView.xaml
    /// </summary>
    public partial class QuickQueueView : UserControl
    {
        public QuickQueueView()
        {
            InitializeComponent();
        }

        private void ListView_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
