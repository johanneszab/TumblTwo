namespace TumblTwo
{
    partial class Settings
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.tvSettings = new System.Windows.Forms.TreeView();
            this.gbSettingsGeneral = new System.Windows.Forms.GroupBox();
            this.cbCheckMirror = new System.Windows.Forms.CheckBox();
            this.nudParallelImageDownloads = new System.Windows.Forms.NumericUpDown();
            this.lbParallelImageDownloads = new System.Windows.Forms.Label();
            this.cbParallelCrawl = new System.Windows.Forms.CheckBox();
            this.cbCheckStatus = new System.Windows.Forms.CheckBox();
            this.cbDeleteIndexOnly = new System.Windows.Forms.CheckBox();
            this.chkGif = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.cbImagesize = new System.Windows.Forms.ComboBox();
            this.nudSimultaneousDownloads = new System.Windows.Forms.NumericUpDown();
            this.lbImageSize = new System.Windows.Forms.Label();
            this.lbSimultaneousDownloads = new System.Windows.Forms.Label();
            this.cbRemoveFinished = new System.Windows.Forms.CheckBox();
            this.cbPicturePreview = new System.Windows.Forms.CheckBox();
            this.panelSettingsGeneral = new System.Windows.Forms.Panel();
            this.bChooseDownloadLocation = new System.Windows.Forms.Button();
            this.lbDownloadLocation = new System.Windows.Forms.Label();
            this.cbDownloadVideos = new System.Windows.Forms.CheckBox();
            this.cbDownloadImages = new System.Windows.Forms.CheckBox();
            this.lbVideoSize = new System.Windows.Forms.Label();
            this.cbVideosize = new System.Windows.Forms.ComboBox();
            this.tbDownloadLocation = new System.Windows.Forms.TextBox();
            this.gbSettingsGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudParallelImageDownloads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSimultaneousDownloads)).BeginInit();
            this.panelSettingsGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvSettings
            // 
            this.tvSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvSettings.BackColor = System.Drawing.SystemColors.Menu;
            this.tvSettings.Location = new System.Drawing.Point(12, 12);
            this.tvSettings.Name = "tvSettings";
            treeNode1.Name = "settingsGeneral";
            treeNode1.Text = "General";
            this.tvSettings.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvSettings.Size = new System.Drawing.Size(205, 478);
            this.tvSettings.TabIndex = 0;
            // 
            // gbSettingsGeneral
            // 
            this.gbSettingsGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSettingsGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbSettingsGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.gbSettingsGeneral.Controls.Add(this.cbDownloadImages);
            this.gbSettingsGeneral.Controls.Add(this.cbDownloadVideos);
            this.gbSettingsGeneral.Controls.Add(this.cbCheckMirror);
            this.gbSettingsGeneral.Controls.Add(this.nudParallelImageDownloads);
            this.gbSettingsGeneral.Controls.Add(this.lbParallelImageDownloads);
            this.gbSettingsGeneral.Controls.Add(this.cbParallelCrawl);
            this.gbSettingsGeneral.Controls.Add(this.cbCheckStatus);
            this.gbSettingsGeneral.Controls.Add(this.cbDeleteIndexOnly);
            this.gbSettingsGeneral.Controls.Add(this.chkGif);
            this.gbSettingsGeneral.Controls.Add(this.buttonCancel);
            this.gbSettingsGeneral.Controls.Add(this.buttonOk);
            this.gbSettingsGeneral.Controls.Add(this.cbVideosize);
            this.gbSettingsGeneral.Controls.Add(this.cbImagesize);
            this.gbSettingsGeneral.Controls.Add(this.nudSimultaneousDownloads);
            this.gbSettingsGeneral.Controls.Add(this.lbVideoSize);
            this.gbSettingsGeneral.Controls.Add(this.lbImageSize);
            this.gbSettingsGeneral.Controls.Add(this.lbSimultaneousDownloads);
            this.gbSettingsGeneral.Controls.Add(this.cbRemoveFinished);
            this.gbSettingsGeneral.Controls.Add(this.cbPicturePreview);
            this.gbSettingsGeneral.Controls.Add(this.panelSettingsGeneral);
            this.gbSettingsGeneral.Location = new System.Drawing.Point(223, 12);
            this.gbSettingsGeneral.Name = "gbSettingsGeneral";
            this.gbSettingsGeneral.Size = new System.Drawing.Size(499, 478);
            this.gbSettingsGeneral.TabIndex = 1;
            this.gbSettingsGeneral.TabStop = false;
            this.gbSettingsGeneral.Text = "General";
            // 
            // cbCheckMirror
            // 
            this.cbCheckMirror.AutoSize = true;
            this.cbCheckMirror.Location = new System.Drawing.Point(12, 276);
            this.cbCheckMirror.Name = "cbCheckMirror";
            this.cbCheckMirror.Size = new System.Drawing.Size(448, 17);
            this.cbCheckMirror.TabIndex = 16;
            this.cbCheckMirror.Text = "Check if new images were previously downloaded from a different mirror (high CPU " +
    "usage)";
            this.cbCheckMirror.UseVisualStyleBackColor = true;
            this.cbCheckMirror.CheckedChanged += new System.EventHandler(this.cbCheckMirror_CheckedChanged);
            // 
            // nudParallelImageDownloads
            // 
            this.nudParallelImageDownloads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudParallelImageDownloads.BackColor = System.Drawing.SystemColors.Menu;
            this.nudParallelImageDownloads.Location = new System.Drawing.Point(305, 339);
            this.nudParallelImageDownloads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudParallelImageDownloads.Name = "nudParallelImageDownloads";
            this.nudParallelImageDownloads.Size = new System.Drawing.Size(169, 20);
            this.nudParallelImageDownloads.TabIndex = 15;
            this.nudParallelImageDownloads.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // lbParallelImageDownloads
            // 
            this.lbParallelImageDownloads.AutoSize = true;
            this.lbParallelImageDownloads.Location = new System.Drawing.Point(9, 341);
            this.lbParallelImageDownloads.Name = "lbParallelImageDownloads";
            this.lbParallelImageDownloads.Size = new System.Drawing.Size(278, 13);
            this.lbParallelImageDownloads.TabIndex = 14;
            this.lbParallelImageDownloads.Text = "Maximum number of parallel image downloads for all blogs";
            // 
            // cbParallelCrawl
            // 
            this.cbParallelCrawl.AutoSize = true;
            this.cbParallelCrawl.Checked = true;
            this.cbParallelCrawl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbParallelCrawl.Location = new System.Drawing.Point(12, 252);
            this.cbParallelCrawl.Name = "cbParallelCrawl";
            this.cbParallelCrawl.Size = new System.Drawing.Size(127, 17);
            this.cbParallelCrawl.TabIndex = 13;
            this.cbParallelCrawl.Text = "Crawl blogs in parallel";
            this.cbParallelCrawl.UseVisualStyleBackColor = true;
            this.cbParallelCrawl.CheckedChanged += new System.EventHandler(this.cbParallelCrawl_CheckedChanged);
            // 
            // cbCheckStatus
            // 
            this.cbCheckStatus.AutoSize = true;
            this.cbCheckStatus.Checked = true;
            this.cbCheckStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckStatus.Location = new System.Drawing.Point(12, 228);
            this.cbCheckStatus.Name = "cbCheckStatus";
            this.cbCheckStatus.Size = new System.Drawing.Size(189, 17);
            this.cbCheckStatus.TabIndex = 12;
            this.cbCheckStatus.Text = "Check blog online status at startup";
            this.cbCheckStatus.UseVisualStyleBackColor = true;
            this.cbCheckStatus.CheckedChanged += new System.EventHandler(this.cbCheckStatus_CheckedChanged);
            // 
            // cbDeleteIndexOnly
            // 
            this.cbDeleteIndexOnly.AutoSize = true;
            this.cbDeleteIndexOnly.Checked = true;
            this.cbDeleteIndexOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDeleteIndexOnly.Location = new System.Drawing.Point(12, 205);
            this.cbDeleteIndexOnly.Name = "cbDeleteIndexOnly";
            this.cbDeleteIndexOnly.Size = new System.Drawing.Size(264, 17);
            this.cbDeleteIndexOnly.TabIndex = 11;
            this.cbDeleteIndexOnly.Text = "Delete only the index files (no downloaded images)";
            this.cbDeleteIndexOnly.UseVisualStyleBackColor = true;
            this.cbDeleteIndexOnly.CheckedChanged += new System.EventHandler(this.cbDeleteIndexOnly_CheckedChanged);
            // 
            // chkGif
            // 
            this.chkGif.AutoSize = true;
            this.chkGif.Location = new System.Drawing.Point(12, 181);
            this.chkGif.Name = "chkGif";
            this.chkGif.Size = new System.Drawing.Size(85, 17);
            this.chkGif.TabIndex = 10;
            this.chkGif.Text = "Skip .gif files";
            this.chkGif.UseVisualStyleBackColor = true;
            this.chkGif.CheckedChanged += new System.EventHandler(this.chkGif_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(334, 449);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(414, 449);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "Save";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // cbImagesize
            // 
            this.cbImagesize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbImagesize.BackColor = System.Drawing.SystemColors.Menu;
            this.cbImagesize.FormattingEnabled = true;
            this.cbImagesize.Items.AddRange(new object[] {
            "1280",
            "500",
            "400",
            "250",
            "100",
            "75"});
            this.cbImagesize.Location = new System.Drawing.Point(305, 369);
            this.cbImagesize.Name = "cbImagesize";
            this.cbImagesize.Size = new System.Drawing.Size(170, 21);
            this.cbImagesize.TabIndex = 7;
            // 
            // nudSimultaneousDownloads
            // 
            this.nudSimultaneousDownloads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSimultaneousDownloads.BackColor = System.Drawing.SystemColors.Menu;
            this.nudSimultaneousDownloads.Location = new System.Drawing.Point(305, 308);
            this.nudSimultaneousDownloads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSimultaneousDownloads.Name = "nudSimultaneousDownloads";
            this.nudSimultaneousDownloads.Size = new System.Drawing.Size(169, 20);
            this.nudSimultaneousDownloads.TabIndex = 6;
            this.nudSimultaneousDownloads.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lbImageSize
            // 
            this.lbImageSize.AutoSize = true;
            this.lbImageSize.Location = new System.Drawing.Point(9, 372);
            this.lbImageSize.Name = "lbImageSize";
            this.lbImageSize.Size = new System.Drawing.Size(113, 13);
            this.lbImageSize.TabIndex = 5;
            this.lbImageSize.Text = "Imagesize (max. width)";
            // 
            // lbSimultaneousDownloads
            // 
            this.lbSimultaneousDownloads.AutoSize = true;
            this.lbSimultaneousDownloads.Location = new System.Drawing.Point(9, 310);
            this.lbSimultaneousDownloads.Name = "lbSimultaneousDownloads";
            this.lbSimultaneousDownloads.Size = new System.Drawing.Size(193, 13);
            this.lbSimultaneousDownloads.TabIndex = 4;
            this.lbSimultaneousDownloads.Text = "Maximum number of parallel blog crawls";
            // 
            // cbRemoveFinished
            // 
            this.cbRemoveFinished.AutoSize = true;
            this.cbRemoveFinished.Location = new System.Drawing.Point(12, 157);
            this.cbRemoveFinished.Name = "cbRemoveFinished";
            this.cbRemoveFinished.Size = new System.Drawing.Size(169, 17);
            this.cbRemoveFinished.TabIndex = 3;
            this.cbRemoveFinished.Text = "Remove blog index after crawl";
            this.cbRemoveFinished.UseVisualStyleBackColor = true;
            this.cbRemoveFinished.CheckedChanged += new System.EventHandler(this.cbRemoveFinished_CheckedChanged);
            // 
            // cbPicturePreview
            // 
            this.cbPicturePreview.AutoSize = true;
            this.cbPicturePreview.Checked = true;
            this.cbPicturePreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPicturePreview.Location = new System.Drawing.Point(12, 133);
            this.cbPicturePreview.Name = "cbPicturePreview";
            this.cbPicturePreview.Size = new System.Drawing.Size(146, 17);
            this.cbPicturePreview.TabIndex = 2;
            this.cbPicturePreview.Text = "Show the picture preview";
            this.cbPicturePreview.UseVisualStyleBackColor = true;
            this.cbPicturePreview.CheckedChanged += new System.EventHandler(this.cbPicturePreview_CheckedChanged);
            // 
            // panelSettingsGeneral
            // 
            this.panelSettingsGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSettingsGeneral.Controls.Add(this.bChooseDownloadLocation);
            this.panelSettingsGeneral.Controls.Add(this.tbDownloadLocation);
            this.panelSettingsGeneral.Controls.Add(this.lbDownloadLocation);
            this.panelSettingsGeneral.Location = new System.Drawing.Point(6, 19);
            this.panelSettingsGeneral.Name = "panelSettingsGeneral";
            this.panelSettingsGeneral.Size = new System.Drawing.Size(483, 60);
            this.panelSettingsGeneral.TabIndex = 1;
            // 
            // bChooseDownloadLocation
            // 
            this.bChooseDownloadLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bChooseDownloadLocation.Location = new System.Drawing.Point(393, 8);
            this.bChooseDownloadLocation.Name = "bChooseDownloadLocation";
            this.bChooseDownloadLocation.Size = new System.Drawing.Size(75, 23);
            this.bChooseDownloadLocation.TabIndex = 2;
            this.bChooseDownloadLocation.Text = "Browse";
            this.bChooseDownloadLocation.UseVisualStyleBackColor = true;
            this.bChooseDownloadLocation.Click += new System.EventHandler(this.bChooseDownloadLocation_Click);
            // 
            // lbDownloadLocation
            // 
            this.lbDownloadLocation.AutoSize = true;
            this.lbDownloadLocation.Location = new System.Drawing.Point(3, 13);
            this.lbDownloadLocation.Name = "lbDownloadLocation";
            this.lbDownloadLocation.Size = new System.Drawing.Size(102, 13);
            this.lbDownloadLocation.TabIndex = 0;
            this.lbDownloadLocation.Text = "Download Location:";
            // 
            // cbDownloadVideos
            // 
            this.cbDownloadVideos.AutoSize = true;
            this.cbDownloadVideos.Location = new System.Drawing.Point(12, 110);
            this.cbDownloadVideos.Name = "cbDownloadVideos";
            this.cbDownloadVideos.Size = new System.Drawing.Size(203, 17);
            this.cbDownloadVideos.TabIndex = 17;
            this.cbDownloadVideos.Text = "Download videos (tumblr.com hosted)";
            this.cbDownloadVideos.UseVisualStyleBackColor = true;
            this.cbDownloadVideos.CheckedChanged += new System.EventHandler(this.cbDownloadVideos_CheckedChanged);
            // 
            // cbDownloadImages
            // 
            this.cbDownloadImages.AutoSize = true;
            this.cbDownloadImages.Checked = true;
            this.cbDownloadImages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDownloadImages.Location = new System.Drawing.Point(12, 87);
            this.cbDownloadImages.Name = "cbDownloadImages";
            this.cbDownloadImages.Size = new System.Drawing.Size(110, 17);
            this.cbDownloadImages.TabIndex = 18;
            this.cbDownloadImages.Text = "Download images";
            this.cbDownloadImages.UseVisualStyleBackColor = true;
            this.cbDownloadImages.CheckedChanged += new System.EventHandler(this.cbDownloadImages_CheckedChanged);
            // 
            // lbVideoSize
            // 
            this.lbVideoSize.AutoSize = true;
            this.lbVideoSize.Location = new System.Drawing.Point(9, 399);
            this.lbVideoSize.Name = "lbVideoSize";
            this.lbVideoSize.Size = new System.Drawing.Size(111, 13);
            this.lbVideoSize.TabIndex = 5;
            this.lbVideoSize.Text = "Videosize (max. width)";
            // 
            // cbVideosize
            // 
            this.cbVideosize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbVideosize.BackColor = System.Drawing.SystemColors.Menu;
            this.cbVideosize.FormattingEnabled = true;
            this.cbVideosize.Items.AddRange(new object[] {
            "1080",
            "480"});
            this.cbVideosize.Location = new System.Drawing.Point(305, 396);
            this.cbVideosize.Name = "cbVideosize";
            this.cbVideosize.Size = new System.Drawing.Size(170, 21);
            this.cbVideosize.TabIndex = 7;
            // 
            // tbDownloadLocation
            // 
            this.tbDownloadLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDownloadLocation.BackColor = System.Drawing.SystemColors.Menu;
            this.tbDownloadLocation.Location = new System.Drawing.Point(111, 9);
            this.tbDownloadLocation.Name = "tbDownloadLocation";
            this.tbDownloadLocation.Size = new System.Drawing.Size(276, 20);
            this.tbDownloadLocation.TabIndex = 1;
            this.tbDownloadLocation.Text = global::TumblTwo.Properties.Settings.Default.configDownloadLocation;
            this.tbDownloadLocation.TextChanged += new System.EventHandler(this.tbDownloadLocation_TextChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(734, 501);
            this.Controls.Add(this.gbSettingsGeneral);
            this.Controls.Add(this.tvSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.gbSettingsGeneral.ResumeLayout(false);
            this.gbSettingsGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudParallelImageDownloads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSimultaneousDownloads)).EndInit();
            this.panelSettingsGeneral.ResumeLayout(false);
            this.panelSettingsGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvSettings;
        private System.Windows.Forms.GroupBox gbSettingsGeneral;
        private System.Windows.Forms.Panel panelSettingsGeneral;
        private System.Windows.Forms.Label lbDownloadLocation;
        private System.Windows.Forms.Button bChooseDownloadLocation;
        private System.Windows.Forms.TextBox tbDownloadLocation;
        private System.Windows.Forms.CheckBox cbRemoveFinished;
        private System.Windows.Forms.CheckBox cbPicturePreview;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox cbImagesize;
        private System.Windows.Forms.NumericUpDown nudSimultaneousDownloads;
        private System.Windows.Forms.Label lbImageSize;
        private System.Windows.Forms.Label lbSimultaneousDownloads;
        private System.Windows.Forms.CheckBox cbDeleteIndexOnly;
        private System.Windows.Forms.CheckBox chkGif;
        private System.Windows.Forms.CheckBox cbCheckStatus;
        private System.Windows.Forms.CheckBox cbParallelCrawl;
        private System.Windows.Forms.NumericUpDown nudParallelImageDownloads;
        private System.Windows.Forms.Label lbParallelImageDownloads;
        private System.Windows.Forms.CheckBox cbCheckMirror;
        private System.Windows.Forms.CheckBox cbDownloadImages;
        private System.Windows.Forms.CheckBox cbDownloadVideos;
        private System.Windows.Forms.ComboBox cbVideosize;
        private System.Windows.Forms.Label lbVideoSize;
    }
}