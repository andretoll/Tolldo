using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Tolldo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        private const string _appName = "Tolldo";

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Only allow one instance of the application to run.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            _mutex = new Mutex(true, _appName, out createdNew);

            // If process is already running, show message and exit
            if (!createdNew)
            {
                // Check if process is already running
                Process current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        int hWnd = (int)process.MainWindowHandle;

                        // If original application is NOT minimized, show window
                        if (hWnd != 0)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                        }
                        // If original application is minimized, show message
                        else
                        {
                            MessageBox.Show("Application is already running.", "Tolldo", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        }

                        break;
                    }
                }

                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
