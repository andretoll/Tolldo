using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Tolldo.Extensions
{
    /// <summary>
    /// Extension that only allows left mouse button to select items in a list.
    /// </summary>
    public class OnlyLeftMouseButtonSelectExtension
    {
        public static readonly DependencyProperty OnlyLeftMouseButtonSelectProperty =
            DependencyProperty.RegisterAttached(
                "OnlyLeftMouseButtonSelect", typeof(bool), typeof(ListSelectBehaviorExtension),
                new PropertyMetadata(default(bool), HandleOnlyLeftMouseButtonSelect));

        public static void SetOnlyLeftMouseButtonSelect(DependencyObject element, bool value)
        {
            element.SetValue(OnlyLeftMouseButtonSelectProperty, value);
        }

        public static bool GetOnlyLeftMouseButtonSelect(DependencyObject element)
        {
            return (bool)element.GetValue(OnlyLeftMouseButtonSelectProperty);
        }

        private static void HandleOnlyLeftMouseButtonSelect(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector selector)
            {
                selector.PreviewMouseDown -= HandleSelectPreviewMouseDown;

                if (Equals(e.NewValue, true))
                {
                    selector.PreviewMouseDown += HandleSelectPreviewMouseDown;
                }
            }
        }

        private static void HandleSelectPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = e.ChangedButton == MouseButton.Right;
        }
    }
}
