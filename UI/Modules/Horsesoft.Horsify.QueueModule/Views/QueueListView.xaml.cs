using System.Windows.Controls;
using System.Windows.Input;

namespace Horsesoft.Horsify.QueueModule.Views
{
    /// <summary>
    /// Interaction logic for QueueListView.xaml
    /// </summary>
    public partial class QueueListView : UserControl
    {
        public QueueListView()
        {
            InitializeComponent();
        }

        private void ListView_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
