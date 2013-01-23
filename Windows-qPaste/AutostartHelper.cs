using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows_qPaste
{
    using Microsoft.Win32;
    using System.Windows.Forms;

    /// <summary>
    /// Autostart helper class.
    /// </summary>
    public class AutostartHelper
    {
        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string KEY_NAME = @"qShare";

        /// <summary>
        /// Sets the autostart value for the assembly.
        /// </summary>
        public static void SetAutostart()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.SetValue(KEY_NAME, Application.ExecutablePath.ToString());
        }

        /// <summary>
        /// Check if autostart is enabled.
        /// </summary>
        /// <returns>Returns whether the 'qShare' key exists in the registry.</returns>
        public static bool isAutostartEnabled()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
            if (key == null)
                return false;

            string value = (string)key.GetValue(KEY_NAME);
            //if (value == null)
            //    return false;

            return (value != null);
        }

        /// <summary>
        /// Check if this executable is the autostarting one. 
        /// </summary>
        /// <returns>Returns whether the path to this executable is the same as the one in the registry.</returns>
        public static bool isAutostartPathThis()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
            if (key == null)
                return false;

            string value = (string)key.GetValue(KEY_NAME);
            return value.Equals(Application.ExecutablePath.ToString());
        }

        /// <summary>
        /// Unsets the autostart value for the assembly.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        public static void UnSetAutostart()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.DeleteValue(KEY_NAME);
        }
    }
}
