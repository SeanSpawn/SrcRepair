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
using System.Drawing;
using System.Windows.Forms;

namespace srcrepair.core
{
    /// <summary>
    /// Class with hacks for high pixel density displays.
    /// </summary>
    public static class DpiManager
    {
        /// <summary>
        /// Correctly scale columns width in DataGridView container on high
        /// pixel density displays.
        /// </summary>
        /// <param name="ScaleSource">DataGridView control name.</param>
        /// <param name="ScaleFactor">Scale factor value.</param>
        public static void ScaleColumnsInControl(DataGridView ScaleSource, SizeF ScaleFactor)
        {
            foreach (DataGridViewColumn Column in ScaleSource.Columns)
            {
                Column.Width = (int)Math.Round(Column.Width * ScaleFactor.Width);
            }
        }

        /// <summary>
        /// Correctly scale columns width in ListView container on high
        /// pixel density displays.
        /// </summary>
        /// <param name="ScaleSource">ListView control name.</param>
        /// <param name="ScaleFactor">Scale factor value.</param>
        public static void ScaleColumnsInControl(ListView ScaleSource, SizeF ScaleFactor)
        {
            foreach (ColumnHeader Column in ScaleSource.Columns)
            {
                Column.Width = (int)Math.Round(Column.Width * ScaleFactor.Width);
            }
        }

        /// <summary>
        /// Correctly scale columns width in StatusStrip container on high
        /// pixel density displays.
        /// </summary>
        /// <param name="ScaleSource">StatusStrip control name.</param>
        /// <param name="ScaleFactor">Scale factor value.</param>
        public static void ScaleColumnsInControl(StatusStrip ScaleSource, SizeF ScaleFactor)
        {
            foreach (ToolStripItem StatusBarItem in ScaleSource.Items)
            {
                StatusBarItem.Width = (int)Math.Round(StatusBarItem.Width * ScaleFactor.Width);
            }
        }

        /// <summary>
        /// Compare two floating point numbers.
        /// </summary>
        /// <param name="First">First floating point number.</param>
        /// <param name="Second">Second floating point number.</param>
        /// <returns>Comparison result.</returns>
        public static bool CompareFloats(float First, float Second)
        {
            return Math.Abs(First - Second) < 0.00001f;
        }
    }
}
