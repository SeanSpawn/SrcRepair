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
using System.IO;
using System.Xml;
using System.Collections.Generic;
using NLog;

namespace srcrepair.core
{
    /// <summary>
    /// Класс для работы с коллекцией HUD.
    /// </summary>
    public sealed class HUDManager
    {
        /// <summary>
        /// Управляет записью событий в журнал.
        /// </summary>
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Хранит информацию о всех доступных HUD.
        /// </summary>
        private List<HUDTlx> HUDsAvailable;
        
        /// <summary>
        /// Хранит информацию о выбранном HUD. Для заполнения используется метод Select().
        /// </summary>
        public HUDTlx SelectedHUD { get; private set; }

        /// <summary>
        /// Выбирает определённый HUD.
        /// </summary>
        /// <param name="HUDName">Имя HUD, информацию о котором надо получить</param>
        public void Select(string HUDName)
        {
            SelectedHUD = HUDsAvailable.Find(Item => String.Equals(Item.Name, HUDName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Получает имена найденных HUD для указанной игры.
        /// </summary>
        /// <param name="GameName">ID игры</param>
        /// <returns>Возвращает имена найденных HUD</returns>
        public List<String> GetHUDNames(string GameName)
        {
            // Инициализируем список...
            List<String> Result = new List<String>();

            // Выполняем запрос...
            foreach (HUDTlx HUD in HUDsAvailable.FindAll(Item => String.Equals(Item.Game, GameName, StringComparison.CurrentCultureIgnoreCase)))
            {
                Result.Add(HUD.Name);
            }

            // Возвращаем результат...
            return Result;
        }

        /// <summary>
        /// Проверяет установлен ли указанный HUD.
        /// </summary>
        /// <param name="CustomInstallDir">Каталог установки кастомных файлов</param>
        /// <param name="HUDDir">Каталог установки проверяемого HUD</param>
        /// <returns>Возвращает истину если HUD с указанным именем установлен</returns>
        public static bool CheckInstalledHUD(string CustomInstallDir, string HUDDir)
        {
            // Описываем локальные переменные...
            bool Result = false;
            string HUDPath = Path.Combine(CustomInstallDir, HUDDir);

            // Проверим существование каталога...
            if (Directory.Exists(HUDPath))
            {
                // Проверим наличие файлов или каталогов внутри...
                using (IEnumerator<String> StrEn = Directory.EnumerateFileSystemEntries(HUDPath).GetEnumerator())
                {
                    Result = StrEn.MoveNext();
                }
            }

            // Возвращаем результат...
            return Result;
        }

        /// <summary>
        /// Форматирует путь в соответствии с типом ОС.
        /// </summary>
        /// <param name="IntDir">Исходное значение</param>
        /// <returns>Отформатированное значение</returns>
        public static string FormatIntDir(string IntDir)
        {
            return IntDir.Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Проверяет актуальность базы HUD.
        /// </summary>
        /// <param name="LastHUDUpdate">Дата последней проверки обновлений базы HUD</param>
        /// <returns>Булево актуальности базы HUD</returns>
        public static bool CheckHUDDatabase(DateTime LastHUDUpdate)
        {
            return (DateTime.Now - LastHUDUpdate).Days >= 7;
        }

        /// <summary>
        /// Конструктор класса. Читает базу данных в формате XML и заполняет нашу структуру.
        /// </summary>
        /// <param name="HUDDbFile">Путь к БД HUD</param>
        /// <param name="AppHUDDir">Путь к локальному каталогу с HUD</param>
        /// <param name="HideOutdated">Скрывать устаревшие HUD</param>
        public HUDManager(string HUDDbFile, string AppHUDDir, bool HideOutdated)
        {
            // Инициализируем наш список...
            HUDsAvailable = new List<HUDTlx>();

            // Получаем полный список доступных HUD для данной игры. Открываем поток...
            using (FileStream XMLFS = new FileStream(HUDDbFile, FileMode.Open, FileAccess.Read))
            {
                // Загружаем XML из потока...
                XmlDocument XMLD = new XmlDocument();
                XMLD.Load(XMLFS);

                // Разбираем XML файл и обходим его в цикле...
                for (int i = 0; i < XMLD.GetElementsByTagName("HUD").Count; i++)
                {
                    try
                    {
                        if (!HideOutdated || XMLD.GetElementsByTagName("IsUpdated")[i].InnerText == "1")
                        {
                            HUDsAvailable.Add(new HUDTlx(AppHUDDir, XMLD.GetElementsByTagName("Name")[i].InnerText, XMLD.GetElementsByTagName("Game")[i].InnerText, XMLD.GetElementsByTagName("URI")[i].InnerText, XMLD.GetElementsByTagName("UpURI")[i].InnerText, XMLD.GetElementsByTagName("IsUpdated")[i].InnerText == "1", XMLD.GetElementsByTagName("Preview")[i].InnerText, XMLD.GetElementsByTagName("LastUpdate")[i].InnerText, XMLD.GetElementsByTagName("Site")[i].InnerText, XMLD.GetElementsByTagName("ArchiveDir")[i].InnerText, XMLD.GetElementsByTagName("InstallDir")[i].InnerText, XMLD.GetElementsByTagName("Hash")[i].InnerText, Path.Combine(AppHUDDir, Path.ChangeExtension(Path.GetFileName(XMLD.GetElementsByTagName("Name")[i].InnerText), ".zip"))));
                        }
                    }
                    catch (Exception Ex)
                    {
                        Logger.Warn(Ex, "Minor exception while building HUD list object.");
                    }
                }
            }
        }
    }
}
