﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Tolldo.Helpers
{
    /// <summary>
    /// A theme mananger to apply themes and accents based on user settings.
    /// </summary>
    public class ThemeManager
    {
        #region Public Properties

        public bool DarkThemeEnabled { get; private set; }       
        public string Accent { get; private set; }
        public List<string> Accents { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ThemeManager()
        {
            // Get settings
            DarkThemeEnabled = (bool)SettingsManager.LoadSetting(SettingsManager.Setting.DarkTheme.ToString());
            Accent = (string)SettingsManager.LoadSetting(SettingsManager.Setting.Accent.ToString());

            // Apply settings
            SetTheme(DarkThemeEnabled);
            SetAccent(Accent);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Sets dark or light theme.
        /// </summary>
        /// <param name="dark">A value indicating whether or not to apply the dark theme.</param>
        public void SetTheme(bool dark)
        {
            DarkThemeEnabled = dark;

            if (dark)
            {
                // Set primary background                
                SetBrush("DarkGray", "PrimaryBackgroundBrush");

                // Set secondary background
                SetBrush("Dark", "SecondaryBackgroundBrush");

                // Set gray background
                SetBrush("MediumDarkGray", "GrayBackgroundBrush");

                // Set primary foreground
                SetBrush("Light", "PrimaryForegroundBrush");
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
            }
        }

        /// <summary>
        /// Sets the provided accent.
        /// </summary>
        /// <param name="accent">The accent name to apply.</param>
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
                case "Pink":
                    SetBrush("Pink", "ColorForegroundBrush");
                    SetBrush("DarkerPink", "DarkerColorForegroundBrush");
                    SetBrush("DarkPink", "DarkColorForegroundBrush");
                    SetLinearBrush("DarkerPink", "Pink", "ColorGradientBrush");
                    break;
                case "Purple":
                    SetBrush("Purple", "ColorForegroundBrush");
                    SetBrush("DarkerPurple", "DarkerColorForegroundBrush");
                    SetBrush("DarkPurple", "DarkColorForegroundBrush");
                    SetLinearBrush("DarkerPurple", "Purple", "ColorGradientBrush");
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
        /// Applies the specified <see cref="Color"/> to the specified <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="colorName">The name of the <see cref="Color"/> from resources.</param>
        /// <param name="brushName">The name of the <see cref="SolidColorBrush"/></param>
        private void SetBrush(string colorName, string brushName)
        {
            Color color = (Color)Application.Current.Resources[colorName];
            SolidColorBrush brush = new SolidColorBrush(color);
            Application.Current.Resources[brushName] = brush;
        }

        /// <summary>
        /// Applies the specified <see cref="Color"/>s to the specified <see cref="LinearGradientBrush"/>.
        /// </summary>
        /// <param name="color1">The name of the <see cref="Color"/> from resources.</param>
        /// <param name="color2">The name of the <see cref="Color"/> from resources.</param>
        /// <param name="brushName">Name of the <see cref="LinearGradientBrush"/>.</param>
        private void SetLinearBrush(string color1, string color2, string brushName)
        {
            // Retrieve the colors
            Color firstColor = (Color)Application.Current.Resources[color1];
            Color secondColor = (Color)Application.Current.Resources[color2];

            // Create new brush
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(1, 1);
            brush.GradientStops.Add(new GradientStop(firstColor, 0.0));
            brush.GradientStops.Add(new GradientStop(secondColor, 0.50));
            brush.GradientStops.Add(new GradientStop(firstColor, 1.0));

            // Apply new brush
            Application.Current.Resources[brushName] = brush;
        }

        #endregion        
    }
}
