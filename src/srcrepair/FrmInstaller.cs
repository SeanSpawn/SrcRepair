﻿/**
 * SPDX-FileCopyrightText: 2011-2025 EasyCoding Team
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
*/

using System;
using System.IO;
using System.Windows.Forms;
using NLog;

namespace srcrepair.gui
{
    /// <summary>
    /// Class of the Quick add-on installer module.
    /// </summary>
    public partial class FrmInstaller : Form
    {
        /// <summary>
        /// Logger instance for the FrmInstaller class.
        /// </summary>
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Stores the full path to the game installation directory.
        /// </summary>
        private readonly string FullGamePath;

        /// <summary>
        /// Stores the full path to a special directory for storing
        /// custom user content.
        /// </summary>
        private readonly string CustomInstallDir;

        /// <summary>
        /// Stores the full path to the installation directory.
        /// </summary>
        private readonly string UserInstallDir;

        /// <summary>
        /// FrmInstaller class constructor.
        /// </summary>
        /// <param name="GameDir">Full path to the game installation directory.</param>
        /// <param name="UseCustomDir">If current game is using a special directory for storing custom user content.</param>
        /// <param name="CustomDir">Full path to the custom user content directory.</param>
        public FrmInstaller(string GameDir, bool UseCustomDir, string CustomDir)
        {
            InitializeComponent();
            FullGamePath = GameDir;
            CustomInstallDir = CustomDir;
            UserInstallDir = UseCustomDir ? Path.Combine(CustomDir, Properties.Settings.Default.UserCustDirName) : GameDir;
        }

        /// <summary>
        /// Generates a configuration file from a template.
        /// </summary>
        /// <param name="FileName">Full path to the target configuration file.</param>
        /// <param name="Template">Configuration file template as a string.</param>
        private void GenerateConfigFromTemplate(string FileName, string Template)
        {
            try
            {
                using (StreamWriter CFile = new StreamWriter(FileName))
                {
                    CFile.Write(Template.Replace("{D}", Path.GetFileNameWithoutExtension(FileName)));
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExQIGenerateConfig);
                MessageBox.Show(AppStrings.QI_GenerateConfigError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Installs a custom content file into the game.
        /// </summary>
        /// <param name="FileName">Full path to the source file.</param>
        /// <param name="DestDir">Full path to the destination directory.</param>
        private void InstallFileNow(string FileName, string DestDir)
        {
            // Checking if the destination directory exists...
            if (!Directory.Exists(DestDir)) { Directory.CreateDirectory(DestDir); }

            // Copying source file to the destination directory...
            File.Copy(FileName, Path.Combine(DestDir, Path.GetFileName(FileName)), true);
        }

        /// <summary>
        /// Installs custom content into the game that requires a special
        /// configuration file.
        /// </summary>
        /// <param name="FileName">Full path to the custom content file.</param>
        /// <param name="DestDir">Destination directory.</param>
        /// <param name="ConfigExtension">Configuration file extension.</param>
        /// <param name="ConfigTemplate">Configuration file template as a string.</param>
        private void InstallWithConfigNow(string FileName, string DestDir, string ConfigExtension, string ConfigTemplate)
        {
            // Generating full path to the configuration file...
            string ConfigFile = Path.Combine(Path.GetDirectoryName(FileName), Path.ChangeExtension(Path.GetFileName(FileName), ConfigExtension));

            // Checking if the configuration file exists...
            if (!File.Exists(ConfigFile) && MessageBox.Show(AppStrings.QI_GenerateConfigQuestion, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GenerateConfigFromTemplate(ConfigFile, ConfigTemplate);
            }

            // Installing the requested file...
            InstallFileNow(FileName, DestDir);

            // Installing the configuration file...
            if (File.Exists(ConfigFile))
            {
                InstallFileNow(ConfigFile, DestDir);
            }
        }

        /// <summary>
        /// Installs a custom plugin into the game.
        /// </summary>
        /// <param name="FileName">Full path to the source plugin file.</param>
        private void InstallPluginNow(string FileName)
        {
            InstallWithConfigNow(FileName, Path.Combine(UserInstallDir, "addons"), ".vdf", Properties.Resources.TemplatePlugin);
        }

        /// <summary>
        /// Installs a custom spray into the game.
        /// </summary>
        /// <param name="FileName">Full path to the source spray file.</param>
        private void InstallSprayNow(string FileName)
        {
            InstallWithConfigNow(FileName, Path.Combine(FullGamePath, "materials", "vgui", "logos"), ".vmt", Properties.Resources.TemplateSpray);
        }

        /// <summary>
        /// Installs a custom content into the game.
        /// </summary>
        /// <param name="FileName">Full path to the custom content file.</param>
        private void InstallContent(string FileName)
        {
            // Using different methods, based on source file extension...
            switch (Path.GetExtension(FileName))
            {
                case ".dem": // Installing demo file...
                    InstallFileNow(FileName, FullGamePath);
                    break;
                case ".vpk": // Installing VPK package...
                    InstallFileNow(FileName, CustomInstallDir);
                    break;
                case ".cfg": // Installing game config...
                    InstallFileNow(FileName, Path.Combine(UserInstallDir, "cfg"));
                    break;
                case ".bsp": // Installing map...
                    InstallFileNow(FileName, Path.Combine(UserInstallDir, "maps"));
                    break;
                case ".wav": // Installing hitsound...
                    InstallFileNow(FileName, Path.Combine(UserInstallDir, "sound", "ui"));
                    break;
                case ".vtf": // Installing spray...
                    InstallSprayNow(FileName);
                    break;
                case ".zip": // Installing contents of Zip archive...
                    GuiHelpers.FormShowArchiveExtract(FileName, CustomInstallDir);
                    break;
                case ".dll": // Installing binary plugin...
                    InstallPluginNow(FileName);
                    break;
                default: // Unknown file type...
                    throw new NotImplementedException(DebugStrings.AppDbgQIUnknownFileType);
            }
        }

        /// <summary>
        /// "Browse" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void QI_Browse_Click(object sender, EventArgs e)
        {
            if (QI_OpenFile.ShowDialog() == DialogResult.OK)
            {
                QI_InstallPath.Text = QI_OpenFile.FileName;
            }
        }

        /// <summary>
        /// "Install" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void QI_Install_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(QI_InstallPath.Text))
            {
                MessageBox.Show(AppStrings.QI_FileNotSelected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                InstallContent(QI_InstallPath.Text);
                MessageBox.Show(AppStrings.QI_InstallationSuccessful, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExQIBaseInstall);
                MessageBox.Show(AppStrings.QI_InstallationError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
