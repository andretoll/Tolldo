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
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
