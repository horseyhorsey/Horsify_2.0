using Horsesoft.Horsify.Resource.Windows.Selectors;
using Horsesoft.Horsify.SearchModule.ViewModels;
using System;
using System.Windows.Controls;

namespace Horsesoft.Horsify.SearchModule.UserControls
{
    /// <summary>
    /// Interaction logic for ucSongTemplateSwitcher.xaml
    /// </summary>
    public partial class ucSongTemplateSwitcher : UserControl
    {
        public ucSongTemplateSwitcher()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Switches the template of songs in view. This gets the commmand parameter from the actual button sent in.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SwitchTemplateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnParam = btn?.CommandParameter.ToString();
            var itemToChangeTo = (SongItem)Convert.ToInt32(btnParam);
            if (itemToChangeTo != SongItemTemplateSelector.CurrentSongItem)
            {
                SongItemTemplateSelector.CurrentSongItem = itemToChangeTo;

                //TODO: Remove the use of a ViewModel Reference?
                var vm = DataContext as SearchedSongsViewModel;
                vm?.SongsListView.Refresh();
            }
        }
    }
}
