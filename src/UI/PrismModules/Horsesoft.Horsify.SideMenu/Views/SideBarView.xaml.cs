using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Horsesoft.Horsify.SideMenu.Views
{
    /// <summary>
    /// Interaction logic for SearchButtonListView.xaml
    /// </summary>
    public partial class SideBarView : UserControl
    {
        public SideBarView()
        {
            InitializeComponent();

            var view = CollectionViewSource.GetDefaultView(SearchButtonListView1.ItemsSource);
            view.CollectionChanged += View_CollectionChanged;
        }

        private void View_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SearchButtonListView1.ScrollIntoView(SearchButtonListView1.Items.Count > 0 ? SearchButtonListView1.Items[0] : -1);
        }

        private void SearchButtonListView1_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void SearchButtonListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            //SearchButtonListView1.SelectedIndex = 0;

            //var items = SearchButtonListView1.Items;
            //if(items?.Count > 0)
            //{
            //    var item = SearchButtonListView1.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;                
            //    item?.Focus();

            //    //SearchButtonListView1.ScrollIntoView(SearchButtonListView1.Items[0]);
            //    //Keyboard.Focus(SearchButtonListView1.Items[0] as ListViewItem);
            //}
        }
    }
}
