using Horsesoft.Horsify.Resource.Windows.Selectors;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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

        private string _lastSortName = "Rating";

        void SortingViewSourceHelper(string name, bool ascending = false)
        {
            var view = FindResource("SearchedSongsViewSource") as CollectionViewSource;
            var sortDescriptions = view?.SortDescriptions;

            if (sortDescriptions != null)
            {
                if (sortDescriptions.Count > 0)
                {
                    var sortDesc = sortDescriptions.FirstOrDefault(x => x.PropertyName == name);
                    if (sortDesc != null)
                    {
                        sortDescriptions.Clear();                        
                    }

                    AddToSortDescription(view, name, ascending);
                }
                else
                {
                    AddToSortDescription(view, name, ascending);
                }

                view.View.Refresh();
            }                       
        }

        void AddToSortDescription(CollectionViewSource source, string propName, bool ascending = false)
        {
            source.SortDescriptions.Add(new SortDescription() { PropertyName = propName , Direction = ascending ? ListSortDirection.Ascending : ListSortDirection.Descending});
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _lastSortName = e.Parameter as string;
            this.SortingViewSourceHelper(_lastSortName, true);
        }

        private void SortCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;            
        }

        private void RadioButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            var cnt = btn.Content.ToString();

            if (cnt == "Descending")
            {
                this.SortingViewSourceHelper(_lastSortName);
            }
            else
            {
                this.SortingViewSourceHelper(_lastSortName, true);
            }            
        }

        /// <summary>
        /// Switches the teplate of the searched songs in view. This gets the commmand parameter from the actual button sent in
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

                var view = FindResource("SearchedSongsViewSource") as CollectionViewSource;
                view?.View.Refresh();                
            }
        }
    }
}
