using System.Windows;
using System.Windows.Input;

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

        // Minimum window width and height
        private const int _windowMinimumWidth = 400;
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

            // Listen for window resizing
            _window.StateChanged += (sender, e) =>
            {
                NotifyPropertyChanged(nameof(ResizeBorderThickness));
                NotifyPropertyChanged(nameof(OuterMarginSize));
                NotifyPropertyChanged(nameof(OuterMarginSizeThickness));
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
        /// Return the current mouse position.
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

        #endregion
    }
}
