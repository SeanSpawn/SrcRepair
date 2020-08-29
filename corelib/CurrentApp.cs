﻿/*
 * This file is a part of SRC Repair project. For more information
 * visit official site: https://www.easycoding.org/projects/srcrepair
 *
 * Copyright (c) 2011 - 2020 EasyCoding Team (ECTeam).
 * Copyright (c) 2005 - 2020 EasyCoding Team.
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
using System.Globalization;
using System.IO;
using System.Reflection;
using NLog;

namespace srcrepair.core
{
    /// <summary>
    /// Class for working with running application instance.
    /// </summary>
    public sealed class CurrentApp
    {
        /// <summary>
        /// Get User-Agent header for outgoing HTTP queries.
        /// </summary>
        public string UserAgent { get; private set; }

        /// <summary>
        /// Get SRC Repair's installation directory.
        /// </summary>
        public string FullAppPath { get; private set; }

        /// <summary>
        /// Get path to SRC Repair's user directory.
        /// </summary>
        public string AppUserDir { get; private set; }

        /// <summary>
        /// Get information about running operating system.
        /// </summary>
        public CurrentPlatform Platform { get; private set; }

        /// <summary>
        /// Get or set list of available Source games.
        /// </summary>
        public GameManager SourceGames { get; set; }

        /// <summary>
        /// Get or set Steam configuration.
        /// </summary>
        public SteamManager SteamClient { get; set; }

        /// <summary>
        /// Get or set plugins configuration.
        /// </summary>
        public PluginManager Plugins { get; set; }

        /// <summary>
        /// Get information about hardware architecture.
        /// </summary>
        private string SystemArch => Environment.Is64BitOperatingSystem ? "Amd64" : "x86";

        /// <summary>
        /// Get full path to Nlog active log file.
        /// </summary>
        public static string LogFileName
        {
            get
            {
                NLog.Targets.FileTarget LogTarget = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("logfile");
                return Path.GetFullPath(LogTarget.FileName.Render(new LogEventInfo()));
            }
        }

        /// <summary>
        /// Get full path to Nlog's directory for storing log files.
        /// </summary>
        public static string LogDirectoryPath => Path.GetDirectoryName(LogFileName);

        /// <summary>
        /// Get application name from the resource section of calling assembly.
        /// </summary>
        public static string AppProduct
        {
            get
            {
                object[] Attribs = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return Attribs.Length != 0 ? ((AssemblyProductAttribute)Attribs[0]).Product : String.Empty;
            }
        }

        /// <summary>
        /// Get application version from the resource section of calling assembly.
        /// </summary>
        public static string AppVersion => Assembly.GetCallingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Get application developer name from the resource section of calling assembly.
        /// </summary>
        public static string AppCompany
        {
            get
            {
                object[] Attribs = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return Attribs.Length != 0 ? ((AssemblyCompanyAttribute)Attribs[0]).Company : String.Empty;
            }
        }

        /// <summary>
        /// Get application copyright from the resource section of calling assembly.
        /// </summary>
        public static string AppCopyright
        {
            get
            {
                object[] Attribs = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return Attribs.Length != 0 ? ((AssemblyCopyrightAttribute)Attribs[0]).Copyright : String.Empty;
            }
        }

        /// <summary>
        /// Checks if current date belongs to the New Year eve.
        /// </summary>
        public static bool IsNewYear
        {
            get
            {
                DateTime XDate = DateTime.Now;
                return (XDate.Month == 12 && XDate.Day >= 20 && XDate.Day <= 31) || (XDate.Month == 1 && XDate.Day >= 1 && XDate.Day <= 10);
            }
        }

        /// <summary>
        /// CurrentApp class constructor.
        /// </summary>
        /// <param name="IsPortable">Enable portable mode (with settings in the same directory as executable).</param>
        /// <param name="AppName">Application name.</param>
        public CurrentApp(bool IsPortable, string AppName)
        {
            // Getting information about operating system and platform...
            Platform = new CurrentPlatform();

            // Getting full path to application installation directory...
            FullAppPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            // Getting full to application user directory...
            AppUserDir = IsPortable ? Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "portable") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName); ;

            // Checking if user directory exists. If not - creating it...
            if (!(Directory.Exists(AppUserDir)))
            {
                Directory.CreateDirectory(AppUserDir);
            }

            // Generating User-Agent header for outgoing HTTP queries...
            UserAgent = String.Format(Properties.Resources.AppDefUA, Platform.OSFriendlyName, Platform.UASuffix, Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor, CultureInfo.CurrentCulture.Name, AppVersion, AppName, SystemArch);
        }
    }
}
