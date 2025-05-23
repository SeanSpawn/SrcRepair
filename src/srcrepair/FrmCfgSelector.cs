﻿/**
 * SPDX-FileCopyrightText: 2011-2025 EasyCoding Team
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace srcrepair.gui
{
    /// <summary>
    /// Class of the config file selection module.
    /// </summary>
    public partial class FrmCfgSelector : Form
    {
        /// <summary>
        /// Stores the list of available config files.
        /// </summary>
        private readonly List<string> Configs;

        /// <summary>
        /// Gets or sets the full path to the user-selected config file.
        /// </summary>
        public string Config { get; private set; }

        /// <summary>
        /// FrmCfgSelector class constructor.
        /// </summary>
        /// <param name="Cfgs">The list of available config files.</param>
        public FrmCfgSelector(List<string> Cfgs)
        {
            InitializeComponent();
            Configs = Cfgs;
        }

        /// <summary>
        /// "Form create" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void FrmCfgSelector_Load(object sender, EventArgs e)
        {
            CS_CfgSel.DataSource = Configs;
        }

        /// <summary>
        /// "OK" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CS_OK_Click(object sender, EventArgs e)
        {
            Config = CS_CfgSel.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// "Cancel" button click event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CS_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// "Config file selected" event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CS_CfgSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Setting the full config file path to the tooltip...
            CS_ToolTip.SetToolTip((ComboBox)sender, ((ComboBox)sender).Text);
        }
    }
}
