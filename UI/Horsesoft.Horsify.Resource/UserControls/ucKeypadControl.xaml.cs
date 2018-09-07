using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Horsesoft.Horsify.Resource.UserControls
{
    /// <summary>
    /// Interaction logic for ucKeypadControl.xaml
    /// </summary>
    public partial class ucKeypadControl : UserControl
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!*$'," + "\"";
        public IEnumerable<char> FilterChars { get; set; }        

        public ucKeypadControl()
        {            
            InitializeComponent();
            
            FilterChars = chars.ToCharArray();            

            this.DataContext = this;
        }

        public char? SelectedChar
        {
            get { return (char)GetValue(SelectedCharProperty); }
            set { SetValue(SelectedCharProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedChar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCharProperty =
            DependencyProperty.Register("SelectedChar", typeof(char?), 
                typeof(ucKeypadControl), 
                new PropertyMetadata(null));


        public double KeyWidth
        {
            get { return (double)GetValue(KeyWidthProperty); }
            set { SetValue(KeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyWidthProperty =
            DependencyProperty.Register("KeyWidth", typeof(double), typeof(ucKeypadControl), new PropertyMetadata(35.0));


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.Count > 0)
            {
                SelectedChar = (char)e.AddedItems[0];
            }
        }
    }
}
