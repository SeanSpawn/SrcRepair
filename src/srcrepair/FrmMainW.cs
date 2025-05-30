﻿/**
 * SPDX-FileCopyrightText: 2011-2025 EasyCoding Team
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using srcrepair.core;

namespace srcrepair.gui
{
    /// <summary>
    /// Class of main form.
    /// </summary>
    public partial class FrmMainW : Form
    {
        /// <summary>
        /// Logger instance for FrmMainW class.
        /// </summary>
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ResourceManager instance for managing Cvar descriptions in Config Editor.
        /// </summary>
        private readonly ResourceManager CvarFetcher = new ResourceManager(Properties.Resources.CE_CVResDf, typeof(FrmMainW).Assembly);

        /// <summary>
        /// Stores loaded in Config Editor file name.
        /// </summary>
        private string CFGFileName;

        /// <summary>
        /// Stores an instance of the CurrentApp class.
        /// </summary>
        private CurrentApp App;

        /// <summary>
        /// FrmMainW class constructor.
        /// </summary>
        public FrmMainW()
        {
            // Initializing controls...
            InitializeComponent();
        }

        /// <summary>
        /// Scales controls on current form with some additional hacks applied.
        /// </summary>
        /// <param name="ScalingFactor">Scaling factor.</param>
        /// <param name="ControlBounds">The bounds of the control.</param>
        protected override void ScaleControl(SizeF ScalingFactor, BoundsSpecified ControlBounds)
        {
            base.ScaleControl(ScalingFactor, ControlBounds);
            if (!DpiManager.CompareFloats(Math.Max(ScalingFactor.Width, ScalingFactor.Height), 1.0f))
            {
                DpiManager.ScaleColumnsInControl(BU_LVTable, ScalingFactor);
                DpiManager.ScaleColumnsInControl(StatusBar, ScalingFactor);
            }
        }

        /// <summary>
        /// Overrides system cryptographic policies.
        /// </summary>
        private void ConfigureCryptoPolicy()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCryptoPolicy);
            }
        }

        /// <summary>
        /// Seaching for installed games and adds them to ComboBox control
        /// of main window.
        /// </summary>
        private void DetectInstalledGames()
        {
            try
            {
                // Reading game database...
                App.SourceGames = new GameManager(App, Properties.Settings.Default.HideUnsupportedGames);

                // Clearing all existing items...
                AppSelector.Items.Clear();

                // Adding found games to selector...
                AppSelector.Items.AddRange(App.SourceGames.InstalledGameNames.ToArray());
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExDetectInstalledGames);
            }
        }

        /// <summary>
        /// Updates video settings of Type 1 game.
        /// </summary>
        private void UpdateType1VideoSettings(Type1Video Video)
        {
            Video.ScreenWidth = (int)GT_ResHor.Value;
            Video.ScreenHeight = (int)GT_ResVert.Value;
            Video.DisplayMode = GT_ScreenType.SelectedIndex;
            Video.ModelQuality = GT_ModelQuality.SelectedIndex;
            Video.TextureQuality = GT_TextureQuality.SelectedIndex;
            Video.ShaderQuality = GT_ShaderQuality.SelectedIndex;
            Video.ReflectionsQuality = GT_WaterQuality.SelectedIndex;
            Video.ShadowQuality = GT_ShadowQuality.SelectedIndex;
            Video.ColorCorrection = GT_ColorCorrectionT.SelectedIndex;
            Video.AntiAliasing = GT_AntiAliasing.SelectedIndex;
            Video.FilteringMode = GT_Filtering.SelectedIndex;
            Video.VSync = GT_VSync.SelectedIndex;
            Video.MotionBlur = GT_MotionBlur.SelectedIndex;
            Video.DirectXMode = GT_DxMode.SelectedIndex;
            Video.HDRType = GT_HDR.SelectedIndex;
        }

        /// <summary>
        /// Updates video settings of Type 2 game.
        /// </summary>
        private void UpdateType2VideoSettings(Type2Video Video)
        {
            Video.ScreenWidth = (int)GT_NCF_HorRes.Value;
            Video.ScreenHeight = (int)GT_NCF_VertRes.Value;
            Video.ScreenRatio = GT_NCF_Ratio.SelectedIndex;
            Video.ScreenGamma = GT_NCF_Brightness.Text;
            Video.ShadowQuality = GT_NCF_Shadows.SelectedIndex;
            Video.MotionBlur = GT_NCF_MBlur.SelectedIndex;
            Video.ScreenMode = GT_NCF_DispMode.SelectedIndex;
            Video.AntiAliasing = GT_NCF_AntiAlias.SelectedIndex;
            Video.FilteringMode = GT_NCF_Filtering.SelectedIndex;
            Video.VSync = GT_NCF_VSync.SelectedIndex;
            Video.RenderingMode = GT_NCF_Multicore.SelectedIndex;
            Video.ShaderEffects = GT_NCF_ShaderE.SelectedIndex;
            Video.Effects = GT_NCF_EffectD.SelectedIndex;
            Video.MemoryPool = GT_NCF_MemPool.SelectedIndex;
            Video.ModelQuality = GT_NCF_Quality.SelectedIndex;
        }

        /// <summary>
        /// Updates video settings of Type 4 game.
        /// </summary>
        private void UpdateType4VideoSettings(Type4Video Video)
        {
            Video.ScreenWidth = (int)GT_ResHor.Value;
            Video.ScreenHeight = (int)GT_ResVert.Value;
            Video.DisplayMode = GT_ScreenType.SelectedIndex;
            Video.ModelQuality = GT_ModelQuality.SelectedIndex;
            Video.TextureQuality = GT_TextureQuality.SelectedIndex;
            Video.ShaderQuality = GT_ShaderQuality.SelectedIndex;
            Video.ReflectionsQuality = GT_WaterQuality.SelectedIndex;
            Video.ShadowQuality = GT_ShadowQuality.SelectedIndex;
            Video.ColorCorrection = GT_ColorCorrectionT.SelectedIndex;
            Video.AntiAliasing = GT_AntiAliasing.SelectedIndex;
            Video.FilteringMode = GT_Filtering.SelectedIndex;
            Video.VSync = GT_VSync.SelectedIndex;
            Video.MotionBlur = GT_MotionBlur.SelectedIndex;
            Video.DirectXMode = GT_DxMode.SelectedIndex;
            Video.HDRType = GT_HDR.SelectedIndex;
        }

        /// <summary>
        /// Fetches video settings of Type 1 game.
        /// </summary>
        private void ReadType1VideoSettings(Type1Video Video)
        {
            GT_ResHor.Value = Video.ScreenWidth;
            GT_ResVert.Value = Video.ScreenHeight;
            GT_ScreenType.SelectedIndex = Video.DisplayMode;
            GT_ModelQuality.SelectedIndex = Video.ModelQuality;
            GT_TextureQuality.SelectedIndex = Video.TextureQuality;
            GT_ShaderQuality.SelectedIndex = Video.ShaderQuality;
            GT_WaterQuality.SelectedIndex = Video.ReflectionsQuality;
            GT_ShadowQuality.SelectedIndex = Video.ShadowQuality;
            GT_ColorCorrectionT.SelectedIndex = Video.ColorCorrection;
            GT_AntiAliasing.SelectedIndex = Video.AntiAliasing;
            GT_Filtering.SelectedIndex = Video.FilteringMode;
            GT_VSync.SelectedIndex = Video.VSync;
            GT_MotionBlur.SelectedIndex = Video.MotionBlur;
            GT_DxMode.SelectedIndex = Video.DirectXMode;
            GT_HDR.SelectedIndex = Video.HDRType;
        }

        /// <summary>
        /// Fetches video settings of Type 2 game.
        /// </summary>
        private void ReadType2VideoSettings(Type2Video Video)
        {
            GT_NCF_HorRes.Value = Video.ScreenWidth;
            GT_NCF_VertRes.Value = Video.ScreenHeight;
            GT_NCF_Ratio.SelectedIndex = Video.ScreenRatio;
            GT_NCF_Brightness.Text = Video.ScreenGamma;
            GT_NCF_Shadows.SelectedIndex = Video.ShadowQuality;
            GT_NCF_MBlur.SelectedIndex = Video.MotionBlur;
            GT_NCF_DispMode.SelectedIndex = Video.ScreenMode;
            GT_NCF_AntiAlias.SelectedIndex = Video.AntiAliasing;
            GT_NCF_Filtering.SelectedIndex = Video.FilteringMode;
            GT_NCF_VSync.SelectedIndex = Video.VSync;
            GT_NCF_Multicore.SelectedIndex = Video.RenderingMode;
            GT_NCF_ShaderE.SelectedIndex = Video.ShaderEffects;
            GT_NCF_EffectD.SelectedIndex = Video.Effects;
            GT_NCF_MemPool.SelectedIndex = Video.MemoryPool;
            GT_NCF_Quality.SelectedIndex = Video.ModelQuality;
        }

        /// <summary>
        /// Fetches video settings of Type 4 game.
        /// </summary>
        private void ReadType4VideoSettings(Type4Video Video)
        {
            GT_ResHor.Value = Video.ScreenWidth;
            GT_ResVert.Value = Video.ScreenHeight;
            GT_ScreenType.SelectedIndex = Video.DisplayMode;
            GT_ModelQuality.SelectedIndex = Video.ModelQuality;
            GT_TextureQuality.SelectedIndex = Video.TextureQuality;
            GT_ShaderQuality.SelectedIndex = Video.ShaderQuality;
            GT_WaterQuality.SelectedIndex = Video.ReflectionsQuality;
            GT_ShadowQuality.SelectedIndex = Video.ShadowQuality;
            GT_ColorCorrectionT.SelectedIndex = Video.ColorCorrection;
            GT_AntiAliasing.SelectedIndex = Video.AntiAliasing;
            GT_Filtering.SelectedIndex = Video.FilteringMode;
            GT_VSync.SelectedIndex = Video.VSync;
            GT_MotionBlur.SelectedIndex = Video.MotionBlur;
            GT_DxMode.SelectedIndex = Video.DirectXMode;
            GT_HDR.SelectedIndex = Video.HDRType;
        }

        /// <summary>
        /// Checks if the user is trying to edit config.cfg file and show
        /// a warning message.
        /// </summary>
        private void CheckGameConfigEditor()
        {
            if (CFGFileName.EndsWith("config.cfg"))
            {
                MessageBox.Show(AppStrings.CE_RestConfigOpenWarn, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Gets user-friendly names for backup files.
        /// </summary>
        /// <param name="FileName">Backup file name.</param>
        /// <returns>Returns tuple with backup type and its friendly name.</returns>
        private Tuple<string, string> GenUserFriendlyBackupDesc(FileInfo FileName)
        {
            string ConfRow, ConfType;
            switch (FileName.Name.Substring(0, FileName.Name.LastIndexOf('_')))
            {
                case "Container":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_Bud, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Cont;
                    break;
                case "Config":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_Config, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Cfg;
                    break;
                case "VoiceBan":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_VChat, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_DB;
                    break;
                case "VideoCfg":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_GRGame, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Video;
                    break;
                case "VideoAutoCfg":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_GameAuto, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Video;
                    break;
                case "Game_Options":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_GRGame, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Reg;
                    break;
                case "Source_Options":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_SRCAll, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Reg;
                    break;
                case "Steam_BackUp":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_SteamAll, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Reg;
                    break;
                case "Game_AutoBackUp":
                    ConfRow = string.Format(Properties.Resources.BU_TablePrefix, AppStrings.BU_BName_GameAuto, FileName.CreationTime);
                    ConfType = AppStrings.BU_BType_Reg;
                    break;
                default:
                    ConfRow = Path.GetFileNameWithoutExtension(FileName.Name);
                    ConfType = AppStrings.BU_BType_Unkn;
                    break;
            }
            return Tuple.Create(ConfType, ConfRow);
        }

        /// <summary>
        /// Resets controls of Type 1 video settings on Graphic Tweaker
        /// to default values.
        /// </summary>
        private void NullType1Settings()
        {
            GT_ResHor.Value = 640;
            GT_ResVert.Value = 640;
            GT_ScreenType.SelectedIndex = -1;
            GT_ModelQuality.SelectedIndex = -1;
            GT_TextureQuality.SelectedIndex = -1;
            GT_ShaderQuality.SelectedIndex = -1;
            GT_WaterQuality.SelectedIndex = -1;
            GT_ShadowQuality.SelectedIndex = -1;
            GT_ColorCorrectionT.SelectedIndex = -1;
            GT_AntiAliasing.SelectedIndex = -1;
            GT_Filtering.SelectedIndex = -1;
            GT_VSync.SelectedIndex = -1;
            GT_MotionBlur.SelectedIndex = -1;
            GT_DxMode.SelectedIndex = -1;
            GT_HDR.SelectedIndex = -1;
        }

        /// <summary>
        /// Resets controls of Type 2 video settings on Graphic Tweaker
        /// to default values.
        /// </summary>
        private void NullType2Settings()
        {
            GT_NCF_HorRes.Value = 640;
            GT_NCF_VertRes.Value = 480;
            GT_NCF_Brightness.SelectedIndex = -1;
            GT_NCF_Shadows.SelectedIndex = -1;
            GT_NCF_MBlur.SelectedIndex = -1;
            GT_NCF_Ratio.SelectedIndex = -1;
            GT_NCF_DispMode.SelectedIndex = -1;
            GT_NCF_AntiAlias.SelectedIndex = -1;
            GT_NCF_Filtering.SelectedIndex = -1;
            GT_NCF_VSync.SelectedIndex = -1;
            GT_NCF_Multicore.SelectedIndex = -1;
            GT_NCF_ShaderE.SelectedIndex = -1;
            GT_NCF_EffectD.SelectedIndex = -1;
            GT_NCF_MemPool.SelectedIndex = -1;
            GT_NCF_Quality.SelectedIndex = -1;
        }

        /// <summary>
        /// Resets controls of video settings on Graphic Tweaker
        /// to default values.
        /// </summary>
        private void NullGraphSettings()
        {
            NullType1Settings();
            NullType2Settings();
        }

        /// <summary>
        /// Changes the state of some controls on form.
        /// </summary>
        private void HandleControlsOnSelGame()
        {
            // Enable main tab selector...
            MainTabControl.Enabled = true;

            // Clearing lists...
            FP_ConfigSel.Items.Clear();
            HD_HSel.Items.Clear();

            // Disable controls on FPS-config tab...
            SetFPSButtons(false);
            FP_Install.Enabled = false;
            FP_Comp.Visible = false;

            // Disable controls on HUD Manager tab...
            SetHUDButtons(false);
            HD_Install.Enabled = false;
            HD_Homepage.Enabled = false;
            HD_Warning.Visible = false;
            HD_GB_Pbx.Image = null;
            HD_LastUpdate.Visible = false;

            // Enable custom installer menu element...
            MNUInstaller.Enabled = true;
        }

        /// <summary>
        /// Set video settings on form.
        /// </summary>
        private void UpdateVideoFormControl()
        {
            switch (App.SourceGames[AppSelector.Text].SourceType)
            {
                case 1:
                    ReadType1VideoSettings((Type1Video)App.SourceGames[AppSelector.Text].Video);
                    break;
                case 2:
                    ReadType2VideoSettings((Type2Video)App.SourceGames[AppSelector.Text].Video);
                    break;
                case 4:
                    ReadType4VideoSettings((Type4Video)App.SourceGames[AppSelector.Text].Video);
                    break;
                default:
                    Logger.Warn(DebugStrings.AppDbgIncorrectSourceType);
                    break;
            }
        }

        /// <summary>
        /// Loads video settings of selected game.
        /// </summary>
        private void LoadGraphicSettings()
        {
            NullGraphSettings();
            App.SourceGames[AppSelector.Text].Video.ReadSettings();
            UpdateVideoFormControl();
            SelectGraphicWidget();
        }

        /// <summary>
        /// Creates a backup of the game video settings.
        /// </summary>
        /// <param name="IsManual">Determines whether the backup was initiated by the user or not.</param>
        private void VideoSettingsBackup(bool IsManual)
        {
            try
            {
                if (Properties.Settings.Default.SafeCleanup || IsManual)
                {
                    App.SourceGames[AppSelector.Text].Video.BackUpSettings(App.SourceGames[AppSelector.Text].FullBackUpDirPath, IsManual);
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExVideoBackUpFail);
            }
        }

        /// <summary>
        /// Removes game video settings.
        /// </summary>
        private void VideoSettingsRemove()
        {
            try
            {
                App.SourceGames[AppSelector.Text].Video.RemoveSettings();
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExVideoRemoveFail);
            }
        }

        /// <summary>
        /// Performes backup and then writes video settings of Type 1 game,
        /// selected in main window.
        /// </summary>
        private void WriteType1VideoSettings()
        {
            UpdateType1VideoSettings((Type1Video)App.SourceGames[AppSelector.Text].Video);
        }

        /// <summary>
        /// Performes backup and then writes video settings of Type 2 game,
        /// selected in main window.
        /// </summary>
        private void WriteType2VideoSettings()
        {
            UpdateType2VideoSettings((Type2Video)App.SourceGames[AppSelector.Text].Video);
        }

        /// <summary>
        /// Performes backup and then writes video settings of Type 4 game,
        /// selected in main window.
        /// </summary>
        private void WriteType4VideoSettings()
        {
            UpdateType4VideoSettings((Type4Video)App.SourceGames[AppSelector.Text].Video);
        }

        /// <summary>
        /// Resets controls of video settings on Graphic Tweaker
        /// to default values.
        /// </summary>
        private void WriteGraphicSettings()
        {
            VideoSettingsBackup(false);
            switch (App.SourceGames[AppSelector.Text].SourceType)
            {
                case 1:
                    WriteType1VideoSettings();
                    break;
                case 2:
                    WriteType2VideoSettings();
                    break;
                case 4:
                    WriteType4VideoSettings();
                    break;
                default:
                    Logger.Warn(DebugStrings.AppDbgIncorrectSourceType);
                    break;
            }
            App.SourceGames[AppSelector.Text].Video.WriteSettings();
        }

        /// <summary>
        /// Switches between different views of Graphic Tweaker.
        /// </summary>
        private void SelectGraphicWidget()
        {
            switch (App.SourceGames[AppSelector.Text].SourceType)
            {
                case 1:
                case 4:
                    GT_GType1.Visible = true;
                    GT_GType2.Visible = false;
                    break;
                case 2:
                    GT_GType1.Visible = false;
                    GT_GType2.Visible = true;
                    break;
                default:
                    Logger.Warn(DebugStrings.AppDbgIncorrectSourceType);
                    break;
            }
        }

        /// <summary>
        /// Shows current Safe Clean status on form.
        /// </summary>
        private void CheckSafeClnStatus()
        {
            if (Properties.Settings.Default.SafeCleanup)
            {
                SB_App.Text = AppStrings.AppSafeClnStTextOn;
                SB_App.Image = Properties.Resources.IconGreenCircle;
            }
            else
            {
                SB_App.Text = AppStrings.AppSafeClnStTextOff;
                SB_App.Image = Properties.Resources.IconRedCircle;
            }
        }

        /// <summary>
        /// Returns manually entered by user path to installed Steam client.
        /// </summary>
        /// <returns>Path to installed Steam client.</returns>
        private string GetPathByMEnter()
        {
            string Result;

            if (FldrBrwse.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(Path.Combine(FldrBrwse.SelectedPath, App.Platform.SteamBinaryName)))
                {
                    throw new FileNotFoundException(AppStrings.AppSteamPathEnterInvalid, Path.Combine(FldrBrwse.SelectedPath, App.Platform.SteamBinaryName));
                }
                else
                {
                    Result = FldrBrwse.SelectedPath;
                }
            }
            else
            {
                throw new OperationCanceledException(AppStrings.AppSteamPathEnterWinClosed);
            }

            return Result;
        }

        /// <summary>
        /// Checks if source string contains valid path to Steam client
        /// installation directory. If not, opens special dialog.
        /// </summary>
        /// <param name="OldPath">Valid path to Steam client installation directory.</param>
        private string CheckLastSteamPath(string OldPath)
        {
            return (!string.IsNullOrWhiteSpace(OldPath) && File.Exists(Path.Combine(OldPath, App.Platform.SteamBinaryName))) ? OldPath : GetPathByMEnter();
        }

        /// <summary>
        /// Tries to find Steam client installation directory.
        /// </summary>
        private void ValidateAndHandle()
        {
            try
            {
                App.SteamClient = new SteamManager(CheckLastSteamPath(Properties.Settings.Default.LastSteamPath), Properties.Settings.Default.LastSteamID);
            }
            catch (FileNotFoundException Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExSteamPath);
                MessageBox.Show(AppStrings.SteamPathEnterErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(ReturnCodes.StmWrongPath);
            }
            catch (OperationCanceledException Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExSteamPath);
                MessageBox.Show(AppStrings.SteamPathCancel, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(ReturnCodes.StmPathCancel);
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExSteamPath);
                MessageBox.Show(AppStrings.AppGenericError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(ReturnCodes.StmPathException);
            }
        }

        /// <summary>
        /// Restores a backup file depending on its extension.
        /// </summary>
        /// <returns>Result of the restore.</returns>
        private bool RestoreBackUpFile(string FileName)
        {
            try
            {
                switch (Path.GetExtension(FileName))
                {
                    case ".reg": // Registry file...
                        App.Platform.RestoreRegistrySettings(Path.Combine(App.SourceGames[AppSelector.Text].FullBackUpDirPath, FileName));
                        break;
                    case ".bud": // Standard archive...
                        GuiHelpers.FormShowArchiveExtract(Path.Combine(App.SourceGames[AppSelector.Text].FullBackUpDirPath, FileName), Path.GetPathRoot(App.SourceGames[AppSelector.Text].FullGamePath));
                        HandleConfigs();
                        break;
                    default: // Unknown type...
                        MessageBox.Show(AppStrings.BU_UnknownType, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                }
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExBackUpRestore);
                MessageBox.Show(AppStrings.BU_RestFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Restores selected backup files.
        /// </summary>
        private void RestoreSelectedBackUps()
        {
            bool RestoreStatus = true;

            foreach (ListViewItem BU_Item in BU_LVTable.SelectedItems)
            {
                RestoreStatus &= RestoreBackUpFile(BU_Item.SubItems[4].Text);
            }

            if (RestoreStatus)
            {
                MessageBox.Show(AppStrings.BU_RestSuccessful, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Creates instances of CurrentApp and SteamManager classes.
        /// </summary>
        private void InitializeApp()
        {
            // Create a new instance of CurrentApp class...
            App = new CurrentApp(Properties.Settings.Default.IsPortable, Properties.Resources.AppName);

            // Create a new instance of SteamManager class and take care of possible errors...
            try
            {
                App.SteamClient = new SteamManager(Properties.Settings.Default.LastSteamID, App.Platform);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show(AppStrings.AppNoSteamIDSetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(ReturnCodes.NoUserIdsDetected);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExStmmInit);
                ValidateAndHandle();
            }
        }

        /// <summary>
        /// Find installed plugins.
        /// </summary>
        private async void FindPlugins()
        {
            try
            {
                await FindPluginsTask();
                RegisterPlugins();
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExPluginsInit);
            }
        }

        /// <summary>
        /// Register installed plugins.
        /// </summary>
        private void RegisterPlugins()
        {
            try
            {
                if (App.Platform.AdvancedFeaturesSupported)
                {
                    MNUWinMnuDisabler.Enabled = App.Plugins["kbhelper"].Installed;
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExPluginRegister);
            }
        }

        /// <summary>
        /// Sets strings data for main form.
        /// </summary>
        private void SetAppStrings()
        {
            // Save last Steam path in application's settings...
            Properties.Settings.Default.LastSteamPath = App.SteamClient.FullSteamPath;

            // Add Steam client installation path to Troubleshooting page...
            PS_StPath.Text = string.Format(PS_StPath.Text, App.SteamClient.FullSteamPath);
        }

        /// <summary>
        /// Changes state of some controls, depending on current running
        /// platform or access level.
        /// </summary>
        private void ChangePrvControlState()
        {
            // Checking platform...
            if (!App.Platform.AdvancedFeaturesSupported)
            {
                // On Linux and MacOS we will disable some modules and features...
                MNUReportBuilder.Enabled = false;
                MNUWinMnuDisabler.Enabled = false;
                PS_CleanRegistry.Enabled = false;
                PS_ServiceRepair.Enabled = false;
                BUT_RegSettings.Enabled = false;
            }
        }

        /// <summary>
        /// Checks file system name on game installation drive and shows this on form.
        /// </summary>
        private void DetectFS()
        {
            try
            {
                PS_OSDrive.Text = string.Format(PS_OSDrive.Text, FileManager.DetectDriveFileSystem(Path.GetPathRoot(App.SourceGames[AppSelector.Text].FullGamePath)));
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExDetectFileSystem);
                PS_OSDrive.Text = string.Format(PS_OSDrive.Text, "Unknown");
            }
        }

        /// <summary>
        /// Checks the number of found supported games and performes some actions.
        /// </summary>
        /// <param name="GamesCount">The number of found supported games.</param>
        private void CheckGames(int GamesCount)
        {
            switch (GamesCount)
            {
                case 0:
                    Logger.Warn(AppStrings.AppNoGamesDLog, App.SteamClient.FullSteamPath);
                    MessageBox.Show(AppStrings.AppNoGamesDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Environment.Exit(ReturnCodes.NoGamesDetected);
                    break;
                case 1:
                    AppSelector.SelectedIndex = 0;
                    UpdateStatusBar();
                    break;
                default:
                    int Ai = AppSelector.Items.IndexOf(Properties.Settings.Default.LastGameName);
                    AppSelector.SelectedIndex = Ai != -1 ? Ai : 0;
                    break;
            }
        }

        /// <summary>
        /// Checks for restricted symbols in Steam installation path and shows
        /// result on form.
        /// </summary>
        private void CheckSymbolsSteam()
        {
            if (!FileManager.CheckNonASCII(App.SteamClient.FullSteamPath))
            {
                Logger.Warn(AppStrings.AppRestrSymbLog, App.SteamClient.FullSteamPath);
                PS_PathSteam.ForeColor = Color.Red;
                PS_PathSteam.Image = Properties.Resources.IconUpdateError;
            }
        }

        /// <summary>
        /// Checks for restricted symbols in selected game installation path
        /// and shows result on form.
        /// </summary>
        private void CheckSymbolsGame()
        {
            if (!FileManager.CheckNonASCII(App.SourceGames[AppSelector.Text].FullGamePath))
            {
                Logger.Warn(AppStrings.AppRestrSymbLog, App.SourceGames[AppSelector.Text].FullGamePath);
                PS_PathGame.ForeColor = Color.Red;
                PS_PathGame.Image = Properties.Resources.IconUpdateError;
            }
            else
            {
                PS_PathGame.ForeColor = Color.Green;
                PS_PathGame.Image = Properties.Resources.IconUpdateNotAvailable;
            }
        }

        /// <summary>
        /// Get the selection status of repair procedures.
        /// </summary>
        /// <returns>Returns True if at least one repair procedure is selected.</returns>
        private bool GetRepairSelState()
        {
            return PS_CleanPackages.Checked || PS_CleanRegistry.Checked || PS_ServiceRepair.Checked;
        }

        /// <summary>
        /// Shows the description of the specified variable or function.
        /// </summary>
        /// <param name="Variable">Variable name.</param>
        private void ShowVariableDescription(string Variable)
        {
            string Description = CvarFetcher.GetString(Variable);
            if (!string.IsNullOrEmpty(Description))
            {
                MessageBox.Show(Description, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(AppStrings.CE_ClNoDescr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Handles with installed FPS-configs and shows special icon.
        /// </summary>
        private void HandleConfigs()
        {
            try
            {
                if (!Directory.Exists(App.AppCfgDir)) { Directory.CreateDirectory(App.AppCfgDir); }
                App.SourceGames[AppSelector.Text].FPSConfigs = ConfigManager.ListFPSConfigs(App.SourceGames[AppSelector.Text].FullGamePath, App.SourceGames[AppSelector.Text].IsUsingUserDir);
                GT_Warning.Visible = App.SourceGames[AppSelector.Text].FPSConfigs.Count > 0;
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHandleConfigs);
                App.SourceGames[AppSelector.Text].FPSConfigs = new List<string>();
                GT_Warning.Visible = false;
            }
        }

        /// <summary>
        /// Copies the contents of the selected cells to the clipboard.
        /// </summary>
        private void HandleCopy()
        {
            if (CE_Editor.SelectedCells.Count > 0)
            {
                Clipboard.SetDataObject(CE_Editor.GetClipboardContent());
            }
        }

        /// <summary>
        /// Clears the contents of the selected cells.
        /// </summary>
        private void HandleClearSelection()
        {
            foreach (DataGridViewCell Cell in CE_Editor.SelectedCells)
            {
                if (!Cell.OwningRow.IsNewRow)
                {
                    Cell.Value = null;
                }
            }
        }

        /// <summary>
        /// Pastes the contents of the clipboard into the selected cell.
        /// </summary>
        private void HandlePasteSingle()
        {
            if (!CE_Editor.Rows[CE_Editor.CurrentRow.Index].IsNewRow && Clipboard.ContainsText())
            {
                CE_Editor.Rows[CE_Editor.CurrentRow.Index].Cells[CE_Editor.CurrentCell.ColumnIndex].Value = Clipboard.GetText().Trim();
            }
        }

        /// <summary>
        /// Pastes the contents of the clipboard into the selected cells.
        /// Internal implementation.
        /// </summary>
        private void HandlePasteMultipleInternal()
        {
            string[] Items = Clipboard.GetText().Split('\n');
            for (int i = 0; i < Items.Length; i++)
            {
                string[] Item = Items[i].Split('\t');
                if (Item.Length > 1 && !string.IsNullOrWhiteSpace(Item[0]) && !string.IsNullOrWhiteSpace(Item[1]))
                {
                    if ((i < CE_Editor.SelectedRows.Count) && !CE_Editor.SelectedRows[i].IsNewRow)
                    {
                        CE_Editor.SelectedRows[i].Cells[0].Value = Item[0].Trim();
                        CE_Editor.SelectedRows[i].Cells[1].Value = Item[1].Trim();
                    }
                    else
                    {
                        CE_Editor.Rows.Add(Item[0].Trim(), Item[1].Trim());
                    }
                }
            }
        }

        /// <summary>
        /// Pastes the contents of the clipboard into the selected cells.
        /// </summary>
        private void HandlePasteMultiple()
        {
            if (Clipboard.ContainsText())
            {
                HandlePasteMultipleInternal();
            }
        }

        /// <summary>
        /// Downloads and shows on the form screenshot of the selected HUD.
        /// </summary>
        private async Task HandleHUDScreenshot()
        {
            string HUDScreenshotFile = GetHUDScreenshotFileName();
            try
            {
                await DownloadHUDScreenshotTask(GetHUDScreenshotURL(), HUDScreenshotFile);
                HD_GB_Pbx.Image = Image.FromFile(HUDScreenshotFile);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHandleHUDScreenshot);
                FileManager.RemoveFile(HUDScreenshotFile);
            }
        }

        /// <summary>
        /// Handles with current Steam UserID.
        /// </summary>
        /// <param name="SID">Last used Steam UserID.</param>
        private void HandleSteamIDs(string SID)
        {
            try
            {
                string Result = App.SteamClient.GetCurrentSteamID(SID);
                SB_SteamID.Text = Result;
                Properties.Settings.Default.LastSteamID = Result;
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHandleSteamIDs);
                SB_SteamID.Text = string.Empty;
            }
        }

        /// <summary>
        /// Show offline help with specified page name to display.
        /// </summary>
        /// <param name="PageName">Page name to display.</param>
        private void HandleShowHelp(string PageName)
        {
            try
            {
                string CHMFile = Path.Combine(App.FullAppPath, Properties.Resources.AppHelpDirectory, string.Format(Properties.Resources.AppHelpFileName, AppStrings.AppLangPrefix));
                if (File.Exists(CHMFile))
                {
                    Help.ShowHelp(this, CHMFile, HelpNavigator.Topic, PageName);
                }
                else
                {
                    MessageBox.Show(AppStrings.AppHelpCHMNotFound, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHlpShow);
                MessageBox.Show(AppStrings.AppHelpCHMPageError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Changes the state of some controls on "HUD Manager" page.
        /// </summary>
        /// <param name="State">If selected HUD installed.</param>
        private void SetHUDButtons(bool State)
        {
            HD_Install.Text = State ? AppStrings.HD_BtnUpdateText : AppStrings.HD_BtnInstallText;
            HD_Uninstall.Enabled = State;
            HD_OpenDir.Enabled = State;
        }

        /// <summary>
        /// Changes the state of some controls on "FPS-config" page.
        /// </summary>
        /// <param name="State">If selected FPS-config installed.</param>
        private void SetFPSButtons(bool State)
        {
            FP_Install.Text = State ? App.SourceGames[AppSelector.Text].IsUsingUserDir ? AppStrings.FP_BtnUpdateText : AppStrings.FP_BtnReplaceText : AppStrings.FP_BtnInstallText;
            FP_Uninstall.Text = App.SourceGames[AppSelector.Text].IsUsingUserDir ? AppStrings.FP_BtnUninstallSelectedText : AppStrings.FP_BtnUninstallAllText;
            FP_Uninstall.Enabled = State;
            FP_Edit.Enabled = State;
        }

        /// <summary>
        /// Changes the state of the status bar depending of currently selected tab.
        /// </summary>
        private void UpdateStatusBar()
        {
            switch (MainTabControl.SelectedIndex)
            {
                case 1: // "Config Editor" page selected...
                    {
                        MNUShowEdHint.Enabled = true;
                        SB_Status.ForeColor = Color.Black;
                        SB_Status.Text = string.Format(AppStrings.StatusOpenedFile, string.IsNullOrEmpty(CFGFileName) ? AppStrings.UnnamedFileName : Path.GetFileName(CFGFileName));
                    }
                    break;
                default: // Any other page selected...
                    {
                        MNUShowEdHint.Enabled = false;
                        SB_Status.ForeColor = Color.Black;
                        SB_Status.Text = AppStrings.StatusNormal;
                    }
                    break;
            }
        }

        /// <summary>
        /// Checks if all Type 1 game video settings were set by user.
        /// </summary>
        private bool CheckType1Settings()
        {
            return (GT_ScreenType.SelectedIndex != -1) && (GT_ModelQuality.SelectedIndex != -1)
                && (GT_TextureQuality.SelectedIndex != -1) && (GT_ShaderQuality.SelectedIndex != -1)
                && (GT_WaterQuality.SelectedIndex != -1) && (GT_ShadowQuality.SelectedIndex != -1)
                && (GT_ColorCorrectionT.SelectedIndex != -1) && (GT_AntiAliasing.SelectedIndex != -1)
                && (GT_Filtering.SelectedIndex != -1) && (GT_VSync.SelectedIndex != -1)
                && (GT_MotionBlur.SelectedIndex != -1) && (GT_DxMode.SelectedIndex != -1)
                && (GT_HDR.SelectedIndex != -1);
        }

        /// <summary>
        /// Checks if all Type 2 game video settings were set by user.
        /// </summary>
        private bool CheckType2Settings()
        {
            return (GT_NCF_Quality.SelectedIndex != -1) && (GT_NCF_MemPool.SelectedIndex != -1)
                && (GT_NCF_EffectD.SelectedIndex != -1) && (GT_NCF_ShaderE.SelectedIndex != -1)
                && (GT_NCF_Multicore.SelectedIndex != -1) && (GT_NCF_VSync.SelectedIndex != -1)
                && (GT_NCF_Filtering.SelectedIndex != -1) && (GT_NCF_AntiAlias.SelectedIndex != -1)
                && (GT_NCF_DispMode.SelectedIndex != -1) && (GT_NCF_Ratio.SelectedIndex != -1)
                && (GT_NCF_Brightness.SelectedIndex != -1) && (GT_NCF_Shadows.SelectedIndex != -1)
                && (GT_NCF_MBlur.SelectedIndex != -1);
        }

        /// <summary>
        /// Checks if all game video settings were set by user.
        /// </summary>
        private bool ValidateGameSettings()
        {
            bool Result;
            switch (App.SourceGames[AppSelector.Text].SourceType)
            {
                case 1:
                case 4:
                    Result = CheckType1Settings();
                    break;
                case 2:
                    Result = CheckType2Settings();
                    break;
                default:
                    Logger.Warn(DebugStrings.AppDbgIncorrectSourceType);
                    Result = false;
                    break;
            }
            return Result;
        }

        /// <summary>
        /// Closes all loaded files in Config Editor and clean its window.
        /// </summary>
        private void CloseEditorConfigs()
        {
            CFGFileName = string.Empty;
            CE_Editor.Rows.Clear();
        }

        /// <summary>
        /// Gets full list of available backups in a separate thread.
        /// </summary>
        private async void UpdateBackUpList()
        {
            try
            {
                BU_LVTable.Items.Clear();
                AddBackUpsToTable(await UpdateBackUpListTask(AppSelector.Text));
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExUpdateBackUpList);
            }
        }

        /// <summary>
        /// Searches for installed supported games.
        /// </summary>
        private void FindGames()
        {
            // Searching for installed games...
            try
            {
                DetectInstalledGames();
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExXmlParse);
                MessageBox.Show(AppStrings.AppXMLParseError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(ReturnCodes.GameDbParseError);
            }

            // Checking if any installed supported games were found...
            CheckGames(App.SourceGames.InstalledGameNames.Count);
        }

        /// <summary>
        /// Enables or disables HUD Manager page.
        /// </summary>
        /// <param name="Mode">If current game supports custom HUDs.</param>
        private void HandleHUDMode(bool Mode)
        {
            if (Mode)
            {
                if (!MainTabControl.TabPages.Contains(HUDInstall))
                {
                    MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(FPSCfgInstall) + 1, HUDInstall);
                }
            }
            else
            {
                if (MainTabControl.TabPages.Contains(HUDInstall))
                {
                    MainTabControl.TabPages.Remove(HUDInstall);
                }
            }
        }

        /// <summary>
        /// Performs deletion of package cache, if selected.
        /// </summary>
        /// <returns>Returns True if the process completed without errors.</returns>
        private bool HandleCleanPackages()
        {
            if (PS_CleanPackages.Checked)
            {
                try
                {
                    App.SteamClient.CleanPackagesNow();
                }
                catch (Exception Ex)
                {
                    Logger.Error(Ex, DebugStrings.AppDbgExClnPackages);
                    MessageBox.Show(AppStrings.PS_CleanPackagesException, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs deletion of registry entries, if selected.
        /// </summary>
        /// <returns>Returns True if the process completed without errors.</returns>
        private bool HandleCleanRegistry()
        {
            if (PS_CleanRegistry.Checked)
            {
                try
                {
                    if (Properties.Settings.Default.SafeCleanup)
                    {
                        App.Platform.BackUpRegistrySettings(App.SourceGames[AppSelector.Text].FullBackUpDirPath);
                    }
                    App.Platform.CleanRegistrySettings(App.Platform.SteamLanguage);
                }
                catch (Exception Ex)
                {
                    Logger.Error(Ex, DebugStrings.AppDbgExClnReg);
                    MessageBox.Show(AppStrings.PS_CleanRegistryException, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs automatic service repair, if selected.
        /// </summary>
        /// <returns>Returns True if the process completed without errors.</returns>
        private bool HandleServiceRepair()
        {
            if (PS_ServiceRepair.Checked)
            {
                try
                {
                    App.Platform.StartServiceRepair(App.SteamClient.FullBinPath);
                }
                catch (Exception Ex)
                {
                    Logger.Error(Ex, DebugStrings.AppDbgExSvcRepair);
                    MessageBox.Show(AppStrings.PS_ServiceRepairException, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shuts down the running Steam client and handles possible exceptions.
        /// </summary>
        private bool HandleShutdownClient()
        {
            try
            {
                if (ProcessManager.ProcessTerminate("Steam") != 0)
                {
                    MessageBox.Show(AppStrings.PS_ProcessDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExShutdownClient);
                MessageBox.Show(AppStrings.PS_ShutdownClientException, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Starts Steam client and handles possible exceptions.
        /// </summary>
        private void HandleStartClient()
        {
            try
            {
                string ClientBinary = Path.Combine(App.SteamClient.FullSteamPath, App.Platform.SteamBinaryName);
                App.Platform.StartRegularProcess(File.Exists(ClientBinary) ? ClientBinary : Properties.Resources.AppURLSteamStart);
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExStartClient);
                MessageBox.Show(AppStrings.PS_StartClientException, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Launches a program update checker in a separate thread, waits for the
        /// result and returns a message if found.
        /// </summary>
        private async Task CheckForUpdates()
        {
            if (IsAutoUpdateCheckNeeded())
            {
                try
                {
                    if (await CheckForUpdatesTask(App.UserAgent))
                    {
                        MessageBox.Show(string.Format(AppStrings.AppUpdateAvailable, Properties.Resources.AppName), Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Properties.Settings.Default.LastUpdateTime = DateTime.Now;
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExBgWChk);
                }
            }
        }

        /// <summary>
        /// Tries to find and delete an old application update files in a
        /// separate thread.
        /// </summary>
        private async Task CleanOldUpdates()
        {
            try
            {
                if (IsCleanupNeeded())
                {
                    await Task.Run(() => { if (Directory.Exists(App.AppUpdateDir)) { Directory.Delete(App.AppUpdateDir, true); } });
                    Properties.Settings.Default.LastCleanupTime = DateTime.Now;
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExClnOldUpdates);
            }
        }

        /// <summary>
        /// Generates internal offline help URL, based on current tab.
        /// </summary>
        /// <returns>Internal offline help URL.</returns>
        private string GetHelpWebPage()
        {
            // Generating page name, based on tab ID...
            Dictionary<TabPage, string> TabMappings = new Dictionary<TabPage, string>
            {
                { GraphicTweaker, "graphic-tweaker.html" }, // Graphic tweaker...
                { ConfigEditor, "config-editor.html" }, // Config editor...
                { ProblemSolver, "cleanup.html" }, // Problem solver...
                { FPSCfgInstall, "fps-configs.html" }, // FPS-config manager...
                { HUDInstall, "hud-manager.html" }, // HUD manager...
                { RescueCentre, "backups.html" } // BackUps manager...
            };

            // Returns result...
            return TabMappings[MainTabControl.SelectedTab];
        }

        /// <summary>
        /// Checks if the application update check is required.
        /// </summary>
        private bool IsAutoUpdateCheckNeeded()
        {
            return Properties.Settings.Default.AutoUpdateCheck && (DateTime.Now - Properties.Settings.Default.LastUpdateTime).Days >= Properties.Settings.Default.UpdateCheckInterval;
        }

        /// <summary>
        /// Checks if the application needs to perform cleanup.
        /// </summary>
        private bool IsCleanupNeeded()
        {
            if (!App.Platform.AutoUpdateSupported) { return false; }
            return (DateTime.Now - Properties.Settings.Default.LastCleanupTime).Days >= Properties.Settings.Default.CleanupInterval;
        }

        /// <summary>
        /// Adds found backup files to collection on BackUps tab.
        /// </summary>
        /// <param name="DItems">Found items.</param>
        private void AddBackUpsToTable(List<FileInfo> DItems)
        {
            foreach (FileInfo DItem in DItems)
            {
                Tuple<string, string> Rs = GenUserFriendlyBackupDesc(DItem);
                ListViewItem LvItem = new ListViewItem(Rs.Item2)
                {
                    BackColor = Properties.Settings.Default.HighlightOldBackUps && (DateTime.UtcNow - DItem.CreationTimeUtc > TimeSpan.FromDays(30)) ? Color.LightYellow : BU_LVTable.BackColor,
                    SubItems =
                    {
                        Rs.Item1,
                        GuiHelpers.SclBytes(DItem.Length),
                        DItem.CreationTime.ToString(CultureInfo.CurrentCulture),
                        DItem.Name
                    }
                };
                BU_LVTable.Items.Add(LvItem);
            }
        }

        /// <summary>
        /// Opens cleanup window and start cleanup sequence with additional targets.
        /// </summary>
        /// <param name="ID">Cleanup target ID.</param>
        /// <param name="Title">Title for cleanup window.</param>
        /// <param name="Targets">Additional targets for cleanup.</param>
        private void StartCleanup(int ID, string Title, List<CleanupItem> Targets)
        {
            if (App.SourceGames[AppSelector.Text].ClnMan == null)
            {
                MessageBox.Show(AppStrings.PS_BwBusy, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<CleanupItem> CleanDirs = new List<CleanupItem>(App.SourceGames[AppSelector.Text].ClnMan[ID].Items);
                if (Targets != null)
                {
                    CleanDirs.AddRange(Targets);
                }
                GuiHelpers.FormShowCleanup(CleanDirs, Title.ToLower(CultureInfo.CurrentUICulture), AppStrings.PS_CleanupSuccess, App.SourceGames[AppSelector.Text].FullBackUpDirPath, App.SourceGames[AppSelector.Text].GameBinaryFile);
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExStartCleanup);
                MessageBox.Show(AppStrings.PS_ClnWndInitError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens cleanup window and start cleanup sequence.
        /// </summary>
        /// <param name="ID">Cleanup target ID.</param>
        /// <param name="Title">Title for cleanup window.</param>
        private void StartCleanup(int ID, string Title)
        {
            StartCleanup(ID, Title, null);
        }

        /// <summary>
        /// Searches for available HUDs.
        /// </summary>
        private async void HandleHUDs()
        {
            if (App.SourceGames[AppSelector.Text].IsHUDsAvailable)
            {
                try
                {
                    await HandleHUDsTask(AppSelector.Text);
                    HD_HSel.Items.AddRange(App.SourceGames[AppSelector.Text].HUDMan.AvailableHUDNames.ToArray<object>());
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExHandleHUDs);
                }
            }
        }

        /// <summary>
        /// Searches for available FPS-configs.
        /// </summary>
        private async void HandleFpsConfigs()
        {
            try
            {
                // Building collection in a separate thread...
                await HandleFpsConfigsTask(AppSelector.Text);

                // Adding configs to collection...
                FP_ConfigSel.Items.AddRange(App.SourceGames[AppSelector.Text].CFGMan.ConfigNames.ToArray());

                // Checking if collection contains any items...
                if (FP_ConfigSel.Items.Count >= 1)
                {
                    FP_Description.Text = AppStrings.FP_SelectFromList;
                    FP_Description.ForeColor = Color.Black;
                }
            }
            catch (Exception Ex)
            {
                // Exception detected. Writing to log...
                Logger.Warn(Ex, DebugStrings.AppDbgExHandleFpsConfigs);

                // Showing message...
                FP_Description.Text = AppStrings.FP_NoCfgGame;
                FP_Description.ForeColor = Color.Red;

                // Blockg some form controls...
                FP_Install.Enabled = false;
                FP_ConfigSel.Enabled = false;
                FP_Edit.Enabled = false;
            }
        }

        /// <summary>
        /// Installs the selected HUD.
        /// </summary>
        private async Task HandleHUDInstallation()
        {
            try
            {
                HD_Install.Enabled = false;
                await HandleHUDInstallationTask(AppSelector.Text, HD_HSel.Text);
                HD_Install.Enabled = true;
                MessageBox.Show(AppStrings.HD_InstallSuccessfull, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetHUDButtons(HUDManager.CheckInstalledHUD(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir));
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExHUDInstall);
                MessageBox.Show(AppStrings.HD_InstallError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Searches for available cleanup targets.
        /// </summary>
        private async void HandleCleanupTargets()
        {
            try
            {
                await HandleCleanupTargetsTask(AppSelector.Text);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHandleCleanupTargets);
            }
        }

        /// <summary>
        /// BackUp available FPS-configs to archive if SafeCleanup
        /// is enabled.
        /// </summary>
        private void BackUpFPSConfigs()
        {
            if (Properties.Settings.Default.SafeCleanup)
            {
                try
                {
                    string BackUpFileName = FileManager.GenerateBackUpFileName(App.SourceGames[AppSelector.Text].FullBackUpDirPath, Properties.Resources.BU_PrefixCfg);
                    if (App.SourceGames[AppSelector.Text].IsUsingUserDir)
                    {
                        string ConfigDir = Path.Combine(App.SourceGames[AppSelector.Text].CFGMan.FPSConfigInstallPath, App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].InstallDir);
                        if (Directory.Exists(ConfigDir))
                        {
                            FileManager.CompressDirectory(ConfigDir, BackUpFileName);
                        }
                    }
                    else
                    {
                        if (App.SourceGames[AppSelector.Text].FPSConfigs.Count > 0)
                        {
                            FileManager.CompressFiles(App.SourceGames[AppSelector.Text].FPSConfigs, BackUpFileName);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExFpsInstBackup);
                }
            }
        }

        /// <summary>
        /// Configures Save and Save As dialogs.
        /// </summary>
        private void ConfigureSaveFileDialog()
        {
            CE_SaveCfgDialog.InitialDirectory = App.SourceGames[AppSelector.Text].FullCfgPath;
            CE_SaveCfgDialog.FileName = File.Exists(Path.Combine(App.SourceGames[AppSelector.Text].FullCfgPath, "autoexec.cfg")) ? AppStrings.UnnamedFileName : "autoexec.cfg";
        }

        /// <summary>
        /// Loads the specified FPS-config into the Config Editor or the default
        /// text editor.
        /// </summary>
        /// <param name="ConfigFile">Full path to the config file for editing.</param>
        /// <param name="UseTextEditor">Use the default text editor instead of the Config Editor.</param>
        private async Task EditFPSConfig(string ConfigFile, bool UseTextEditor)
        {
            if (!string.IsNullOrWhiteSpace(ConfigFile) && File.Exists(ConfigFile))
            {
                if (UseTextEditor)
                {
                    App.Platform.OpenTextEditor(ConfigFile, Properties.Settings.Default.EditorBin);
                }
                else
                {
                    await ReadConfigFromFileTask(ConfigFile);
                    MainTabControl.SelectedIndex = 1;
                }
            }
            else
            {
                Logger.Warn(DebugStrings.AppDbgExCfgEditorLoad, ConfigFile);
            }
        }

        /// <summary>
        /// Saves contents of Config Editor to a specified configuration file.
        /// <param name="ConfigFile">Full path to the configuration file.</param>
        private async Task SaveConfigToFile(string ConfigFile)
        {
            try
            {
                await WriteTableToFileTask(ConfigFile);
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExCfgEdSave);
                MessageBox.Show(AppStrings.CE_CfgSVVEx, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Checks if any of FPS-configs are installed.
        /// </summary>
        /// <returns>Returns True if at least one FPS-config is installed.</returns>
        private bool CheckIfFPSConfigInstalled()
        {
            return App.SourceGames[AppSelector.Text].CFGMan.CheckInstalledConfig(App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].InstallDir, App.SourceGames[AppSelector.Text].IsUsingUserDir);
        }

        /// <summary>
        /// Gets FPS-config download URI.
        /// </summary>
        /// <param name="ForceMirror">Force use of the reserve server.</param>
        /// <returns>Returns FPS-config download URI.</returns>
        private string GetFPSConfigDownloadURI(bool ForceMirror)
        {
            return ForceMirror ? App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].Mirror : App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].URI;
        }

        /// <summary>
        /// Downloads FPS-config from the main or reserve server.
        /// </summary>
        /// <param name="ForceMirror">Force use of the reserve server.</param>
        /// <returns>Returns True if the file was downloaded.</returns>
        private bool DownloadFPSConfig(bool ForceMirror = false)
        {
            GuiHelpers.FormShowDownloader(GetFPSConfigDownloadURI(ForceMirror), App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].LocalFile);
            return File.Exists(App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].LocalFile);
        }

        /// <summary>
        /// Installs the downloaded FPS-config.
        /// </summary>
        private void InstallFPSConfig()
        {
            // Checking hash of downloaded file...
            if (App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].CheckHash())
            {
                // Checking if selected FPS-config is installed...
                if (CheckIfFPSConfigInstalled())
                {
                    // Creating backup of current FPS-config files...
                    BackUpFPSConfigs();

                    // Removing installed files...
                    GuiHelpers.FormShowRemoveFiles(Path.Combine(App.SourceGames[AppSelector.Text].CFGMan.FPSConfigInstallPath, App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].InstallDir));
                }

                // Extracting downloaded archove...
                GuiHelpers.FormShowArchiveExtract(App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].LocalFile, App.SourceGames[AppSelector.Text].CFGMan.FPSConfigInstallPath);

                // Moving archive contents for games without user directory support...
                if (!App.SourceGames[AppSelector.Text].IsUsingUserDir)
                {
                    App.SourceGames[AppSelector.Text].CFGMan.MoveLegacyConfig(App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].InstallDir, App.SourceGames[AppSelector.Text].FullCfgPath);
                }

                // Installation successful message...
                MessageBox.Show(AppStrings.FP_InstallSuccessful, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Hash missmatch. Show error...
                MessageBox.Show(AppStrings.FP_HashError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Checks if the selected HUD is installed.
        /// </summary>
        /// <returns>Returns True if selected HUD is installed.</returns>
        private bool CheckIfHUDInstalled()
        {
            return HUDManager.CheckInstalledHUD(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir);
        }

        /// <summary>
        /// Gets HUD download URI.
        /// </summary>
        /// <param name="ForceMirror">Force use of the reserve server.</param>
        /// <returns>Returns HUD download URI.</returns>
        private string GetHUDDownloadURI(bool ForceMirror)
        {
            string Result = App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].URI;

            if (ForceMirror)
            {
                Result = App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].Mirror;
            }
            else
            {
                if (Properties.Settings.Default.HUDUseUpstream)
                {
                    Result = App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].UpURI;
                }
            }

            return Result;
        }

        /// <summary>
        /// Gets the HUD screenshot URL.
        /// </summary>
        /// <returns>Returns HUD screenshot URL.</returns>
        private string GetHUDScreenshotURL()
        {
            return App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].Preview;
        }

        /// <summary>
        /// Gets the local screenshot file name of the selected HUD.
        /// </summary>
        /// <returns>Returns full path to the HUD screenshot file.</returns>
        private string GetHUDScreenshotFileName()
        {
            return Path.Combine(App.AppHUDDir, Path.GetFileName(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].Preview));
        }

        /// <summary>
        /// Downloads HUD archive from the main or reserve server.
        /// </summary>
        /// <param name="ForceMirror">Force use of the reserve server.</param>
        /// <returns>Returns True if the file was downloaded.</returns>
        private bool DownloadHUD(bool ForceMirror = false)
        {
            GuiHelpers.FormShowDownloader(GetHUDDownloadURI(ForceMirror), App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].LocalFile);
            return File.Exists(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].LocalFile);
        }

        /// <summary>
        /// Installs the contents of the downloaded HUD archive.
        /// </summary>
        private async Task InstallHUD()
        {
            // Checking hash of downloaded file...
            if (Properties.Settings.Default.HUDUseUpstream || App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].CheckHash())
            {
                // Checking if selected HUD is installed...
                if (CheckIfHUDInstalled())
                {
                    // Removing installed files...
                    GuiHelpers.FormShowRemoveFiles(Path.Combine(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir));
                }

                // Extracting downloaded archove...
                GuiHelpers.FormShowArchiveExtract(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].LocalFile, Path.Combine(App.SourceGames[AppSelector.Text].CustomInstallDir, "hudtemp"));

                // Installing files in a separate thread...
                await HandleHUDInstallation();
            }
            else
            {
                // Hash missmatch. Show error...
                MessageBox.Show(AppStrings.HD_HashError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Removing downloaded file...
            try
            {
                File.Delete(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].LocalFile);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHudArchRem);
            }
        }

        /// <summary>
        /// Checks for application updates in a separate thread.
        /// </summary>
        /// <param name="Agent">User-Agent header for outgoing HTTP queries.</param>
        /// <returns>Returns True if updates were found.</returns>
        private async Task<bool> CheckForUpdatesTask(string Agent)
        {
            try
            {
                return (await UpdateManager.Create(Properties.Resources.AppURLUpdatePrimary, Agent)).CheckAppUpdate();
            }
            catch (WebException Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCheckForUpdatesPrimary);
                return (await UpdateManager.Create(Properties.Resources.AppURLUpdateMirror, Agent)).CheckAppUpdate();
            }
        }

        /// <summary>
        /// Gets collection of available FPS-configs for the selected game
        /// in a separate thread.
        /// </summary>
        /// <param name="SelectedGame">Selected game name.</param>
        private async Task HandleFpsConfigsTask(string SelectedGame)
        {
            await Task.Run(() =>
            {
                App.SourceGames[SelectedGame].CFGMan = new ConfigManager(App.FullAppPath, App.AppCfgDir, App.SourceGames[SelectedGame].IsUsingUserDir ? App.SourceGames[SelectedGame].CustomInstallDir : App.SourceGames[SelectedGame].FullGamePath, AppStrings.AppLangPrefix);
            });
        }

        /// <summary>
        /// Gets collection of available backups for the selected game
        /// in a separate thread.
        /// </summary>
        /// <param name="SelectedGame">Selected game name.</param>
        /// <returns>Returns array of available backup files.</returns>
        private async Task<List<FileInfo>> UpdateBackUpListTask(string SelectedGame)
        {
            return await Task.Run(() =>
            {
                // Checking if the directory exists. If not, creating it...
                if (!Directory.Exists(App.SourceGames[SelectedGame].FullBackUpDirPath))
                {
                    Directory.CreateDirectory(App.SourceGames[SelectedGame].FullBackUpDirPath);
                }

                // Getting the list of files in the game backups directory...
                return Task.FromResult(FileManager.FindFilesInfo(App.SourceGames[SelectedGame].FullBackUpDirPath));
            });
        }

        /// <summary>
        /// Gets a collection of available HUDs for the selected game
        /// in a separate thread.
        /// </summary>
        /// <param name="SelectedGame">Selected game name.</param>
        private async Task HandleHUDsTask(string SelectedGame)
        {
            await Task.Run(() =>
            {
                // Creating if the local directory for HUDs exists. If not, creating it...
                if (!Directory.Exists(App.AppHUDDir))
                {
                    Directory.CreateDirectory(App.AppHUDDir);
                }

                // Building the HUD collection...
                App.SourceGames[SelectedGame].HUDMan = new HUDManager(App.SourceGames[SelectedGame].SmallAppName, App.FullAppPath, App.AppHUDDir, Properties.Settings.Default.HUDHideOutdated);
            });
        }

        /// <summary>
        /// Gets a screenshot of selected HUD in a separate thread.
        /// </summary>
        /// <param name="ScreenURL">Selected game name.</param>
        /// <param name="ScreenFile">Selected HUD name.</param>
        private async Task DownloadHUDScreenshotTask(string ScreenURL, string ScreenFile)
        {
            if (!File.Exists(ScreenFile))
            {
                using (WebClient Downloader = new WebClient())
                {
                    Downloader.Headers.Add("User-Agent", App.UserAgent);
                    await Downloader.DownloadFileTaskAsync(ScreenURL, ScreenFile);
                }
            }
        }

        /// <summary>
        /// Finalizes installation of the selected HUD in a separate thread.
        /// </summary>
        /// <param name="SelectedGame">Selected game name.</param>
        /// <param name="SelectedHUD">Selected HUD name.</param>
        private async Task HandleHUDInstallationTask(string SelectedGame, string SelectedHUD)
        {
            string InstallTmp = Path.Combine(App.SourceGames[SelectedGame].CustomInstallDir, "hudtemp");
            await Task.Run(() =>
            {
                try
                {
                    Directory.Move(Path.Combine(InstallTmp, HUDManager.FormatIntDir(App.SourceGames[SelectedGame].HUDMan[SelectedHUD].ArchiveDir)), Path.Combine(App.SourceGames[SelectedGame].CustomInstallDir, App.SourceGames[SelectedGame].HUDMan[SelectedHUD].InstallDir));
                }
                finally
                {
                    try
                    {
                        if (Directory.Exists(InstallTmp))
                        {
                            Directory.Delete(InstallTmp, true);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Logger.Warn(Ex, DebugStrings.AppDbgExHUDInstallationCleanup);
                    }
                }
            });
        }

        /// <summary>
        /// Gets a collection of available cleanup targets for the selected
        /// game in a separate thread.
        /// </summary>
        /// <param name="SelectedGame">Selected game name.</param>
        private async Task HandleCleanupTargetsTask(string SelectedGame)
        {
            await Task.Run(() =>
            {
                App.SourceGames[SelectedGame].ClnMan = new CleanupManager(App.FullAppPath, App.SourceGames[SelectedGame], Properties.Settings.Default.AllowUnSafeCleanup);
            });
        }

        /// <summary>
        /// Gets a collection of available plugins in a separate thread.
        /// </summary>
        private async Task FindPluginsTask()
        {
            await Task.Run(() =>
            {
                App.Plugins = new PluginManager(App.FullAppPath);
            });
        }

        /// <summary>
        /// Loads config file to Config Editor in a separate thread.
        /// </summary>
        /// <param name="ConfFileName">Full path to config file.</param>
        private async Task LoadConfigFromFileTask(string ConfFileName)
        {
            // Loading config file...
            using (StreamReader ConfigFile = new StreamReader(ConfFileName, Encoding.UTF8))
            {
                // Reading config file to the end...
                while (ConfigFile.Peek() >= 0)
                {
                    // Clearing string from special chars...
                    string ImpStr = StringsManager.CleanString(await ConfigFile.ReadLineAsync());

                    // Checking if the source string is empty or a commentary and parsing it...
                    if (!string.IsNullOrEmpty(ImpStr) && ImpStr[0] != '/' && ConfigEntryParser.TryParse(ImpStr, out ConfigEntryParser Parser))
                    {
                        CE_Editor.Rows.Add(Parser.Variable, Parser.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Loads config file to Config Editor.
        /// </summary>
        /// <param name="ConfFileName">Full path to config file.</param>
        private async Task ReadConfigFromFileTask(string ConfFileName)
        {
            if (File.Exists(ConfFileName))
            {
                try
                {
                    CFGFileName = ConfFileName;
                    CE_Editor.Rows.Clear();
                    await LoadConfigFromFileTask(ConfFileName);
                    UpdateStatusBar();
                }
                catch (Exception Ex)
                {
                    Logger.Error(Ex, DebugStrings.AppDbgExCfgEdLoad);
                    MessageBox.Show(AppStrings.CE_ExceptionDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(AppStrings.CE_OpenFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Saves contents of Config Editor to a text file in a separate thread.
        /// Used by Save and Save As operations.
        /// </summary>
        /// <param name="ConfFile">Full path to config file.</param>
        private async Task WriteTableToFileTask(string ConfFile)
        {
            using (StreamWriter CFile = new StreamWriter(ConfFile))
            {
                foreach (DataGridViewRow Row in CE_Editor.Rows)
                {
                    if (Row.Cells[0].Value != null)
                    {
                        await CFile.WriteLineAsync(string.Format("{0} {1}", Row.Cells[0].Value, Row.Cells[1].Value));
                    }
                }
            }
        }

        /// <summary>
        /// "Form create" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void FrmMainW_Load(object sender, EventArgs e)
        {
            ConfigureCryptoPolicy();
            InitializeApp();
            FindPlugins();
            SetAppStrings();
            ChangePrvControlState();
            CheckSafeClnStatus();
            CheckSymbolsSteam();
            FindGames();
            await CheckForUpdates();
            await CleanOldUpdates();
        }

        /// <summary>
        /// "Clean packages" checkbox status changed event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_CleanPackages_CheckedChanged(object sender, EventArgs e)
        {
            PS_ExecuteNow.Enabled = GetRepairSelState();
        }

        /// <summary>
        /// "Clean registry" checkbox status changed event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_CleanRegistry_CheckedChanged(object sender, EventArgs e)
        {
            PS_ExecuteNow.Enabled = GetRepairSelState();
        }

        /// <summary>
        /// "Service Repair" checkbox status changed event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_ServiceRepair_CheckedChanged(object sender, EventArgs e)
        {
            PS_ExecuteNow.Enabled = GetRepairSelState();
        }

        /// <summary>
        /// "Execute cleanup" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_ExecuteNow_Click(object sender, EventArgs e)
        {
            if (GetRepairSelState() && MessageBox.Show(AppStrings.PS_ExecuteMSG, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (!HandleShutdownClient()) { return; }

                bool CleanStatus = true;
                CleanStatus &= HandleCleanPackages();
                CleanStatus &= HandleCleanRegistry();
                CleanStatus &= HandleServiceRepair();

                if (CleanStatus)
                {
                    MessageBox.Show(AppStrings.PS_SeqCompleted, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    HandleStartClient();
                }
            }
        }

        /// <summary>
        /// "Form close" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void FrmMainW_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = Properties.Settings.Default.ConfirmExit && MessageBox.Show(string.Format(AppStrings.FrmCloseQuery, Properties.Resources.AppName), Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes;
            }
        }

        /// <summary>
        /// "Game selected" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void AppSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                HandleControlsOnSelGame();
                CheckSymbolsGame();
                DetectFS();

                try
                {
                    LoadGraphicSettings();
                }
                catch (Exception Ex)
                {
                    Logger.Error(Ex, DebugStrings.AppDbgExVideoLoadFail);
                    NullGraphSettings();
                    MessageBox.Show(AppStrings.GT_VideoLoadErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                HandleSteamIDs(Properties.Settings.Default.LastSteamID);
                HandleConfigs();
                CloseEditorConfigs();
                HandleFpsConfigs();
                UpdateStatusBar();
                Properties.Settings.Default.LastGameName = AppSelector.Text;
                HandleHUDMode(App.SourceGames[AppSelector.Text].IsHUDsAvailable);
                HandleHUDs();
                UpdateBackUpList();
                HandleCleanupTargets();
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExSelGame);
                MessageBox.Show(AppStrings.AppFailedToGetData, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// "Refresh game list" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void AppRefresh_Click(object sender, EventArgs e)
        {
            FindGames();
        }

        /// <summary>
        /// "Maximum quality" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void GT_Maximum_Graphics_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppStrings.GT_MaxPerfMsg, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                switch (App.SourceGames[AppSelector.Text].SourceType)
                {
                    case 1:
                    case 4:
                        GT_ScreenType.SelectedIndex = 0;
                        GT_ModelQuality.SelectedIndex = 2;
                        GT_TextureQuality.SelectedIndex = 2;
                        GT_ShaderQuality.SelectedIndex = 1;
                        GT_WaterQuality.SelectedIndex = 1;
                        GT_ShadowQuality.SelectedIndex = 1;
                        GT_ColorCorrectionT.SelectedIndex = 1;
                        GT_AntiAliasing.SelectedIndex = 5;
                        GT_Filtering.SelectedIndex = 5;
                        GT_VSync.SelectedIndex = 0;
                        GT_MotionBlur.SelectedIndex = 0;
                        GT_DxMode.SelectedIndex = 3;
                        GT_HDR.SelectedIndex = 2;
                        break;
                    case 2:
                        GT_NCF_DispMode.SelectedIndex = 0;
                        GT_NCF_Ratio.SelectedIndex = 1;
                        GT_NCF_Brightness.Text = "22";
                        GT_NCF_AntiAlias.SelectedIndex = 5;
                        GT_NCF_Filtering.SelectedIndex = 5;
                        GT_NCF_Shadows.SelectedIndex = 3;
                        GT_NCF_MBlur.SelectedIndex = 1;
                        GT_NCF_VSync.SelectedIndex = 0;
                        GT_NCF_Multicore.SelectedIndex = 1;
                        GT_NCF_ShaderE.SelectedIndex = 3;
                        GT_NCF_EffectD.SelectedIndex = 2;
                        GT_NCF_MemPool.SelectedIndex = 2;
                        GT_NCF_Quality.SelectedIndex = 2;
                        break;
                    default:
                        Logger.Warn(DebugStrings.AppDbgIncorrectSourceType);
                        break;
                }
                MessageBox.Show(AppStrings.GT_PerfSet, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// "Maximum performance" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void GT_Maximum_Performance_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppStrings.GT_MinPerfMsg, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                switch (App.SourceGames[AppSelector.Text].SourceType)
                {
                    case 1:
                    case 4:
                        GT_ScreenType.SelectedIndex = 0;
                        GT_ModelQuality.SelectedIndex = 0;
                        GT_TextureQuality.SelectedIndex = 0;
                        GT_ShaderQuality.SelectedIndex = 0;
                        GT_WaterQuality.SelectedIndex = 0;
                        GT_ShadowQuality.SelectedIndex = 0;
                        GT_ColorCorrectionT.SelectedIndex = 0;
                        GT_AntiAliasing.SelectedIndex = 0;
                        GT_Filtering.SelectedIndex = 1;
                        GT_VSync.SelectedIndex = 0;
                        GT_MotionBlur.SelectedIndex = 0;
                        GT_DxMode.SelectedIndex = MessageBox.Show(AppStrings.GT_DxLevelMsg, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes ? 0 : 3;
                        GT_HDR.SelectedIndex = 0;
                        break;
                    case 2:
                        GT_NCF_DispMode.SelectedIndex = 0;
                        GT_NCF_Ratio.SelectedIndex = 1;
                        GT_NCF_Brightness.Text = "22";
                        GT_NCF_AntiAlias.SelectedIndex = 0;
                        GT_NCF_Filtering.SelectedIndex = 1;
                        GT_NCF_Shadows.SelectedIndex = 0;
                        GT_NCF_MBlur.SelectedIndex = 0;
                        GT_NCF_VSync.SelectedIndex = 0;
                        GT_NCF_Multicore.SelectedIndex = 1;
                        GT_NCF_ShaderE.SelectedIndex = 0;
                        GT_NCF_EffectD.SelectedIndex = 0;
                        GT_NCF_MemPool.SelectedIndex = 0;
                        GT_NCF_Quality.SelectedIndex = 0;
                        break;
                    default:
                        Logger.Warn(DebugStrings.AppDbgIncorrectSourceType);
                        break;
                }
                MessageBox.Show(AppStrings.GT_PerfSet, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// "Save video settings" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void GT_SaveApply_Click(object sender, EventArgs e)
        {
            if (ValidateGameSettings())
            {
                if (MessageBox.Show(AppStrings.GT_SaveMsg, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        WriteGraphicSettings();
                        MessageBox.Show(AppStrings.GT_SaveSuccess, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception Ex)
                    {
                        Logger.Error(Ex, DebugStrings.AppDbgExGTSave);
                        MessageBox.Show(AppStrings.GT_SaveFailure, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(AppStrings.GT_NCFNReady, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "FPS-config selected" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void FP_ConfigSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Checking result...
                bool Success = !string.IsNullOrEmpty(App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].Name);

                // Changing some controls state...
                FP_Description.Text = App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].Description;
                FP_Comp.Visible = !App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].CheckCompatibility(App.SourceGames[AppSelector.Text].GameInternalID);
                FP_Install.Enabled = Success;

                // Checking if selected FPS-config is installed...
                SetFPSButtons(CheckIfFPSConfigInstalled());
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExFPSConfigSelectionChange);
                FP_Description.Text = AppStrings.FP_NoDescr;
            }
        }

        /// <summary>
        /// "Install FPS-config" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void FP_Install_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(AppStrings.FP_InstallQuestion, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Downloading FPS-config...
                    bool DownloadResult = DownloadFPSConfig();

                    // If cannot download from the main server, let's try mirrors...
                    if (!DownloadResult)
                    {
                        Logger.Warn(DebugStrings.AppDbgFPSDnlMain);
                        DownloadResult = DownloadFPSConfig(true);
                    }

                    // Installing downloaded FPS-config...
                    if (DownloadResult)
                    {
                        InstallFPSConfig();
                    }
                    else
                    {
                        Logger.Error(DebugStrings.AppDbgFPSDnlMirror);
                        MessageBox.Show(AppStrings.FP_DownloadError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Changing the state of some controls...
                    SetFPSButtons(DownloadResult);
                    HandleConfigs();
                }
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExFpsInstall);
                MessageBox.Show(AppStrings.FP_InstallFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    // Removing downloaded file...
                    FileManager.RemoveFile(App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].LocalFile);
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExCfgArchRem);
                }
            }
        }

        /// <summary>
        /// "Uninstall FPS-config" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void FP_Uninstall_Click(object sender, EventArgs e)
        {
            try
            {
                // If the game is using custom user directory and it exists, silently removing its contents...
                if (App.SourceGames[AppSelector.Text].IsUsingUserDir)
                {
                    // Asking for confirmation...
                    if (MessageBox.Show(AppStrings.FP_UninstallQuestion, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        // Removing files...
                        GuiHelpers.FormShowRemoveFiles(Path.Combine(App.SourceGames[AppSelector.Text].CFGMan.FPSConfigInstallPath, App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].InstallDir));

                        // Showing message...
                        MessageBox.Show(AppStrings.FP_RemoveSuccessful, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Showing interactive dialog with detected FPS-configs...
                    GuiHelpers.FormShowCleanup(CleanupItem.CreateFromLegacyList(App.SourceGames[AppSelector.Text].FPSConfigs, false, true), ((Button)sender).Text.ToLower(CultureInfo.CurrentUICulture), AppStrings.FP_RemoveSuccessful, App.SourceGames[AppSelector.Text].FullBackUpDirPath, App.SourceGames[AppSelector.Text].GameBinaryFile, false, false, Properties.Settings.Default.SafeCleanup);
                }

                // Changing the state of some controls...
                SetFPSButtons(CheckIfFPSConfigInstalled());
                HandleConfigs();
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExFpsUninstall);
                MessageBox.Show(AppStrings.FP_RemoveFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// "FPS-config warning" icon click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void GT_Warning_Click(object sender, EventArgs e)
        {
            try
            {
                string SelectedCfg = GuiHelpers.FormShowCfgSelect(App.SourceGames[AppSelector.Text].FPSConfigs);
                if (!string.IsNullOrWhiteSpace(SelectedCfg))
                {
                    await EditFPSConfig(SelectedCfg, ModifierKeys == Keys.Shift);
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCfgSelection);
                MessageBox.Show(AppStrings.CS_FailedToOpenCfg, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "New config" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_New_Click(object sender, EventArgs e)
        {
            CloseEditorConfigs();
            UpdateStatusBar();
        }

        /// <summary>
        /// "Open config" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void CE_Open_Click(object sender, EventArgs e)
        {
            CE_OpenCfgDialog.InitialDirectory = App.SourceGames[AppSelector.Text].FullCfgPath;

            if (CE_OpenCfgDialog.ShowDialog() == DialogResult.OK)
            {
                CheckGameConfigEditor();
                await ReadConfigFromFileTask(CE_OpenCfgDialog.FileName);
            }
        }

        /// <summary>
        /// "Save config" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void CE_Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CFGFileName))
            {
                if (Properties.Settings.Default.SafeCleanup && File.Exists(CFGFileName))
                {
                    try
                    {
                        FileManager.CreateConfigBackUp(CFGFileName, App.SourceGames[AppSelector.Text].FullBackUpDirPath, Properties.Resources.BU_PrefixCfg);
                    }
                    catch (Exception Ex)
                    {
                        Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdAutoBackup);
                    }
                }
                await SaveConfigToFile(CFGFileName);
            }
            else
            {
                ConfigureSaveFileDialog();
                if (CE_SaveCfgDialog.ShowDialog() == DialogResult.OK)
                {
                    await SaveConfigToFile(CE_SaveCfgDialog.FileName);
                    CFGFileName = CE_SaveCfgDialog.FileName;
                    UpdateStatusBar();
                }
            }
        }

        /// <summary>
        /// "Save config as" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void CE_SaveAs_Click(object sender, EventArgs e)
        {
            ConfigureSaveFileDialog();
            if (CE_SaveCfgDialog.ShowDialog() == DialogResult.OK)
            {
                await SaveConfigToFile(CE_SaveCfgDialog.FileName);
            }
        }

        /// <summary>
        /// "Clean custom maps" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemCustMaps_Click(object sender, EventArgs e)
        {
            StartCleanup(0, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean download cache" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemDnlCache_Click(object sender, EventArgs e)
        {
            StartCleanup(1, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean sound cache" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemSoundCache_Click(object sender, EventArgs e)
        {
            StartCleanup(2, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean screenshots" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemScreenShots_Click(object sender, EventArgs e)
        {
            StartCleanup(3, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean recorded demos" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemDemos_Click(object sender, EventArgs e)
        {
            StartCleanup(4, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean game configs" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemGameOpts_Click(object sender, EventArgs e)
        {
            // Creating list with candidates...
            List<string> CleanDirs = new List<string>();

            // Asking for confirmation...
            if (MessageBox.Show(string.Format(AppStrings.AppQuestionTemplate, ((Button)sender).Text), Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    VideoSettingsBackup(false);
                    VideoSettingsRemove();

                    // Creating backup...
                    if (Properties.Settings.Default.SafeCleanup)
                    {
                        try
                        {
                            FileManager.CreateConfigBackUp(App.SourceGames[AppSelector.Text].CloudConfigs, App.SourceGames[AppSelector.Text].FullBackUpDirPath, Properties.Resources.BU_PrefixCfg);
                        }
                        catch (Exception Ex)
                        {
                            Logger.Warn(Ex, DebugStrings.AppDbgExRemVdAutoCfg);
                        }
                    }

                    // Adding configs to list...
                    CleanDirs.Add(Path.Combine(App.SourceGames[AppSelector.Text].FullCfgPath, "config.cfg"));
                    CleanDirs.AddRange(App.SourceGames[AppSelector.Text].CloudConfigs);

                    // Removing all candidates...
                    GuiHelpers.FormShowRemoveFiles(CleanDirs);
                    MessageBox.Show(AppStrings.PS_CleanupSuccess, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExRemVd);
                    MessageBox.Show(AppStrings.PS_CleanupErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// "Clean old binaries" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemOldBin_Click(object sender, EventArgs e)
        {
            StartCleanup(5, ((Button)sender).Text);
        }

        /// <summary>
        /// "Verify integrity of game cache" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_CheckCache_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(AppStrings.AppQuestionTemplate, ((Button)sender).Text), Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    App.Platform.StartRegularProcess(string.Format(Properties.Resources.AppURLGameValidate, App.SourceGames[AppSelector.Text].GameInternalID));
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExValCache);
                    MessageBox.Show(AppStrings.AppStartSteamFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// "Open Settings" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUAppOptions_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowOptions();
        }

        /// <summary>
        /// "Open Reporter" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUReportBuilder_Click(object sender, EventArgs e)
        {
            if ((AppSelector.Items.Count > 0) && (AppSelector.SelectedIndex != -1))
            {
                GuiHelpers.FormShowRepBuilder(App.AppReportDir, App.AppLogDir, App.SteamClient.FullDumpsPath, App.SteamClient.FullLogsPath, App.SourceGames[AppSelector.Text]);
            }
            else
            {
                MessageBox.Show(AppStrings.AppNoGamesSelected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Open Installer" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUInstaller_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowInstaller(App.SourceGames[AppSelector.Text].FullGamePath, App.SourceGames[AppSelector.Text].IsUsingUserDir, App.SourceGames[AppSelector.Text].CustomInstallDir);
        }

        /// <summary>
        /// "Quit application" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(ReturnCodes.Success);
        }

        /// <summary>
        /// "About" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUAbout_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowAbout();
        }

        /// <summary>
        /// "Report bug" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUReportBug_Click(object sender, EventArgs e)
        {
            try
            {
                App.Platform.OpenWebPage(Properties.Resources.AppURLBugTracker);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExBugRep);
                MessageBox.Show(AppStrings.AppVisitBugTrackerError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Current tab changed" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        /// <summary>
        /// "Show Cvar description" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_ShowHint_Click(object sender, EventArgs e)
        {
            try
            {
                if (CE_Editor.SelectedCells.Count > 0)
                {
                    DataGridViewCell Item = CE_Editor.Rows[CE_Editor.CurrentRow.Index].Cells[0];
                    if (!Item.OwningRow.IsNewRow && (Item.Value != null))
                    {
                        ShowVariableDescription(Item.Value.ToString());
                    }
                    else
                    {
                        MessageBox.Show(AppStrings.CE_ClSelErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(AppStrings.CE_NoSelection, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCvarDescFetchFailure);
                MessageBox.Show(AppStrings.CE_ClSelErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Show help system" menu click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUHelp_Click(object sender, EventArgs e)
        {
            HandleShowHelp(GetHelpWebPage());
        }

        /// <summary>
        /// "Visit official website" menu click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUWebsite_Click(object sender, EventArgs e)
        {
            try
            {
                App.Platform.OpenWebPage(Properties.Resources.AppURLWebsite);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExUrlWebsite);
                MessageBox.Show(AppStrings.AppVisitWebsiteError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Show Steam group" menu click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUSteamGroup_Click(object sender, EventArgs e)
        {
            try
            {
                App.Platform.StartRegularProcess(Properties.Resources.AppURLSteamGrID);
            }
            catch
            {
                try
                {
                    App.Platform.OpenWebPage(Properties.Resources.AppURLSteamGroup);
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExUrlGroup);
                    MessageBox.Show(AppStrings.AppVisitGroupError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// "Remove rows" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_RmRow_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewCell Cell in CE_Editor.SelectedCells)
                {
                    if (Cell.RowIndex != -1 && !Cell.OwningRow.IsNewRow)
                    {
                        CE_Editor.Rows.RemoveAt(Cell.RowIndex);
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdRemRow);
                MessageBox.Show(AppStrings.AppDeleteRowError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Copy selected rows" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_Copy_Click(object sender, EventArgs e)
        {
            try
            {
                HandleCopy();
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdCopy);
                MessageBox.Show(AppStrings.AppClipboardCopyError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Cut selected rows" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_Cut_Click(object sender, EventArgs e)
        {
            try
            {
                HandleCopy();
                HandleClearSelection();
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdCut);
                MessageBox.Show(AppStrings.AppClipboardCutError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Paste from clipboard" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_Paste_Click(object sender, EventArgs e)
        {
            try
            {
                if (CE_Editor.SelectedCells.Count == 1) { HandlePasteSingle(); } else { HandlePasteMultiple(); }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdPaste);
                MessageBox.Show(AppStrings.AppClipboardPasteError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Load FPS-config to editor" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void FP_Edit_Click(object sender, EventArgs e)
        {
            try
            {
                await EditFPSConfig(App.SourceGames[AppSelector.Text].IsUsingUserDir ? Path.Combine(App.SourceGames[AppSelector.Text].CFGMan.FPSConfigInstallPath, App.SourceGames[AppSelector.Text].CFGMan[FP_ConfigSel.Text].InstallDir, "cfg", "autoexec.cfg") : GuiHelpers.FormShowCfgSelect(App.SourceGames[AppSelector.Text].FPSConfigs), ModifierKeys != Keys.Shift);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExEditCfg);
                MessageBox.Show(AppStrings.FP_EditorLoadFailure, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Open Updater" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUUpdateCheck_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowUpdater(App.UserAgent, App.FullAppPath, App.AppUpdateDir);
        }

        /// <summary>
        /// "Refresh backups" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_Refresh_Click(object sender, EventArgs e)
        {
            UpdateBackUpList();
        }

        /// <summary>
        /// "Restore backup" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_Restore_Click(object sender, EventArgs e)
        {
            if (BU_LVTable.Items.Count > 0)
            {
                if (BU_LVTable.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show(AppStrings.BU_QMsg, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        RestoreSelectedBackUps();
                    }
                }
                else
                {
                    MessageBox.Show(AppStrings.BU_NoSelected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(AppStrings.BU_NoFiles, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Delete backup" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_Delete_Click(object sender, EventArgs e)
        {
            if (BU_LVTable.Items.Count > 0)
            {
                if (BU_LVTable.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show(AppStrings.BU_DelMsg, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        foreach (ListViewItem BU_Item in BU_LVTable.SelectedItems)
                        {
                            try
                            {
                                File.Delete(Path.Combine(App.SourceGames[AppSelector.Text].FullBackUpDirPath, BU_Item.SubItems[4].Text));
                                BU_LVTable.Items.Remove(BU_Item);
                            }
                            catch (Exception Ex)
                            {
                                Logger.Warn(Ex, DebugStrings.AppDbgExBackupRem);
                                MessageBox.Show(AppStrings.BU_DelFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        MessageBox.Show(AppStrings.BU_DelSuccessful, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(AppStrings.BU_NoSelected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(AppStrings.BU_NoFiles, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Load backup in text editor" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_TextEditor_Click(object sender, EventArgs e)
        {
            if (BU_LVTable.Items.Count > 0)
            {
                if (BU_LVTable.SelectedItems.Count > 0)
                {
                    if (Regex.IsMatch(Path.GetExtension(BU_LVTable.SelectedItems[0].SubItems[4].Text), @"\.(txt|cfg|[0-9]|reg)"))
                    {
                        try
                        {
                            App.Platform.OpenTextEditor(Path.Combine(App.SourceGames[AppSelector.Text].FullBackUpDirPath, BU_LVTable.SelectedItems[0].SubItems[4].Text), Properties.Settings.Default.EditorBin);
                        }
                        catch (Exception Ex)
                        {
                            Logger.Warn(Ex, DebugStrings.AppDbgExBkExtEdt);
                            MessageBox.Show(AppStrings.BU_OpenTextEditorFailed, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(AppStrings.BU_BinaryFile, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(AppStrings.BU_NoSelected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(AppStrings.BU_NoFiles, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Show backup file in file manager" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_ShowFile_Click(object sender, EventArgs e)
        {
            if (BU_LVTable.Items.Count > 0)
            {
                if (BU_LVTable.SelectedItems.Count > 0)
                {
                    try
                    {
                        App.Platform.OpenExplorer(Path.Combine(App.SourceGames[AppSelector.Text].FullBackUpDirPath, BU_LVTable.SelectedItems[0].SubItems[4].Text));
                    }
                    catch (Exception Ex)
                    {
                        Logger.Warn(Ex, DebugStrings.AppDbgExBkFMan);
                        MessageBox.Show(AppStrings.BU_ShowFileError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(AppStrings.BU_NoSelected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(AppStrings.BU_NoFiles, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Create backup" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_Create_ButtonClick(object sender, EventArgs e)
        {
            BUT_Create.ShowDropDown();
        }

        /// <summary>
        /// "Create game settings backup" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_GameSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppStrings.BU_VideoCreate, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    VideoSettingsBackup(true);
                    MessageBox.Show(AppStrings.BU_VideoDone, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateBackUpList();
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExBkSg);
                    MessageBox.Show(AppStrings.BU_VideoErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// "Create Steam registry settings backup" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BUT_RegSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppStrings.BU_RegCreate, Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    App.Platform.BackUpRegistrySettings(App.SourceGames[AppSelector.Text].FullBackUpDirPath);
                    MessageBox.Show(AppStrings.BU_RegDone, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateBackUpList();
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExBkAllStm);
                    MessageBox.Show(AppStrings.BU_RegErr, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// "Config editor table resized" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BU_LVTable_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            // Blocking resizing...
            e.NewWidth = BU_LVTable.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        /// <summary>
        /// "Form closed" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void FrmMainW_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Saving application settings...
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// "Open Keyboard buttons disabler" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUWinMnuDisabler_Click(object sender, EventArgs e)
        {
            try
            {
                App.Plugins["kbhelper"].Run(App.Platform);
            }
            catch (Win32Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgUACCancel);
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, DebugStrings.AppDbgExKbStart);
                MessageBox.Show(AppStrings.KB_StartError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// "Load config in text editor" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_OpenTextEditor_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CFGFileName))
            {
                try
                {
                    App.Platform.OpenTextEditor(CFGFileName, Properties.Settings.Default.EditorBin);
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdExtEdt);
                    MessageBox.Show(AppStrings.CE_OpenTextEditorError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(AppStrings.CE_NoFileOpened, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Restricted symbols in Steam path detector" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_PathDetector_Click(object sender, EventArgs e)
        {
            if (((Label)sender).ForeColor == Color.Red)
            {
                MessageBox.Show(AppStrings.SteamNonASCIIDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(AppStrings.SteamNonASCIINotDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// "Restricted symbols in game path detector" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_PathGame_Click(object sender, EventArgs e)
        {
            if (((Label)sender).ForeColor == Color.Red) { MessageBox.Show(AppStrings.GameNonASCIIDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning); } else { MessageBox.Show(AppStrings.GameNonASCIINotDetected, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        /// <summary>
        /// "Clean saved replays" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemReplays_Click(object sender, EventArgs e)
        {
            StartCleanup(6, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean custom models and textures" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemTextures_Click(object sender, EventArgs e)
        {
            StartCleanup(7, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean secondary cache contents" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemSecndCache_Click(object sender, EventArgs e)
        {
            StartCleanup(8, ((Button)sender).Text);
        }

        /// <summary>
        /// "Safe Clean status" icon click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void SB_App_DoubleClick(object sender, EventArgs e)
        {
            // Changing Safe Clean status...
            Properties.Settings.Default.SafeCleanup = !Properties.Settings.Default.SafeCleanup;

            // Showing message about consequences of disabling...
            if (!Properties.Settings.Default.SafeCleanup)
            {
                MessageBox.Show(AppStrings.AppSafeClnDisabled, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Updating status bar...
            CheckSafeClnStatus();
        }

        /// <summary>
        /// "Open the list of variables and functions" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_OpenCVList_Click(object sender, EventArgs e)
        {
            try
            {
                App.Platform.OpenWebPage(AppStrings.AppCVListURL);
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExUrlCvList);
                MessageBox.Show(AppStrings.AppVisitCVListError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Create a backup of the config file" toolbar button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CE_ManualBackUpCfg_Click(object sender, EventArgs e)
        {
            if (!(string.IsNullOrEmpty(CFGFileName)))
            {
                if (File.Exists(CFGFileName))
                {
                    try
                    {
                        FileManager.CreateConfigBackUp(CFGFileName, App.SourceGames[AppSelector.Text].FullBackUpDirPath, Properties.Resources.BU_PrefixCfg);
                        MessageBox.Show(string.Format(AppStrings.CE_BackUpCreated, Path.GetFileName(CFGFileName)), Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception Ex)
                    {
                        Logger.Warn(Ex, DebugStrings.AppDbgExCfgEdBkMan);
                        MessageBox.Show(AppStrings.CE_ManualBackUpError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show(AppStrings.CE_NoFileOpened, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Clean custom sounds" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemSounds_Click(object sender, EventArgs e)
        {
            StartCleanup(9, ((Button)sender).Text);
        }

        /// <summary>
        /// "Clean custom directory contents" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemCustDir_Click(object sender, EventArgs e)
        {
            StartCleanup(10, ((Button)sender).Text);
        }

        /// <summary>
        /// "Execute deep cleanup" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_DeepCleanup_Click(object sender, EventArgs e)
        {
            List<string> CleanDirs = new List<string>(App.SourceGames[AppSelector.Text].CloudConfigs);
            if (App.SourceGames[AppSelector.Text].IsUsingVideoFile)
            {
                CleanDirs.AddRange(App.SourceGames[AppSelector.Text].VideoCfgFiles);
            }
            StartCleanup(11, ((Button)sender).Text, CleanupItem.CreateFromLegacyList(CleanDirs, false, false));
        }

        /// <summary>
        /// "Clean FPS-configs" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PS_RemConfigs_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowCleanup(CleanupItem.CreateFromLegacyList(App.SourceGames[AppSelector.Text].FPSConfigs, false, true), ((Button)sender).Text.ToLower(CultureInfo.CurrentUICulture), AppStrings.PS_CleanupSuccess, App.SourceGames[AppSelector.Text].FullBackUpDirPath, App.SourceGames[AppSelector.Text].GameBinaryFile, false, false, Properties.Settings.Default.SafeCleanup);
            HandleConfigs();
        }

        /// <summary>
        /// "Selected HUD changed" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void HD_HSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Checking result...
            bool Success = !string.IsNullOrEmpty(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].Name);

            // Changing some controls state...
            HD_GB_Pbx.Image = Properties.Resources.ImageLoadingFile;
            HD_Install.Enabled = Success;
            HD_Homepage.Enabled = Success;
            HD_Warning.Visible = Success && !App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].IsUpdated;

            // Adding information about last update...
            HD_LastUpdate.Visible = Success;
            if (Success)
            {
                HD_LastUpdate.Text = string.Format(AppStrings.HD_LastUpdateInfo, FileManager.Unix2DateTime(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].LastUpdate).ToLocalTime());
            }

            // Checking if selected HUD is installed...
            SetHUDButtons(HUDManager.CheckInstalledHUD(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir));

            // Downloading screenshot...
            if (Success)
            {
                await HandleHUDScreenshot();
            }
        }

        /// <summary>
        /// "Install selected HUD" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void HD_Install_Click(object sender, EventArgs e)
        {
            if (App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].IsUpdated)
            {
                if (MessageBox.Show(string.Format("{0}?", ((Button)sender).Text), Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    // Downloading HUD archive...
                    bool DownloadResult = DownloadHUD();

                    // If cannot download from the main server, let's try mirrors...
                    if (!DownloadResult)
                    {
                        Logger.Warn(DebugStrings.AppDbgHUDDnlMain);
                        DownloadResult = DownloadHUD(true);
                    }

                    // Installing downloaded HUD...
                    if (DownloadResult)
                    {
                        await InstallHUD();
                    }
                    else
                    {
                        Logger.Error(DebugStrings.AppDbgHUDDnlMirror);
                        MessageBox.Show(AppStrings.HD_DownloadError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(AppStrings.HD_Outdated, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Uninstall selected HUD" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void HD_Uninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("{0}?", ((Button)sender).Text), Properties.Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                // Generating full path...
                string HUDPath = Path.Combine(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir);

                // Removing HUD files...
                GuiHelpers.FormShowRemoveFiles(HUDPath);

                // Checking if HUD installed...
                bool IsInstalled = HUDManager.CheckInstalledHUD(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir);

                // Showing message and removing empty directory...
                if (!IsInstalled)
                {
                    MessageBox.Show(AppStrings.PS_CleanupSuccess, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Changing the state of some controls...
                SetHUDButtons(IsInstalled);
            }
        }

        /// <summary>
        /// "Visit HUD's homepage" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void HD_Homepage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].Site))
            {
                try
                {
                    App.Platform.OpenWebPage(App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].Site);
                }
                catch (Exception Ex)
                {
                    Logger.Warn(Ex, DebugStrings.AppDbgExUrlHudHome);
                    MessageBox.Show(AppStrings.HD_HomepageError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// "Clean downloaded by application files" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUExtClnAppCache_Click(object sender, EventArgs e)
        {
            List<CleanupItem> CleanDirs = new List<CleanupItem>
            {
                new CleanupItem(App.AppCfgDir),
                new CleanupItem(App.AppHUDDir),
                new CleanupItem(App.AppUpdateDir)
            };
            GuiHelpers.FormShowCleanup(CleanDirs, ((ToolStripMenuItem)sender).Text.ToLower(CultureInfo.CurrentUICulture).Replace("&", string.Empty), AppStrings.PS_CleanupSuccess, App.SourceGames[AppSelector.Text].FullBackUpDirPath, App.SourceGames[AppSelector.Text].GameBinaryFile);
        }

        /// <summary>
        /// "Clean system temporary files" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUExtClnTmpDir_Click(object sender, EventArgs e)
        {
            List<CleanupItem> CleanDirs = new List<CleanupItem>
            {
                new CleanupItem(Path.GetTempPath())
            };
            GuiHelpers.FormShowCleanup(CleanDirs, ((ToolStripMenuItem)sender).Text.ToLower(CultureInfo.CurrentUICulture).Replace("&", string.Empty), AppStrings.PS_CleanupSuccess, App.SourceGames[AppSelector.Text].FullBackUpDirPath, App.SourceGames[AppSelector.Text].GameBinaryFile);
        }

        /// <summary>
        /// "Show Log viewer" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUShowLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(App.AppLogFile))
            {
                GuiHelpers.FormShowLogViewer(App.AppLogFile);
            }
            else
            {
                MessageBox.Show(AppStrings.AppNoDebugFile, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Outdated HUD warning" icon click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void HD_Warning_Click(object sender, EventArgs e)
        {
            if (!App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].IsUpdated)
            {
                MessageBox.Show(AppStrings.HD_NotTested, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Show HUD files in shell" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void HD_OpenDir_Click(object sender, EventArgs e)
        {
            try
            {
                App.Platform.OpenExplorer(Path.Combine(App.SourceGames[AppSelector.Text].CustomInstallDir, App.SourceGames[AppSelector.Text].HUDMan[HD_HSel.Text].InstallDir));
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExHudExtFm);
                MessageBox.Show(AppStrings.HD_OpenDirError, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Open Steam cache cleaner" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUExtClnSteam_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowStmCleaner(App.SteamClient.FullSteamPath, App.SourceGames[AppSelector.Text].FullBackUpDirPath);
        }

        /// <summary>
        /// "Open Muted players manager" menu item click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void MNUMuteMan_Click(object sender, EventArgs e)
        {
            GuiHelpers.FormShowMuteManager(App.SourceGames[AppSelector.Text].GetActualBanlistFile(), App.SourceGames[AppSelector.Text].FullBackUpDirPath);
        }

        /// <summary>
        /// "Current Steam UserID" status bar click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void SB_SteamID_Click(object sender, EventArgs e)
        {
            try
            {
                string Result = GuiHelpers.FormShowIDSelect(App.SteamClient.SteamIDs);

                if (!string.IsNullOrWhiteSpace(Result))
                {
                    SB_SteamID.Text = Result;
                    Properties.Settings.Default.LastSteamID = Result;
                    FindGames();
                }
            }
            catch (Exception Ex)
            {
                Logger.Warn(Ex, DebugStrings.AppDbgExUserIdSel);
                MessageBox.Show(AppStrings.SD_NotEnoughStmIDs, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// "Backup file selected" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void BU_LVTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Blocking some buttons if user selected more than one backup file...
            bool IsSingle = BU_LVTable.SelectedItems.Count <= 1;
            BUT_TextEditor.Enabled = IsSingle;
            BUT_ShowFile.Enabled = IsSingle;
        }
    }
}
