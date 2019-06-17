using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Tolldo.Helpers
{
    /// <summary>
    /// A class that manages all read and write actions for the application's settings.
    /// </summary>
    public static class SettingsManager
    {
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
        /// Writes to the specified <see cref="Setting"/> with the specified value.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        /// <param name="value">Value to assign to the property.</param>
        public static void WriteToSetting(string property, object value)
        {
            Properties.Settings.Default[property] = value;
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

        public static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
            CompletedTasksOnTop,
            ProgressBarHeight,
            HideCompletedTasks,
            WelcomeMessage,
            AppImage,
            TodoProgressShowPercent,
            MinimizeToTray,
            MinimizeToTrayMessage,
            PinnedLocation
        }

        #endregion
    }
}
