using Horsesoft.Horsify.Resource.Windows.Selectors;
using Horsesoft.Horsify.SearchModule.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Horsesoft.Horsify.SearchModule.Views
{
    /// <summary>
    /// Interaction logic for SongTemplateSwitchView
    /// </summary>
    public partial class SongTemplateSwitchView : UserControl
    {
        public SongTemplateSwitchView()
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
            var ctx = this.DataContext as SongTemplateSwitchViewModel;
            ListView songsList = GetSongList();

            var btn = sender as Button;
            var btnParam = btn?.CommandParameter.ToString();

            //Switch the listviews style horizontal / vertical
            if (btnParam == "Toggle")
            {
                ctx.IsHorizontal = !ctx.IsHorizontal;
                if (ctx.IsHorizontal)
                {
                    songsList.Style = FindResource("ListViewTouchDefaultStyle") as Style;
                    ChangeStylesTemplate(songsList, SongItem.SongItemTemplate);
                }
                else
                {
                    try
                    {
                        songsList.Style = FindResource("ListViewTouchDefaultVerticalStyle") as Style;
                    }
                    //Catch error about not applying to more than one listview (GridView)?
                    catch { }
                }

                return;
            }

            if (ctx.IsHorizontal)
            {
                var itemToChangeTo = (SongItem)Convert.ToInt32(btnParam);
                if (itemToChangeTo != SongItemTemplateSelector.CurrentSongItem)
                {
                    ChangeStylesTemplate(songsList, itemToChangeTo);
                }
            }
        }

        private void ChangeStylesTemplate(ListView listView, SongItem itemToChangeTo)
        {
            //Change item
            SongItemTemplateSelector.CurrentSongItem = itemToChangeTo;

            //Refresh list collection
            var collection = listView.DataContext as SearchedSongsViewModel;
            collection?.SongsListView.Refresh();
        }

        private ListView GetSongList()
        {
            var songsListView = TryFindParent<UserControl>(this);
            var songsList = songsListView.FindName("ListviewTouch") as ListView;
            if (songsList == null)
                throw new NullReferenceException("Cannot find songs list ListviewTouch");
            return songsList;
        }

        /// <summary>
        /// Finds a parent of a given control/item on the visual tree.
        /// </summary>
        /// <typeparam name="T">Type of Parent</typeparam>
        /// <param name="child">Child whose parent is queried</param>
        /// <returns>Returns the first parent item that matched the type (T), if no match found then it will return null</returns>
        public static T TryFindParent<T>(DependencyObject child)
        where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return TryFindParent<T>(parentObject);
        }
    }
}
