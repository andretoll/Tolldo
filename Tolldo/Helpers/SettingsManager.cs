namespace Tolldo.Helpers
{
    /// <summary>
    /// A class that manages all read and write actions for the application's settings.
    /// </summary>
    public static class SettingsManager
    {
        #region Public Helpers

        /// <summary>
        /// Saves the specified <see cref="Setting"/> with the specified value.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        /// <param name="value">Value to assign to the property.</param>
        public static void SaveSetting(string property, object value)
        {
            Properties.Settings.Default[property] = value;
            Properties.Settings.Default.Save();
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

        #endregion

        #region Enums

        /// <summary>
        /// Types of settings.
        /// </summary>
        public enum Setting
        {
            DarkTheme,
            Accent,
            LastTodo
        }

        #endregion
    }
}
