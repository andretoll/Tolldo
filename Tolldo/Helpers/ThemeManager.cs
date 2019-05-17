using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Tolldo.Helpers
{
    /// <summary>
    /// A manager used to change themes and accents.
    /// </summary>
    public class ThemeManager
    {
        #region Private Members

        private bool _darkThemeEnabled;
        private List<string> _accents;
        private string _accent;

        #endregion

        #region Public Properties

        public bool DarkThemeEnabled { get { return _darkThemeEnabled; } private set { _darkThemeEnabled = value; } }
        public List<string> Accents { get { return _accents; } private set { _accents = value; } }
        public string Accent { get { return _accent; } private set { _accent = value; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThemeManager()
        {
            // Get settings
            DarkThemeEnabled = (bool)LoadSetting(Setting.DarkTheme.ToString());
            Accent = (string)LoadSetting(Setting.Accent.ToString());

            // Apply settings
            SetTheme(DarkThemeEnabled);
            SetAccent(Accent);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Method to set dark or light theme.
        /// </summary>
        /// <param name="dark"></param>
        public void SetTheme(bool dark)
        {
            DarkThemeEnabled = dark;

            if (dark)
            {
                // Set primary background                
                SetBrush("Dark", "PrimaryBackgroundBrush");

                // Set secondary background
                SetBrush("DarkGray", "SecondaryBackgroundBrush");

                // Set gray background
                SetBrush("MediumDarkGray", "GrayBackgroundBrush");

                // Set primary foreground
                SetBrush("LightGray", "PrimaryForegroundBrush");

                // Set inverted
                SetBrush("Light", "InvertedBackgroundBrush");
            }
            else
            {
                // Set primary background
                SetBrush("Light", "PrimaryBackgroundBrush");

                // Set secondary background
                SetBrush("LightGray", "SecondaryBackgroundBrush");

                // Set gray background
                SetBrush("MediumLightGray", "GrayBackgroundBrush");

                // Set primary foreground
                SetBrush("Dark", "PrimaryForegroundBrush");

                // Set inverted
                SetBrush("Dark", "InvertedBackgroundBrush");
            }

            // Save setting
            SaveSetting(Setting.DarkTheme.ToString(), DarkThemeEnabled);
        }

        /// <summary>
        /// Method to set accent.
        /// </summary>
        /// <param name="accent"></param>
        public void SetAccent(string accent)
        {
            Accent = accent;

            switch(accent)
            {
                case "Blue":
                    SetBrush("Blue", "ColorForegroundBrush");
                    SetBrush("DarkerBlue", "DarkerColorForegroundBrush");
                    SetBrush("DarkBlue", "DarkColorForegroundBrush");
                    SetLinearBrush("DarkerBlue", "Blue", "ColorGradientBrush");
                    break;
                case "Red":
                    SetBrush("Red", "ColorForegroundBrush");
                    SetBrush("DarkerRed", "DarkerColorForegroundBrush");
                    SetBrush("DarkRed", "DarkColorForegroundBrush");
                    SetLinearBrush("DarkerRed", "Red", "ColorGradientBrush");
                    break;
                case "Orange":
                    SetBrush("Orange", "ColorForegroundBrush");
                    SetBrush("DarkerOrange", "DarkerColorForegroundBrush");
                    SetBrush("DarkOrange", "DarkColorForegroundBrush");
                    SetLinearBrush("DarkerOrange", "Orange", "ColorGradientBrush");
                    break;
                default:
                    SetBrush("Blue", "ColorForegroundBrush");
                    SetBrush("DarkerBlue", "DarkerColorForegroundBrush");
                    SetBrush("DarkBlue", "DarkColorForegroundBrush");
                    SetLinearBrush("DarkerBlue", "Blue", "ColorGradientBrush");
                    break;
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Method to assign the specified <see cref="Color"/> to the specified <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="colorName"></param>
        /// <param name="brushName"></param>
        private void SetBrush(string colorName, string brushName)
        {
            Color color = (Color)Application.Current.Resources[colorName];
            SolidColorBrush brush = new SolidColorBrush(color);
            Application.Current.Resources[brushName] = brush;
        }

        /// <summary>
        /// Method to assign the specified <see cref="Color"/>s to the specified <see cref="LinearGradientBrush"/>.
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="brushName"></param>
        private void SetLinearBrush(string color1, string color2, string brushName)
        {
            Color firstColor = (Color)Application.Current.Resources[color1];
            Color secondColor = (Color)Application.Current.Resources[color2];

            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(1, 1);
            brush.GradientStops.Add(new GradientStop(firstColor, 0.0));
            brush.GradientStops.Add(new GradientStop(secondColor, 0.50));
            brush.GradientStops.Add(new GradientStop(firstColor, 1.0));

            Application.Current.Resources[brushName] = brush;
        }

        /// <summary>
        /// Method to save the specific setting with the specific value.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SaveSetting(string property, object value)
        {
            Properties.Settings.Default[property] = value;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Method to load the specific setting.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private object LoadSetting(string property)
        {
            return Properties.Settings.Default[property];
        }

        #endregion

        private enum Setting
        {
            DarkTheme,
            Accent
        }

        private enum AccentRotation
        {
            Blue = 1,
            Red = 2,
            Orange = 3
        }
    }
}
