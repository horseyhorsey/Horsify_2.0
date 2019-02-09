using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace Horsesoft.Music.Horsify.WPF.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _appInstance;
        private static string _appKey = "{255F478D-E92E-4563-8615-808188E30685}";

        public App()
        {
            // Do the following if application isn't running
            if (!StartUp())
            {
                //Frame limiter
                Timeline.DesiredFrameRateProperty.OverrideMetadata(
                    typeof(Timeline),
                    new FrameworkPropertyMetadata { DefaultValue = 25 });

                return;
            }

            //Shutdown and activate the running process
            Application.Current.Shutdown();
            var horsifyProcesses = Process.GetProcessesByName("Horsify Jukebox");
            //var ps = Process.GetProcesses();
            if (horsifyProcesses?.Count() > 0)
                SetForegroundWindowNative(horsifyProcesses[0].MainWindowHandle);
        }

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindowNative(IntPtr hWnd);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private bool StartUp()
        {            
            _appInstance = new Mutex(true, _appKey);
            bool alive = false;
            try
            {
                alive = !_appInstance.WaitOne(TimeSpan.Zero, true);
            }
            catch (AbandonedMutexException)
            {
                ShutdownInstance();
                alive = false;
            }
            catch (Exception)
            {
                _appInstance.Close();
                alive = false;
            }
            return alive;
        }

        /// <summary>
        /// Kills the instance.
        /// </summary>
        /// <param name="code">The code.</param>
        internal static void ShutdownInstance(int code = 0)
        {
            if (_appInstance == null) return;
            if (code == 0)
            {
                try
                {
                    _appInstance.ReleaseMutex();
                }
                catch (Exception) { }
            }
            _appInstance.Close();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ShutdownInstance(e.ApplicationExitCode);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //Only ask to shutdown if it was an initial importer error
            if (!e.Exception.Message.Contains("Prism.Mvvm.ViewModelLocator.AutoWireViewModel"))
            {
                System.Windows.MessageBox.Show(e.Exception.Message);
                System.Windows.MessageBox.Show(e.Exception.StackTrace);

                var result = System.Windows.MessageBox.Show("Shutdown Horsify?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    e.Handled = true;
            }    
        }
    }
}
