using Horsesoft.Music.Horsify.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;

namespace Horsesoft.Music.Horsify.WPF.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEventAggregator _eventAggregator;

        public MainWindow(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();

            _eventAggregator.GetEvent<MinimizeEvent>().Subscribe(OnMinimizeExecuted, ThreadOption.UIThread);
            _eventAggregator.GetEvent<ShutdownEvent>().Subscribe(OnCloseExecuted, ThreadOption.UIThread);

        }


        private void OnMinimizeExecuted()
        {
            this.WindowState = WindowState.Minimized;
        }

        //private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        //        this.DragMove();
        //}

        private void _mainWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState != WindowState.Maximized)
                {
                    this.ResizeMode = ResizeMode.NoResize;
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.ResizeMode = ResizeMode.CanResize;
                    this.WindowState = WindowState.Normal;
                }
            }
        }

        private void Close_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OnCloseExecuted();
        }

        private void OnCloseExecuted()
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void _mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Messenger.Default.Send(new WindowSizeChangedMessage(e.NewSize));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SwitchSkinColor(string skinDictionary)
        {
            this.Resources.MergedDictionaries.Clear();
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri(skinDictionary.Replace("\\\\", "\\"), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}
