using System.Windows;
using System.Windows.Controls;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public sealed class ButtonWithLabel : Button
    {
        public ButtonWithLabel()
        {
            this.DefaultStyleKey = typeof(ButtonWithLabel);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ButtonWithLabel), new PropertyMetadata(null));

    }
}
