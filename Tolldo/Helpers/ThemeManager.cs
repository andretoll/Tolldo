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

        #endregion

        #region Public Properties

        public bool DarkThemeEnabled { get { return _darkThemeEnabled; } private set { _darkThemeEnabled = value; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThemeManager()
        {
            // Get settings
            SetTheme(true);
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

        #endregion
    }
}
