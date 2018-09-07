using System.Windows;
using System.Windows.Controls.Primitives;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class HorsifySearchButton : ButtonBase
    {
        public static readonly DependencyProperty SearchTypeProperty =
            DependencyProperty.Register("SearchType", typeof(int), typeof(HorsifySearchButton), new PropertyMetadata(0));

        public static readonly DependencyProperty SearchStringProperty =
                    DependencyProperty.Register("SearchString", typeof(string), typeof(HorsifySearchButton), new PropertyMetadata(""));

        public string SearchString
        {
            get { return (string)GetValue(SearchStringProperty); }
            set { SetValue(SearchStringProperty, value); }
        }        

        public int SearchType
        {
            get { return (int)GetValue(SearchTypeProperty); }
            set { SetValue(SearchTypeProperty, value); }
        }

        static HorsifySearchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorsifySearchButton), new FrameworkPropertyMetadata(typeof(HorsifySearchButton)));
        }
    }
}
