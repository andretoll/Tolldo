using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Tolldo.Helpers
{
    /// <summary>
    /// A class that manages all read and write actions for the application's settings.
    /// </summary>
    public static class SettingsManager
    {
        #region Public Properties

        /// <summary>
        /// The relative path to the banner images.
        /// </summary>
        public static string BannerImageDirectory { get; set; } = "/Images/Banners/";

        /// <summary>
        /// The default banner image name.
        /// </summary>
        public static string DefaultBannerImage { get; } = "banner_winter.jpg";

        #endregion

        #region Public Helpers

        /// <summary>
        /// Saves all current settings.
        /// </summary>
        public static void SaveAllSettings()
        {
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Reloads all previous settings.
        /// </summary>
        public static void ReloadSettings()
        {
            Properties.Settings.Default.Reload();
        }

        /// <summary>
        /// Saves the specified <see cref="Setting"/> with the specified value.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        /// <param name="value">Value to assign to the property.</param>
        public static void SaveSetting(string property, object value)
        {
            Properties.Settings.Default[property] = value;
            SaveAllSettings();
        }

        /// <summary>
        /// Loads the specified setting.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        /// <returns></returns>
        public static object LoadSetting(string property)
        {
            return Properties.Settings.Default[property];
        }

        /// <summary>
        /// Gets the default banner image url.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultBannerImage()
        {
            return Path.Combine(BannerImageDirectory, DefaultBannerImage);
        }

        /// <summary>
        /// Gets the list of banner images of the application. 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetBannerImageList()
        {
            string folder;
            string[] files;

            try
            {
                folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Images\Banners\";
                files = Directory.GetFiles(folder);
            }
            catch (Exception)
            {
                return new List<string>();
            }

            return new List<string>(files);
        }

        #endregion

        #region Enums

        /// <summary>
        /// Types of settings.
        /// </summary>
        public enum Setting
        {
            DarkTheme,
            Accent,
            LastTodo,
            NumberedTasks,
            CompletedTasksOnTop
        }

        #endregion
    }
}
