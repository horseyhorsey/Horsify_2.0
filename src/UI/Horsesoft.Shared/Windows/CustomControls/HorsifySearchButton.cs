using System.Windows;
using System.Windows.Controls.Primitives;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class HorsifySearchButton : ButtonBase
    {
        static HorsifySearchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorsifySearchButton), new FrameworkPropertyMetadata(typeof(HorsifySearchButton)));
        }

        #region Properties
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

        public int ButtonContentSize
        {
            get { return (int)GetValue(ButtonContentSizeProperty); }
            set { SetValue(ButtonContentSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonContentSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonContentSizeProperty =
            DependencyProperty.Register("ButtonContentSize", typeof(int), typeof(HorsifySearchButton), new PropertyMetadata(26)); 
        #endregion

    }
}
