using Prism.Interactivity.InteractionRequest;
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

namespace Horsesoft.Horsify.PlaylistsModule.Views
{
    /// <summary>
    /// Interaction logic for PlaylistHelpView.xaml
    /// </summary>
    public partial class PlaylistHelpView : UserControl, IInteractionRequestAware
    {
        public PlaylistHelpView()
        {
            InitializeComponent();
        }

        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FinishInteraction?.Invoke();
        }

    }
}
