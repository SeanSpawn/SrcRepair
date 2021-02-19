﻿/*
 * This file is a part of SRC Repair project. For more information
 * visit official site: https://www.easycoding.org/projects/srcrepair
 *
 * Copyright (c) 2011 - 2021 EasyCoding Team (ECTeam).
 * Copyright (c) 2005 - 2021 EasyCoding Team.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;

namespace srcrepair.core
{
    /// <summary>
    /// Class for working with platform-dependent functions.
    /// </summary>
    public abstract class CurrentPlatform : IPlatform
    {
        /// <summary>
        /// Create a platform-dependent instance. Factory method.
        /// </summary>
        public static CurrentPlatform Create()
        {
            switch (GetRunningOS())
            {
                case OSType.Windows:
                    return new PlatformWindows();
                case OSType.Linux:
                    return new PlatformLinux();
                case OSType.MacOSX:
                    return new PlatformMac();
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        /// <summary>
        /// Codes and IDs of available platforms.
        /// </summary>
        public enum OSType
        {
            Windows = 0,
            MacOSX = 1,
            Linux = 2
        }

        /// <summary>
        /// Get current operating system friendly name.
        /// </summary>
        public string OSFriendlyName => OS.ToString();

        /// <summary>
        /// Get name and ID of running operating system.
        /// </summary>
        /// <returns>Platform ID.</returns>
        private static OSType GetRunningOS()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    return Directory.Exists("/Applications") ? OSType.MacOSX : OSType.Linux;
                case PlatformID.MacOSX:
                    return OSType.MacOSX;
                default: return OSType.Windows;
            }
        }

        /// <summary>
        /// Add quotes to the path.
        /// </summary>
        /// <param name="Source">Source string with path.</param>
        /// <returns>Quoted string with path.</returns>
        protected static string AddQuotesToPath(string Source)
        {
            return String.Format(Properties.Resources.AppOpenHandlerEscapeTemplate, Source);
        }

        /// <summary>
        /// Open the specified URL in default Web browser.
        /// </summary>
        /// <param name="URI">Full URL.</param>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public virtual void OpenWebPage(string URI)
        {
            Process.Start(URI);
        }

        /// <summary>
        /// Restart current application with admin user rights.
        /// </summary>
        /// <param name="OS">Operating system type.</param>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public virtual void RestartApplicationAsAdmin()
        {
            StartElevatedProcess(CurrentApp.AssemblyLocation);
            Environment.Exit(ReturnCodes.Success);
        }

        /// <summary>
        /// Starts an external helper application.
        /// </summary>
        /// <param name="FileName">Full path to helper application.</param>
        /// <param name="Elevated">Run with administrator privileges.</param>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public virtual void StartExternalHelper(string FileName, bool Elevated = false)
        {
            if (File.Exists(FileName))
            {
                if (Elevated) { StartElevatedProcess(FileName); } else { StartRegularProcess(FileName); }
            }
            else
            {
                throw new FileNotFoundException(DebugStrings.AppDbgExCoreHelperNxExists, FileName);
            }
        }

        /// <summary>
        /// Start the required application as the current user.
        /// </summary>
        /// <param name="FileName">Full path to the executable.</param>
        /// <returns>PID of the newly created process.</returns>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public virtual int StartRegularProcess(string FileName)
        {
            return Process.Start(FileName).Id;
        }

        /// <summary>
        /// Get platform-dependent suffix for HTTP_USER_AGENT header.
        /// </summary>
        public virtual string UASuffix => Properties.Resources.AppUASuffixOther;

        /// <summary>
        /// Get current operating system ID.
        /// </summary>
        public abstract OSType OS { get; }

        /// <summary>
        /// Get platform-dependent Steam installation folder (directory) name.
        /// </summary>
        public abstract string SteamFolderName { get; }

        /// <summary>
        /// Get platform-dependent Steam launcher file name.
        /// </summary>
        public abstract string SteamBinaryName { get; }

        /// <summary>
        /// Get platform-dependent SteamApps directory name.
        /// </summary>
        public abstract string SteamAppsFolderName { get; }

        /// <summary>
        /// Get platform-dependent Steam process name.
        /// </summary>
        public abstract string SteamProcName { get; }

        /// <summary>
        /// Return platform-dependent location of the Hosts file.
        /// </summary>
        public virtual string HostsFileLocation => "/etc";

        /// <summary>
        /// Get platform-dependent Steam installation path.
        /// </summary>
        public virtual string SteamInstallPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SteamFolderName);

        /// <summary>
        /// Get platform-dependent Steam language.
        /// </summary>
        public virtual string SteamLanguage => "english";

        /// <summary>
        /// Return platform-dependent path to Hosts file.
        /// </summary>
        public virtual string HostsFileFullPath => Path.Combine(HostsFileLocation, "hosts");

        /// <summary>
        /// Open the specified text file in default (or overrided in application's
        /// settings (only on Windows platform)) text editor.
        /// </summary>
        /// <param name="FileName">Full path to text file.</param>
        /// <param name="EditorBin">External text editor (Windows only).</param>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public abstract void OpenTextEditor(string FileName, string EditorBin);

        /// <summary>
        /// Show the specified file in default file manager.
        /// </summary>
        /// <param name="FileName">Full path to file.</param>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public abstract void OpenExplorer(string FileName);

        /// <summary>
        /// Start the required application from administrator.
        /// </summary>
        /// <param name="FileName">Full path to the executable.</param>
        /// <returns>PID of the newly created process.</returns>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public abstract int StartElevatedProcess(string FileName);
    }
}
