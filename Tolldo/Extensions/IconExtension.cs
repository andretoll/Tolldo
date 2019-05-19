using System.Windows;

namespace Tolldo.Extensions
{
    /// <summary>
    /// A dependency property that accepts a string to represent an icon.
    /// </summary>
    public static class IconExtension
    {
        public static string GetIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, string value)
        {
            obj.SetValue(IconProperty, value);
        }

        /// <summary>
        /// A dependency property that accepts a string to represent an icon.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached(
                "Icon", typeof(string), typeof(IconExtension),
                new UIPropertyMetadata("", null));
    }
}
