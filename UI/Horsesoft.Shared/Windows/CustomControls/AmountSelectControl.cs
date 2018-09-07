using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class AmountSelectControl : Control
    {
        #region Constructors
        public AmountSelectControl()
        {
            this.DefaultStyleKey = typeof(AmountSelectControl);
            this.DataContext = this;
        }

        #endregion

        #region Properties

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool ShowLargerIncrementButtons
        {
            get { return (bool)GetValue(ShowLargerIncrementButtonsProperty); }
            set { SetValue(ShowLargerIncrementButtonsProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(AmountSelectControl));

        // Using a DependencyProperty as the backing store for ShowLargerIncrementButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLargerIncrementButtonsProperty =
            DependencyProperty.Register("ShowLargerIncrementButtons", typeof(bool), typeof(AmountSelectControl), new PropertyMetadata(true));

        #endregion

        #region Overridden        
        /// <summary>
        /// Assigns the buttons in the template to handlers.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                var DecreaseBy10Button =
                    GetTemplateChild("DecreaseBy10Button") as RepeatButton;
                var DecreaseButton =
                    GetTemplateChild("DecreaseButton") as RepeatButton;
                var IncreaseButton =
                    GetTemplateChild("IncreaseButton") as RepeatButton;
                var IncreaseBy10Button =
                    GetTemplateChild("IncreaseBy10Button") as RepeatButton;

                DecreaseButton.Click += (s, e)
                    =>
                { DecreaseButton_Click(DecreaseButton, e); };
                DecreaseBy10Button.Click += (s, e)
                    =>
                { DecreaseBy10Button_Click(DecreaseBy10Button, e); };
                IncreaseButton.Click += (s, e)
                    =>
                { IncreaseButton_Click(IncreaseButton, e); };
                IncreaseBy10Button.Click += (s, e)
                    =>
                { IncreaseBy10Button_Click(IncreaseBy10Button, e); };
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion

        #region Private Methods
        private void DecreaseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Value > 0)
            {
                Value--;
            }

            e.Handled = true;
        }

        private void IncreaseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Value >= 0)
            {
                Value++;
            }

            e.Handled = true;
        }

        private void DecreaseBy10Button_Click(object sender, RoutedEventArgs e)
        {
            if (Value > 11)
            {
                Value -= 10;
            }

            e.Handled = true;
        }

        private void IncreaseBy10Button_Click(object sender, RoutedEventArgs e)
        {
            if (Value >= 0)
            {
                Value += 10;
            }

            e.Handled = true;
        } 
        #endregion
    }
}
