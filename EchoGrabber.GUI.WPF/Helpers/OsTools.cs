using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EchoGrabber.GUI.WPF.Helpers
{
    public class OsTools
    {
        static readonly bool is64BitProcess = (IntPtr.Size == 8);
        static readonly bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );
        /// <summary>
        /// Проверка битности ОС
        /// </summary>
        private static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (var p = Process.GetCurrentProcess())
                {
                    if (!IsWow64Process(p.Handle, out bool retVal))
                        return false;
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Путь к браузеру по умолчанию
        /// </summary>
        private static string GetDefaultBrowserPath()
        {
            const string userChoice = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
            var result = string.Empty;
            using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoice))
            {
                if (userChoiceKey == null)
                    throw new Win32Exception();
                var progId = userChoiceKey.GetValue("Progid").ToString();
                if (string.IsNullOrEmpty(progId))
                    return result;
                var progIdPath = $@"{progId}\Application";
                using (var pathKey = Registry.ClassesRoot.OpenSubKey(progIdPath))
                {

                    if (pathKey == null)
                        return result;
                    var path = pathKey.GetValue("ApplicationIcon").ToString();
                    result = path.Split(',')[0];
                }
            }

            return result;
        }
        /// <summary>
        /// Список всех установленных браузеров в системе
        /// </summary>
        public static List<BrowserInfo> GetBrowsers()
        {
            var browsersRegistryKeyPath = $@"SOFTWARE{(is64BitOperatingSystem ? @"\WOW6432Node" : "")}\Clients\StartMenuInternet";

            var shellCommandKeyPath = @"shell\open\command";
            var iconKeyPath = @"DefaultIcon";
            var result = new List<BrowserInfo>();
            using (var browsersKey = Registry.LocalMachine.OpenSubKey(browsersRegistryKeyPath))
                foreach (string browserKeyName in browsersKey.GetSubKeyNames())
                    using (var browserKey = browsersKey.OpenSubKey(browserKeyName))
                    using (var iconKey = browserKey.OpenSubKey(iconKeyPath))
                    using (var shellCommandKey = browserKey.OpenSubKey(shellCommandKeyPath))
                    {
                        var browserName = browserKey.GetValue(null).ToString();
                        var iconPath = iconKey.GetValue(null).ToString();
                        iconPath = iconPath.Split(',')[0];
                        var icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
                        var browserPath = shellCommandKey.GetValue(null).ToString();
                        var isDefault = GetDefaultBrowserPath() == iconPath;
                        result.Add(new BrowserInfo(browserName, browserPath, icon, isDefault));
                    }
            return result;
        }

    }
}
