using Horsesoft.Music.Data.Model.Horsify;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Horsesoft.Horsify.SearchModule.UserControls
{
    /// <summary>
    /// Interaction logic for ucRandomSelector.xaml
    /// </summary>
    public partial class ucRandomSelector : UserControl, IInteractionRequestAware
    {
        public ucRandomSelector()
        {
            InitializeComponent();
        }

        //Todo: Close button

        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }

        private void GetRandomButton_Click(object sender, RoutedEventArgs e)
        {
            var randomOption = new RandomSelectOption(AmountControl.Value, RatingEnabled.IsChecked, (byte)RangeControl.RangeLower, (byte)RangeControl.RangeUpper);
            Notification.Content = randomOption;
            
            FinishInteraction?.Invoke();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Notification.Content = false;
            FinishInteraction?.Invoke();
        }
    }
}
