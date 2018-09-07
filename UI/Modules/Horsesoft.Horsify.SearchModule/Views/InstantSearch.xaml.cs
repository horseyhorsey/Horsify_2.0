using System.Windows.Controls;

namespace Horsesoft.Horsify.SearchModule.Views
{
    /// <summary>
    /// Interaction logic for InstantSearch
    /// </summary>
    public partial class InstantSearch : UserControl
    {
        public InstantSearch()
        {
            InitializeComponent();

            this.Loaded += InstantSearch_Loaded; ;
        }

        private void InstantSearch_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.SearchTextBox.Focusable)
                this.SearchTextBox.Focus();
        }
    }
}
