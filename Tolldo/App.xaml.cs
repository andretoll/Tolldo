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
        private const int _windowRestore = 9;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        /// <summary>
        /// Only allow one instance of the application to run.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            _mutex = new Mutex(true, _appName, out createdNew);

            if (!createdNew)
            {
                Process current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        int hWnd = (int)process.MainWindowHandle;
                        ShowWindow(hWnd, _windowRestore);
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }

                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
