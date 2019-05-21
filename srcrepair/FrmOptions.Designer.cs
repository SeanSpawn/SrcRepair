﻿namespace srcrepair.gui
{
    partial class FrmOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOptions));
            this.MO_Okay = new System.Windows.Forms.Button();
            this.MO_Cancel = new System.Windows.Forms.Button();
            this.MO_TC = new System.Windows.Forms.TabControl();
            this.MO_TP1 = new System.Windows.Forms.TabPage();
            this.MO_UseUpstream = new System.Windows.Forms.CheckBox();
            this.MO_RemEmptyDirs = new System.Windows.Forms.CheckBox();
            this.MO_ZipCompress = new System.Windows.Forms.CheckBox();
            this.MO_HighlightOldBackUps = new System.Windows.Forms.CheckBox();
            this.MO_HideUnsupported = new System.Windows.Forms.CheckBox();
            this.MO_ConfirmExit = new System.Windows.Forms.CheckBox();
            this.MO_TP2 = new System.Windows.Forms.TabPage();
            this.MO_HideOutdatedHUDs = new System.Windows.Forms.CheckBox();
            this.MO_AutoCheckUpdates = new System.Windows.Forms.CheckBox();
            this.MO_UnSafeOps = new System.Windows.Forms.CheckBox();
            this.MO_CustDirName = new System.Windows.Forms.TextBox();
            this.L_MO_CustDirName = new System.Windows.Forms.Label();
            this.MO_FindTextEd = new System.Windows.Forms.Button();
            this.MO_TextEdBin = new System.Windows.Forms.TextBox();
            this.L_MO_TextEdBin = new System.Windows.Forms.Label();
            this.MO_SearchBin = new System.Windows.Forms.OpenFileDialog();
            this.MO_TC.SuspendLayout();
            this.MO_TP1.SuspendLayout();
            this.MO_TP2.SuspendLayout();
            this.SuspendLayout();
            // 
            // MO_Okay
            // 
            resources.ApplyResources(this.MO_Okay, "MO_Okay");
            this.MO_Okay.Name = "MO_Okay";
            this.MO_Okay.UseVisualStyleBackColor = true;
            this.MO_Okay.Click += new System.EventHandler(this.MO_Okay_Click);
            // 
            // MO_Cancel
            // 
            this.MO_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.MO_Cancel, "MO_Cancel");
            this.MO_Cancel.Name = "MO_Cancel";
            this.MO_Cancel.UseVisualStyleBackColor = true;
            this.MO_Cancel.Click += new System.EventHandler(this.MO_Cancel_Click);
            // 
            // MO_TC
            // 
            this.MO_TC.Controls.Add(this.MO_TP1);
            this.MO_TC.Controls.Add(this.MO_TP2);
            resources.ApplyResources(this.MO_TC, "MO_TC");
            this.MO_TC.Name = "MO_TC";
            this.MO_TC.SelectedIndex = 0;
            // 
            // MO_TP1
            // 
            this.MO_TP1.Controls.Add(this.MO_UseUpstream);
            this.MO_TP1.Controls.Add(this.MO_RemEmptyDirs);
            this.MO_TP1.Controls.Add(this.MO_ZipCompress);
            this.MO_TP1.Controls.Add(this.MO_HighlightOldBackUps);
            this.MO_TP1.Controls.Add(this.MO_HideUnsupported);
            this.MO_TP1.Controls.Add(this.MO_ConfirmExit);
            resources.ApplyResources(this.MO_TP1, "MO_TP1");
            this.MO_TP1.Name = "MO_TP1";
            this.MO_TP1.UseVisualStyleBackColor = true;
            // 
            // MO_UseUpstream
            // 
            resources.ApplyResources(this.MO_UseUpstream, "MO_UseUpstream");
            this.MO_UseUpstream.Name = "MO_UseUpstream";
            this.MO_UseUpstream.UseVisualStyleBackColor = true;
            // 
            // MO_RemEmptyDirs
            // 
            resources.ApplyResources(this.MO_RemEmptyDirs, "MO_RemEmptyDirs");
            this.MO_RemEmptyDirs.Name = "MO_RemEmptyDirs";
            this.MO_RemEmptyDirs.UseVisualStyleBackColor = true;
            // 
            // MO_ZipCompress
            // 
            resources.ApplyResources(this.MO_ZipCompress, "MO_ZipCompress");
            this.MO_ZipCompress.Name = "MO_ZipCompress";
            this.MO_ZipCompress.UseVisualStyleBackColor = true;
            // 
            // MO_HighlightOldBackUps
            // 
            resources.ApplyResources(this.MO_HighlightOldBackUps, "MO_HighlightOldBackUps");
            this.MO_HighlightOldBackUps.Name = "MO_HighlightOldBackUps";
            this.MO_HighlightOldBackUps.UseVisualStyleBackColor = true;
            // 
            // MO_HideUnsupported
            // 
            resources.ApplyResources(this.MO_HideUnsupported, "MO_HideUnsupported");
            this.MO_HideUnsupported.Name = "MO_HideUnsupported";
            this.MO_HideUnsupported.UseVisualStyleBackColor = true;
            // 
            // MO_ConfirmExit
            // 
            resources.ApplyResources(this.MO_ConfirmExit, "MO_ConfirmExit");
            this.MO_ConfirmExit.Name = "MO_ConfirmExit";
            this.MO_ConfirmExit.UseVisualStyleBackColor = true;
            // 
            // MO_TP2
            // 
            this.MO_TP2.Controls.Add(this.MO_HideOutdatedHUDs);
            this.MO_TP2.Controls.Add(this.MO_AutoCheckUpdates);
            this.MO_TP2.Controls.Add(this.MO_UnSafeOps);
            this.MO_TP2.Controls.Add(this.MO_CustDirName);
            this.MO_TP2.Controls.Add(this.L_MO_CustDirName);
            this.MO_TP2.Controls.Add(this.MO_FindTextEd);
            this.MO_TP2.Controls.Add(this.MO_TextEdBin);
            this.MO_TP2.Controls.Add(this.L_MO_TextEdBin);
            resources.ApplyResources(this.MO_TP2, "MO_TP2");
            this.MO_TP2.Name = "MO_TP2";
            this.MO_TP2.UseVisualStyleBackColor = true;
            // 
            // MO_HideOutdatedHUDs
            // 
            resources.ApplyResources(this.MO_HideOutdatedHUDs, "MO_HideOutdatedHUDs");
            this.MO_HideOutdatedHUDs.Name = "MO_HideOutdatedHUDs";
            this.MO_HideOutdatedHUDs.UseVisualStyleBackColor = true;
            // 
            // MO_AutoCheckUpdates
            // 
            resources.ApplyResources(this.MO_AutoCheckUpdates, "MO_AutoCheckUpdates");
            this.MO_AutoCheckUpdates.Name = "MO_AutoCheckUpdates";
            this.MO_AutoCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // MO_UnSafeOps
            // 
            resources.ApplyResources(this.MO_UnSafeOps, "MO_UnSafeOps");
            this.MO_UnSafeOps.Name = "MO_UnSafeOps";
            this.MO_UnSafeOps.UseVisualStyleBackColor = true;
            // 
            // MO_CustDirName
            // 
            resources.ApplyResources(this.MO_CustDirName, "MO_CustDirName");
            this.MO_CustDirName.Name = "MO_CustDirName";
            this.MO_CustDirName.TextChanged += new System.EventHandler(this.MO_CustDirName_TextChanged);
            // 
            // L_MO_CustDirName
            // 
            resources.ApplyResources(this.L_MO_CustDirName, "L_MO_CustDirName");
            this.L_MO_CustDirName.Name = "L_MO_CustDirName";
            // 
            // MO_FindTextEd
            // 
            this.MO_FindTextEd.Image = global::srcrepair.Properties.Resources.Search;
            resources.ApplyResources(this.MO_FindTextEd, "MO_FindTextEd");
            this.MO_FindTextEd.Name = "MO_FindTextEd";
            this.MO_FindTextEd.UseVisualStyleBackColor = true;
            this.MO_FindTextEd.Click += new System.EventHandler(this.MO_FindTextEd_Click);
            // 
            // MO_TextEdBin
            // 
            resources.ApplyResources(this.MO_TextEdBin, "MO_TextEdBin");
            this.MO_TextEdBin.Name = "MO_TextEdBin";
            // 
            // L_MO_TextEdBin
            // 
            resources.ApplyResources(this.L_MO_TextEdBin, "L_MO_TextEdBin");
            this.L_MO_TextEdBin.Name = "L_MO_TextEdBin";
            // 
            // MO_SearchBin
            // 
            resources.ApplyResources(this.MO_SearchBin, "MO_SearchBin");
            // 
            // FrmOptions
            // 
            this.AcceptButton = this.MO_Okay;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.MO_Cancel;
            this.ControlBox = false;
            this.Controls.Add(this.MO_TC);
            this.Controls.Add(this.MO_Cancel);
            this.Controls.Add(this.MO_Okay);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOptions";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FrmOptions_Load);
            this.MO_TC.ResumeLayout(false);
            this.MO_TP1.ResumeLayout(false);
            this.MO_TP2.ResumeLayout(false);
            this.MO_TP2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button MO_Okay;
        private System.Windows.Forms.Button MO_Cancel;
        private System.Windows.Forms.TabControl MO_TC;
        private System.Windows.Forms.TabPage MO_TP1;
        private System.Windows.Forms.CheckBox MO_HideUnsupported;
        private System.Windows.Forms.CheckBox MO_ConfirmExit;
        private System.Windows.Forms.TabPage MO_TP2;
        private System.Windows.Forms.TextBox MO_TextEdBin;
        private System.Windows.Forms.Label L_MO_TextEdBin;
        private System.Windows.Forms.Button MO_FindTextEd;
        private System.Windows.Forms.OpenFileDialog MO_SearchBin;
        private System.Windows.Forms.CheckBox MO_HighlightOldBackUps;
        private System.Windows.Forms.TextBox MO_CustDirName;
        private System.Windows.Forms.Label L_MO_CustDirName;
        private System.Windows.Forms.CheckBox MO_ZipCompress;
        private System.Windows.Forms.CheckBox MO_UnSafeOps;
        private System.Windows.Forms.CheckBox MO_RemEmptyDirs;
        private System.Windows.Forms.CheckBox MO_UseUpstream;
        private System.Windows.Forms.CheckBox MO_AutoCheckUpdates;
        private System.Windows.Forms.CheckBox MO_HideOutdatedHUDs;
    }
}