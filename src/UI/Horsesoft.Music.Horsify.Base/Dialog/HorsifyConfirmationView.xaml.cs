﻿using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
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

namespace Horsesoft.Music.Horsify.Base.Dialog
{
    /// <summary>
    /// Interaction logic for HorsifyConfirmationView.xaml
    /// </summary>
    public partial class HorsifyConfirmationView : UserControl, IInteractionRequestAware
    {
        public HorsifyConfirmationView()
        {
            InitializeComponent();
        }

        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }

        private void YesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((IConfirmation)Notification).Confirmed = true;
            Notification.Content = true;                        
            FinishInteraction?.Invoke();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Notification.Content = false;
            ((IConfirmation)Notification).Confirmed = false;
            FinishInteraction?.Invoke();
        }
    }
}
