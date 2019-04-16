﻿/*
 * This file is a part of SRC Repair project. For more information
 * visit official site: https://www.easycoding.org/projects/srcrepair
 * 
 * Copyright (c) 2011 - 2019 EasyCoding Team (ECTeam).
 * Copyright (c) 2005 - 2019 EasyCoding Team.
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
using System.Collections.Generic;
using System.IO;

namespace srcrepair
{
    public sealed class SourceGame
    {
        /// <summary>
        /// В этой переменной будем хранить путь установки кастомных файлов.
        /// </summary>
        public string CustomInstallDir { get; private set; }

        /// <summary>
        /// В этой переменной будем хранить полный путь к каталогу игры, которой
        /// мы будем управлять данной утилитой.
        /// </summary>
        public string FullGamePath { get; private set; }

        /// <summary>
        /// В этой переменной будем хранить полный путь к каталогу игры без
        /// включения в путь GV.SmallAppName для служебных целей.
        /// </summary>
        public string GamePath { get; private set; }

        /// <summary>
        /// В этой переменной будем хранить полное имя управляемого приложения
        /// для служебных целей.
        /// </summary>
        public string FullAppName { get; private set; }

        /// <summary>
        /// В этой переменной мы будем хранить краткое имя управляемого приложения
        /// для служебных целей (SteamAlias).
        /// </summary>
        public string SmallAppName { get; private set; }

        /// <summary>
        /// В этой переменной мы будем хранить имя главного процесса игры.
        /// </summary>
        public string GameBinaryFile { get; private set; }

        /// <summary>
        /// В этой переменной мы будем пути к каталогам с облачными конфигами.
        /// </summary>
        public List<String> CloudConfigs { get; private set; }

        /// <summary>
        /// В этой переменной мы будем хранить полный путь до каталога с
        /// файлами конфигурации управляемого приложения.
        /// </summary>
        public string FullCfgPath { get; private set; }

        /// <summary>
        /// В этой переменной мы будем хранить полный путь до каталога с
        /// резервными копиями управляемого приложения.
        /// </summary>
        public string FullBackUpDirPath { get; private set; }

        /// <summary>
        /// Указывает использует ли игра файл video.txt для хранения
        /// своих настроек.
        /// </summary>
        public bool IsUsingVideoFile { get; private set; }

        /// <summary>
        /// Определяет использует ли игра специальный каталог для хранения
        /// пользовательских настроек и скриптов.
        /// </summary>
        public bool IsUsingUserDir { get; private set; }

        /// <summary>
        /// Указывает поддерживает ли конкретная игра кастомные HUD и имеется
        /// ли их поддержка в SRC Repair.
        /// </summary>
        public bool IsHUDsAvailable { get; private set; }

        /// <summary>
        /// Эта переменная хранит ID игры по базе данных Steam. Используется
        /// для служебных целей.
        /// </summary>
        public string GameInternalID { get; private set; }

        /// <summary>
        /// Хранит тип механизма хранения настроек движка Source.
        /// </summary>
        public string SourceType { get; private set; }

        /// <summary>
        /// В этом списке хранятся пути ко всем найденным файлами
        /// с графическими настройками.
        /// </summary>
        public List<String> VideoCfgFiles { get; private set; }

        /// <summary>
        /// Содержит имя каталога, либо ключа реестра с графическими
        /// настройками.
        /// </summary>
        public string ConfDir { get; private set; }

        /// <summary>
        /// В этой переменной будем хранить путь до каталога локального хранения
        /// загруженных файлов HUD и их мета-информации.
        /// </summary>
        public string AppHUDDir { get; private set; }

        /// <summary>
        /// Содержит пути к установленным FPS-конфигам управляемой игры.
        /// </summary>
        public List<String> FPSConfigs { get; set; }

        /// <summary>
        /// Содержит список доступных HUD для управляемой игры.
        /// </summary>
        public HUDManager HUDMan { get; set; }

        /// <summary>
        /// Содержит список доступных HUD для управляемой игры.
        /// </summary>
        public ConfigManager CFGMan { get; set; }

        /// <summary>
        /// Содержит путь к каталогу с загруженными данными из Steam Workshop.
        /// </summary>
        public string AppWorkshopDir { get; private set; }

        /// <summary>
        /// Содержит путь к файлу со списком заглушенных пользователей.
        /// </summary>
        public List<String> BanlistFiles { get; private set; }

        /// <summary>
        /// Указывает установлено ли данное приложение.
        /// </summary>
        public bool IsInstalled { get; private set; }

        /// <summary>
        /// Содержит путь к установленному клиенту Steam.
        /// </summary>
        private string SteamPath { get; set; }

        /// <summary>
        /// Содержит выбранный SteamID пользователя.
        /// </summary>
        private string SteamID { get; set; }

        /// <summary>
        /// Генерирует путь к каталогу установки игры.
        /// </summary>
        /// <param name="AppName">Имя каталога приложения</param>
        /// <param name="GameDirs">Возможные каталоги установки</param>
        /// <param name="OSType">Платформа ОС, под которой запущено приложение</param>
        /// <returns>Возвращает путь к каталогу игры или пустую строку</returns>
        private string GetGameDirectory(string AppName, List<String> GameDirs, CurrentPlatform.OSType OSType)
        {
            foreach (string Dir in GameDirs)
            {
                string GamePath = Path.Combine(Dir, AppName);
                if (Directory.Exists(Path.Combine(GamePath, SmallAppName)) && (File.Exists(Path.Combine(GamePath, GameBinaryFile)) || OSType != CurrentPlatform.OSType.Windows))
                {
                    return GamePath;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Ищет все доступные конфиги, хранящиеся в Cloud или его локальной копии.
        /// </summary>
        /// <param name="Mask">Маска файлов для поиска</param>
        /// <returns>Возвращает список найденных файлов с графическими настройками</returns>
        private List<String> GetCloudConfigs(string Mask = "*.*cfg")
        {
            return FileManager.FindFiles(Path.Combine(SteamPath, "userdata", SteamID, GameInternalID), Mask);
        }

        /// <summary>
        /// Обновляет список файлов с графическими настройками выбранной игры.
        /// </summary>
        private void UpdateVideoFilesList()
        {
            // Ищем файлы с графическими настройками из локального хранилища...
            VideoCfgFiles = GetCloudConfigs("video.txt");
            
            // Добавляем в базу Legacy конфиг...
            VideoCfgFiles.Add(Path.Combine(GamePath, ConfDir, "cfg", "video.txt"));
        }

        /// <summary>
        /// Обновляет список файлов с заблокированными пользователями выбранной игры.
        /// </summary>
        private void UpdateBanlistFilesList()
        {
            BanlistFiles = GetCloudConfigs("voice_ban.dt");
            BanlistFiles.Add(Path.Combine(FullGamePath, "voice_ban.dt"));
        }

        /// <summary>
        /// Возвращает актуальный файл графических настроек игры.
        /// </summary>
        public string GetActualVideoFile()
        {
            return FileManager.FindNewerestFile(VideoCfgFiles);
        }

        /// <summary>
        /// Возвращает актуальный файл с базой заблокированных игроков.
        /// </summary>
        public string GetActualBanlistFile()
        {
            return FileManager.FindNewerestFile(BanlistFiles);
        }

        /// <summary>
        /// Конструктор класса. Заполняет информацию о выбранном приложении.
        /// </summary>
        /// <param name="AppName">Название приложения (из БД)</param>
        /// <param name="DirName">Каталог приложения (из БД)</param>
        /// <param name="SmallName">Внутренний каталог приложения (из БД)</param>
        /// <param name="Executable">Имя главного бинарника (из БД)</param>
        /// <param name="SID">Внутренний ID приложения в Steam (из БД)</param>
        /// <param name="SV">Механизм хранения настроек движка (из БД)</param>
        /// <param name="VFDir">Каталог хранения графических настроек (из БД)</param>
        /// <param name="HasVF">Задаёт формат приложения: GCF/NCF (из БД)</param>
        /// <param name="UserDir">Указывает использует ли приложение кастомный каталог (из БД)</param>
        /// <param name="AppPath">Путь к каталогу SRC Repair</param>
        /// <param name="AUserDir">Путь к каталогу с данными SRC Repair</param>
        /// <param name="UserDir">Путь к пользовательскому каталогу SRC Repair</param>
        /// <param name="SteamDir">Путь к установленному клиенту Steam</param>
        /// <param name="SteamAppsDirName">Платформо-зависимое название каталога SteamApps</param>
        /// <param name="OS">Платформа ОС, под которой запущено приложение</param>
        public SourceGame(string AppName, string DirName, string SmallName, string Executable, string SID, string SV, string VFDir, bool HasVF, bool UserDir, bool HUDAv, string AppPath, string AUserDir, string SteamDir, string SteamAppsDirName, string SelectedSteamID, List<String> GameDirs, CurrentPlatform.OSType OS)
        {
            // Начинаем определять нужные нам значения переменных...
            FullAppName = AppName;
            SmallAppName = SmallName;
            GameBinaryFile = Executable;
            GameInternalID = SID;
            SourceType = SV;
            ConfDir = VFDir;
            IsUsingVideoFile = HasVF;
            IsUsingUserDir = UserDir;
            IsHUDsAvailable = HUDAv;
            SteamPath = SteamDir;

            // Получаем полный путь до каталога управляемого приложения...
            GamePath = GetGameDirectory(DirName, GameDirs, OS);

            // Проверяем установлено ли приложение...
            IsInstalled = !String.IsNullOrWhiteSpace(GamePath);

            // Заполняем остальные свойства класса если приложение установлено...
            if (IsInstalled)
            {
                SteamID = SelectedSteamID;
                FullGamePath = Path.Combine(GamePath, SmallAppName);
                FullCfgPath = Path.Combine(FullGamePath, "cfg");
                FullBackUpDirPath = Path.Combine(AUserDir, "backups", Path.GetFileName(SmallAppName));
                AppHUDDir = Path.Combine(AUserDir, Properties.Resources.HUDLocalDir, SmallAppName);
                CustomInstallDir = Path.Combine(FullGamePath, IsUsingUserDir ? "custom" : String.Empty);
                AppWorkshopDir = Path.Combine(SteamDir, SteamAppsDirName, Properties.Resources.WorkshopFolderName, "content", GameInternalID);
                if (IsUsingVideoFile) { UpdateVideoFilesList(); }
                UpdateBanlistFilesList();
                CloudConfigs = GetCloudConfigs();
            }
        }
    }
}
