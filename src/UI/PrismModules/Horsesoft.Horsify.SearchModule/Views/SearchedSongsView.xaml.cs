using Horsesoft.Horsify.Resource.Windows.CustomControls;
using System.Windows.Controls;

namespace Horsesoft.Horsify.SearchModule.Views
{
    /// <summary>
    /// Interaction logic for JoinedSongsView.xaml
    /// </summary>
    public partial class SearchedSongsView : UserControl
    {
        public SearchedSongsView()
        {
            InitializeComponent();
        }

        private void Listview_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
