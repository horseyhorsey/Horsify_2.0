using System.Windows;
using System.Windows.Controls;

namespace Horsesoft.Horsify.Resource.Windows.Selectors
{
    public class SongItemTemplateSelector : DataTemplateSelector
    {
        public static SongItem CurrentSongItem { get; set; } = SongItem.SongItemTemplate;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            switch (CurrentSongItem)
            {
                case SongItem.SongItemTemplate:
                default:
                    return element.FindResource("SongItemTemplate") as DataTemplate;
                case SongItem.JukeboxLabel:
                    return element.FindResource("JukeboxLabelTemplate") as DataTemplate;
                case SongItem.SongItemMinimal:
                    return element.FindResource("SongItemMinimalArtistTitle") as DataTemplate;
                case SongItem.SmallSongItemNoImage:
                    return element.FindResource("SmallSongItemNoImage") as DataTemplate;
                case SongItem.VerticalSlimTemplate:
                    return element.FindResource("VerticalSlimTemplate") as DataTemplate;

            }
        }        
    }

    public enum SongItem
    {
        SongItemTemplate = 0,
        JukeboxLabel = 1,
        SongItemMinimal = 2,
        SmallSongItemNoImage = 3,
        VerticalSlimTemplate = 4,
    }
}
