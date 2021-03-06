﻿using System.Windows;
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
        private const int _windowMinimumWidth = 395;
        private const int _windowMinimumHeight = 475;

        // Thickness for resize border cursor
        private const int _resizeBorder = 6;

        // Margin for outer window
        private int _outerMarginSize = 10;

        // Height of the title bar
        private const int _titleHeight = 36;

        // Indicates if window is pinned
        private bool _pinned;
        private double _previousWidth;
        private double _previousHeight;
        private Point _previousPoint;

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

        // Indicates if window is pinned
        public bool Pinned
        {
            get
            {
                return _pinned;
            }
            set
            {
                PinWindow(value);

                _pinned = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }

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

            // Initialize NotifyIcon
            InitializeNotifyIcon();

            // Listen for location changed
            _window.LocationChanged += (sender, e) =>
            {
                if (Pinned)
                {
                    _window.Topmost = false;
                    _pinned = false;
                    NotifyPropertyChanged(nameof(Pinned));
                }
            };

            // Get window settings
            _minimizeToTray = bool.Parse(SettingsManager.LoadSetting(SettingsManager.Setting.MinimizeToTray.ToString()).ToString());
            _minimizeToTrayMessage = bool.Parse(SettingsManager.LoadSetting(SettingsManager.Setting.MinimizeToTrayMessage.ToString()).ToString());

            // Listen for window state changed
            _window.StateChanged += (sender, e) =>
            {
                NotifyPropertyChanged(nameof(ResizeBorderThickness));
                NotifyPropertyChanged(nameof(OuterMarginSize));
                NotifyPropertyChanged(nameof(OuterMarginSizeThickness));

                // Minimize to tray if setting is active
                if (_minimizeToTray)
                {
                    // If window is minimized, enable NotifyIcon
                    if (_window.WindowState == WindowState.Minimized)
                    {
                        // Hide window and enable NotifyIcon
                        _window.Hide();
                        _notifyIcon.Visible = true;

                        // Show message if setting is active
                        if (_minimizeToTrayMessage)
                        {
                            _notifyIcon.ShowBalloonTip(2000, "Window minimized", "You can change these settings under General Settings.", ToolTipIcon.None);
                            _minimizeToTrayMessage = false;
                        }
                    }
                    // If window is normal, disable NotifyIcon
                    else if (_window.WindowState == WindowState.Normal)
                        _notifyIcon.Visible = false; 
                }
            };
            
            // Initialize commands
            MinimizeCommand = new RelayCommand.RelayCommand(p => { _window.WindowState = WindowState.Minimized; });
            MaximizeCommand = new RelayCommand.RelayCommand(p => { _window.WindowState ^= WindowState.Maximized; });
            CloseCommand = new RelayCommand.RelayCommand(p => { _window.Close(); });

            // Resize windows when maximized
            var resizer = new WindowResizer.WindowResizer(_window);
        }

        #endregion

        #region Private Helpers

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

        /// <summary>
        /// Pins the window topmost in a specific place, with a specific size.
        /// </summary>
        /// <param name="b"></param>
        private void PinWindow(bool b)
        {
            if (_window.WindowState == WindowState.Maximized | _window.WindowState == WindowState.Minimized)
                return;

            if (b)
            {
                // Save current dimensions and position
                _previousWidth = _window.Width;
                _previousHeight = _window.Height;
                _previousPoint = new Point(_window.Left, _window.Top);

                // Set width and height
                _window.Width = _windowMinimumWidth;
                _window.Height = _windowMinimumHeight;

                // Set to be always on top
                _window.Topmost = true;

                // Set position
                string pos = SettingsManager.LoadSetting(SettingsManager.Setting.PinnedLocation.ToString()).ToString();
                var desktopWorkingArea = SystemParameters.WorkArea;

                switch(pos)
                {
                    // Top right
                    case "TopRight":
                        _window.Left = desktopWorkingArea.Right - _window.Width + this.OuterMarginSize;
                        _window.Top = 0;
                        break;
                    // Top left
                    case "TopLeft":
                        _window.Left = 0 - this.OuterMarginSize;
                        _window.Top = 0;
                        break;
                    // Middle right
                    case "MiddleRight":
                        _window.Left = desktopWorkingArea.Right - _window.Width + this.OuterMarginSize;
                        _window.Top = desktopWorkingArea.Bottom / 2 - (_window.Height / 2);
                        break;
                    // Middle left
                    case "MiddleLeft":
                        _window.Left = 0 - this.OuterMarginSize;
                        _window.Top = desktopWorkingArea.Bottom / 2 - (_window.Height / 2);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Revert to previous width and height
                _window.Width = _previousWidth;
                _window.Height = _previousHeight;

                // Set to not be on top
                _window.Topmost = false;

                // Revert to previous position
                _window.Left = _previousPoint.X;
                _window.Top = _previousPoint.Y;

                _pinned = false;
            }
        }

        /// <summary>
        /// Initializes the <see cref="NotifyIcon"/> class.
        /// </summary>
        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(new System.Uri("pack://application:,,,/Tolldo;component/Images/Logo/favicon.ico")).Stream);
            _notifyIcon.Text = "Tolldo";
            _notifyIcon.ContextMenu = CreateContextMenu();
            _notifyIcon.Click += (sender, e) =>
            {
                RestoreWindow();
            };
            _notifyIcon.DoubleClick += (sender, e) =>
            {
                RestoreWindow();
            };
        }

        #endregion
    }
}
