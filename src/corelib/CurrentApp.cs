﻿/**
 * SPDX-FileCopyrightText: 2011-2025 EasyCoding Team
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
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
        /// Get full path to the active log file.
        /// </summary>
        public string AppLogFile { get; private set; }

        /// <summary>
        /// Get full path to the local FPS-configs directory.
        /// </summary>
        public string AppCfgDir { get; private set; }

        /// <summary>
        /// Get full path to the local HUDs directory.
        /// </summary>
        public string AppHUDDir { get; private set; }

        /// <summary>
        /// Get full path to the local logs directory.
        /// </summary>
        public string AppLogDir { get; private set; }

        /// <summary>
        /// Get full path to the local reports directory.
        /// </summary>
        public string AppReportDir { get; private set; }

        /// <summary>
        /// Get full path to the local updates directory.
        /// </summary>
        public string AppUpdateDir { get; private set; }

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
        /// Get the application name from the resource section of the entry assembly.
        /// </summary>
        public static string AppProduct
        {
            get
            {
                object[] Attribs = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return Attribs.Length > 0 ? ((AssemblyProductAttribute)Attribs[0]).Product : string.Empty;
            }
        }

        /// <summary>
        /// Get the application version from the resource section of the entry assembly.
        /// </summary>
        public static Version AppVersion => Assembly.GetEntryAssembly().GetName().Version;

        /// <summary>
        /// Get the library version from the resource section of the executing assembly.
        /// </summary>
        public static Version LibVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Get the application developer name from the resource section of the entry assembly.
        /// </summary>
        public static string AppCompany
        {
            get
            {
                object[] Attribs = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return Attribs.Length > 0 ? ((AssemblyCompanyAttribute)Attribs[0]).Company : string.Empty;
            }
        }

        /// <summary>
        /// Get the application copyright from the resource section of the entry assembly.
        /// </summary>
        public static string AppCopyright
        {
            get
            {
                object[] Attribs = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return Attribs.Length > 0 ? ((AssemblyCopyrightAttribute)Attribs[0]).Copyright : string.Empty;
            }
        }

        /// <summary>
        /// Get the full path to the application executable file.
        /// </summary>
        public static string AppLocation => Assembly.GetEntryAssembly().Location;

        /// <summary>
        /// Get the full path to the active application's log file.
        /// </summary>
        /// <returns>Full path to the active log file.</returns>
        private string GetLogFileName()
        {
            try
            {
                NLog.Targets.FileTarget LogTarget = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("logfile");
                return Path.GetFullPath(LogTarget.FileName.Render(new LogEventInfo()));
            }
            catch
            {
                return Path.Combine(AppUserDir, Properties.Resources.LogLocalDir, Properties.Resources.LogMainFile);
            }
        }

        /// <summary>
        /// Get the full path to the configured application's log directory.
        /// </summary>
        /// <returns>Full path to the configured application's log directory.</returns>
        private string GetLogDirPath()
        {
            try
            {
                return Path.GetFullPath(LogManager.Configuration.Variables["logdir"].Render(new LogEventInfo()));
            }
            catch
            {
                return Path.Combine(AppUserDir, Properties.Resources.LogLocalDir);
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
            Platform = CurrentPlatform.Create();

            // Getting full path to application installation directory...
            FullAppPath = Path.GetDirectoryName(AppLocation);

            // Getting full to application user directory...
            AppUserDir = IsPortable ? Path.Combine(FullAppPath, Properties.Resources.PortableLocalDir) : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

            // Getting full paths to local application directories...
            AppCfgDir = Path.Combine(AppUserDir, Properties.Resources.CfgLocalDir);
            AppHUDDir = Path.Combine(AppUserDir, Properties.Resources.HUDLocalDir);
            AppReportDir = Path.Combine(AppUserDir, Properties.Resources.ReportLocalDir);
            AppUpdateDir = Path.Combine(AppUserDir, Properties.Resources.UpdateLocalDir);

            // Gettings full paths to local application logs...
            AppLogFile = GetLogFileName();
            AppLogDir = GetLogDirPath();

            // Checking if user directory exists. If not - creating it...
            if (!Directory.Exists(AppUserDir))
            {
                Directory.CreateDirectory(AppUserDir);
            }

            // Generating User-Agent header for outgoing HTTP queries...
            UserAgent = string.Format(Properties.Resources.AppUserAgentTemplate, Platform.OSFriendlyName, Platform.OSVersion, Platform.OSArchitecture, CultureInfo.CurrentCulture.Name, AppName, AppVersion);
        }
    }
}
