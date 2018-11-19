using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Horsesoft.Horsify.SearchModule.UserControls
{
    /// <summary>
    /// Interaction logic for ucSortSongDialog.xaml
    /// </summary>
    public partial class ucSortSongDialog : UserControl, IInteractionRequestAware
    {
        private string _lastSortName = "Rating";
        private string _lastSortOrderName;

        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }

        public ucSortSongDialog()
        {
            InitializeComponent();
        }

        void SortingViewSourceHelper(string name, bool? ascending)
        {
            var view = Notification.Content as ListCollectionView;
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

                    AddToSortDescription(view, name, ascending.Value);
                }
                else
                {
                    AddToSortDescription(view, name, ascending.Value);
                }                
            }

            FinishInteraction?.Invoke();
        }

        void AddToSortDescription(ListCollectionView source, string propName, bool ascending = false)
        {
            source.SortDescriptions.Add(new SortDescription()
            {
                PropertyName = propName,
                Direction = ascending ? ListSortDirection.Ascending : ListSortDirection.Descending
            });
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _lastSortName = e.Parameter as string;
            var ascending = true;
            if (_lastSortOrderName == "DESCENDING")
                ascending = false;

            this.SortingViewSourceHelper(_lastSortName, ascending);
        }

        private void SortCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RadioButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            _lastSortOrderName = btn.Content.ToString();

            if (_lastSortOrderName == "DESCENDING")
            {
                this.SortingViewSourceHelper(_lastSortName, false);
            }
            else
            {
                this.SortingViewSourceHelper(_lastSortName, true);
            }
        }
    }
}
