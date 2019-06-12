using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Tolldo.Helpers;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a custom chrome window. Based on the <see cref="BaseViewModel"/>.
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Members

        // Referenced window
        private Window _window;

        // NotifyIcon (Task tray)
        private NotifyIcon _notifyIcon;
        private bool _minimizeToTray;
        private bool _minimizeToTrayMessage;

        // Minimum window width and height
        private const int _windowMinimumWidth = 410;
        private const int _windowMinimumHeight = 400;

        // Thickness for resize border cursor
        private const int _resizeBorder = 6;

        // Margin for outer window
        private int _outerMarginSize = 10;

        // Height of the title bar
        private const int _titleHeight = 36;

        #endregion

        #region Public Properties

        // Minimum window width and height (READONLY)
        public double WindowMinimumWidth { get { return _windowMinimumWidth; } }
        public double WindowMinimumHeight { get { return _windowMinimumHeight; } }

        // Size of the resize border around the window
        public Thickness ResizeBorderThickness { get { return new Thickness(_resizeBorder + OuterMarginSize); } }        

        // Margin of outer window. If maximized, return 0
        public int OuterMarginSize { get { return _window.WindowState == WindowState.Maximized ? 0 : _outerMarginSize; } }        
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        // Height of the title bar
        public int TitleHeight { get { return _titleHeight; } }
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + _resizeBorder); } }

        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that accepts a <see cref="Window"/>.
        /// </summary>
        /// <param name="window"></param>
        public WindowViewModel(Window window)
        {
            // Initialize window
            _window = window;

            // Get window settings
            _minimizeToTray = bool.Parse(SettingsManager.LoadSetting(SettingsManager.Setting.MinimizeToTray.ToString()).ToString());
            _minimizeToTrayMessage = bool.Parse(SettingsManager.LoadSetting(SettingsManager.Setting.MinimizeToTrayMessage.ToString()).ToString());

            // Initialize notify icon
            _notifyIcon = new NotifyIcon();            
            _notifyIcon.Icon = new System.Drawing.Icon(Path.Combine(SettingsManager.GetApplicationDirectory(), @"Images\Logo\favicon.ico"));
            _notifyIcon.Text = "Tolldo";
            _notifyIcon.ContextMenu = CreateContextMenu();
            _notifyIcon.Click += (sender, e) =>
            {
                // Open context menu on left click
                System.Reflection.MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            };
            _notifyIcon.DoubleClick += (sender, e) =>
            {
                RestoreWindow();
            };            

            // Listen for window state changed
            _window.StateChanged += (sender, e) =>
            {
                NotifyPropertyChanged(nameof(ResizeBorderThickness));
                NotifyPropertyChanged(nameof(OuterMarginSize));
                NotifyPropertyChanged(nameof(OuterMarginSizeThickness));

                // Minimize to tray if setting is active
                if (_minimizeToTray)
                {
                    if (_window.WindowState == WindowState.Minimized)
                    {
                        _window.Hide();
                        _notifyIcon.Visible = true;

                        // Show message if setting is active
                        if (_minimizeToTrayMessage)
                        {
                            _notifyIcon.ShowBalloonTip(2000, "Window minimized", "You can change these settings under General Settings.", ToolTipIcon.None);
                            _minimizeToTrayMessage = false;
                        }
                    }
                    // Restore window
                    if (_window.WindowState == WindowState.Normal)
                        _notifyIcon.Visible = false; 
                }
            };
            
            // Initialize commands
            MinimizeCommand = new RelayCommand.RelayCommand(p => { _window.WindowState = WindowState.Minimized; });
            MaximizeCommand = new RelayCommand.RelayCommand(p => { _window.WindowState ^= WindowState.Maximized; });
            CloseCommand = new RelayCommand.RelayCommand(p => { _window.Close(); });
            MenuCommand = new RelayCommand.RelayCommand(p => { SystemCommands.ShowSystemMenu(_window, GetMousePosition()); });

            // Resize windows when maximized
            var resizer = new WindowResizer.WindowResizer(_window);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Returns the current mouse position.
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(_window);

            if (_window.WindowState == WindowState.Maximized)
                return new Point(position.X, position.Y);
            else
                return new Point(position.X + _window.Left, position.Y + _window.Top);
        }

        /// <summary>
        /// Creates the taskbar context menu.
        /// </summary>
        /// <returns></returns>
        private ContextMenu CreateContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            // Open window
            contextMenu.MenuItems.Add("Open", (sender, e) =>
            {
                RestoreWindow();
            });

            // Exit
            contextMenu.MenuItems.Add("Exit", (sender, e) =>
            {
                _notifyIcon.Visible = false;
                System.Windows.Application.Current.Shutdown();
            });

            return contextMenu;
        }

        /// <summary>
        /// Restores the window to its normal <see cref="WindowState"/>.
        /// </summary>
        private void RestoreWindow()
        {
            _window.Show();
            _window.WindowState = WindowState.Normal;
        }

        #endregion
    }
}
