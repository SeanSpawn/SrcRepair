﻿namespace srcrepair.gui
{
    partial class FrmArchWrk
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmArchWrk));
            this.AR_WlcMsg = new System.Windows.Forms.Label();
            this.AR_PrgBr = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // AR_WlcMsg
            // 
            resources.ApplyResources(this.AR_WlcMsg, "AR_WlcMsg");
            this.AR_WlcMsg.Name = "AR_WlcMsg";
            // 
            // AR_PrgBr
            // 
            resources.ApplyResources(this.AR_PrgBr, "AR_PrgBr");
            this.AR_PrgBr.Name = "AR_PrgBr";
            // 
            // FrmArchWrk
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.AR_PrgBr);
            this.Controls.Add(this.AR_WlcMsg);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmArchWrk";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmArchWrk_FormClosing);
            this.Load += new System.EventHandler(this.FrmArchWrk_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label AR_WlcMsg;
        private System.Windows.Forms.ProgressBar AR_PrgBr;
    }
}