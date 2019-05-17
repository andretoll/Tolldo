using System.Windows;
using System.Windows.Markup;

namespace Tolldo.Extensions
{
    /// <summary>
    /// Extension to provide icon string
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

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached(
                "Icon", typeof(string), typeof(IconExtension),
                new UIPropertyMetadata("", null));
    }
}
