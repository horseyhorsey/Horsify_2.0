using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _appInstance;
        private static string _appKey = "{FC3A7A4A-03DF-449B-B88D-AF76E62899DC}";

        public App()
        {
            // Do the following if application isn't running
            if (!StartUp())
            {
                return;
            }

            //Shutdown and activate the running process
            Application.Current.Shutdown();
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
        private static void ShutdownInstance(int code = 0)
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
    }
}
