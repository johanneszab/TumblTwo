using System.Threading;
using System.Windows.Forms;

namespace TumblTwo
{

    public partial class CrawlerForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
        /// 
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrawlerForm));
            this.tBlogUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblProcess = new System.Windows.Forms.Label();
            this.lbl23 = new System.Windows.Forms.Label();
            this.contextBlog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuShowFilesInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveBlog2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCrawl = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVisit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAddBlog = new System.Windows.Forms.ToolStripButton();
            this.toolRemoveBlog = new System.Windows.Forms.ToolStripButton();
            this.toolShowExplorer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolAddQueue = new System.Windows.Forms.ToolStripButton();
            this.toolRemoveQueue = new System.Windows.Forms.ToolStripButton();
            this.toolCrawl = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolPause = new System.Windows.Forms.ToolStripButton();
            this.toolResume = new System.Windows.Forms.ToolStripButton();
            this.toolStop = new System.Windows.Forms.ToolStripButton();
            this.toolCheckClipboard = new System.Windows.Forms.ToolStripButton();
            this.toolSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolAbout = new System.Windows.Forms.ToolStripButton();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.smallImage = new System.Windows.Forms.PictureBox();
            this.lvQueue = new System.Windows.Forms.ListView();
            this.chQueueName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chQueueStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvBlog = new TumblTwo.DataGridViewExtended();
            this.contextBlog.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smallImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvBlog)).BeginInit();
            this.SuspendLayout();
            // 
            // tBlogUrl
            // 
            this.tBlogUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tBlogUrl.BackColor = System.Drawing.SystemColors.Menu;
            this.tBlogUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tBlogUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBlogUrl.ForeColor = System.Drawing.Color.Black;
            this.tBlogUrl.Location = new System.Drawing.Point(135, 16);
            this.tBlogUrl.Name = "tBlogUrl";
            this.tBlogUrl.Size = new System.Drawing.Size(1052, 21);
            this.tBlogUrl.TabIndex = 1;
            this.tBlogUrl.Text = "http://";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label1.Location = new System.Drawing.Point(9, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enter a valid Tumblr Url:";
            // 
            // lblProcess
            // 
            this.lblProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProcess.AutoSize = true;
            this.lblProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcess.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblProcess.Location = new System.Drawing.Point(6, 49);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(79, 13);
            this.lblProcess.TabIndex = 6;
            this.lblProcess.Text = "                  ";
            // 
            // lbl23
            // 
            this.lbl23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl23.AutoSize = true;
            this.lbl23.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl23.Location = new System.Drawing.Point(6, 27);
            this.lbl23.Name = "lbl23";
            this.lbl23.Size = new System.Drawing.Size(85, 13);
            this.lbl23.TabIndex = 5;
            this.lbl23.Text = "Current Process:";
            // 
            // contextBlog
            // 
            this.contextBlog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowFilesInExplorer,
            this.mnuRemoveBlog2,
            this.toolStripMenuItem2,
            this.mnuCrawl,
            this.mnuVisit});
            this.contextBlog.Name = "contextBlog";
            this.contextBlog.Size = new System.Drawing.Size(254, 162);
            // 
            // mnuShowFilesInExplorer
            // 
            this.mnuShowFilesInExplorer.Image = global::TumblTwo.Properties.Resources.Explorer;
            this.mnuShowFilesInExplorer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuShowFilesInExplorer.Name = "mnuShowFilesInExplorer";
            this.mnuShowFilesInExplorer.Size = new System.Drawing.Size(253, 38);
            this.mnuShowFilesInExplorer.Text = "Show files in Windows Explorer";
            this.mnuShowFilesInExplorer.Click += new System.EventHandler(this.mnuShowFilesInExplorer_Click);
            // 
            // mnuRemoveBlog2
            // 
            this.mnuRemoveBlog2.Image = global::TumblTwo.Properties.Resources.RemoveBlog;
            this.mnuRemoveBlog2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuRemoveBlog2.Name = "mnuRemoveBlog2";
            this.mnuRemoveBlog2.Size = new System.Drawing.Size(253, 38);
            this.mnuRemoveBlog2.Text = "Remove selected blogs";
            this.mnuRemoveBlog2.Click += new System.EventHandler(this.RemoveBlog);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(250, 6);
            // 
            // mnuCrawl
            // 
            this.mnuCrawl.Image = global::TumblTwo.Properties.Resources.Scan;
            this.mnuCrawl.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuCrawl.Name = "mnuCrawl";
            this.mnuCrawl.Size = new System.Drawing.Size(253, 38);
            this.mnuCrawl.Text = "Start Crawling";
            this.mnuCrawl.Click += new System.EventHandler(this.mnuRescanBlog_Click);
            // 
            // mnuVisit
            // 
            this.mnuVisit.Name = "mnuVisit";
            this.mnuVisit.Size = new System.Drawing.Size(253, 38);
            this.mnuVisit.Text = "Visit blogs in Internet Browser";
            this.mnuVisit.Click += new System.EventHandler(this.mnuVisit_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAddBlog,
            this.toolRemoveBlog,
            this.toolShowExplorer,
            this.toolStripSeparator1,
            this.toolAddQueue,
            this.toolRemoveQueue,
            this.toolCrawl,
            this.toolStripSeparator2,
            this.toolPause,
            this.toolResume,
            this.toolStop,
            this.toolCheckClipboard,
            this.toolSettings,
            this.toolStripSeparator3,
            this.toolAbout});
            this.toolStrip1.Location = new System.Drawing.Point(1194, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(100, 671);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAddBlog
            // 
            this.toolAddBlog.BackColor = System.Drawing.SystemColors.Menu;
            this.toolAddBlog.Image = global::TumblTwo.Properties.Resources.AddBlog;
            this.toolAddBlog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddBlog.Name = "toolAddBlog";
            this.toolAddBlog.Size = new System.Drawing.Size(97, 51);
            this.toolAddBlog.Text = "Add Blog";
            this.toolAddBlog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolAddBlog.ToolTipText = "Add a new blog to the library";
            this.toolAddBlog.Click += new System.EventHandler(this.AddBlog);
            // 
            // toolRemoveBlog
            // 
            this.toolRemoveBlog.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolRemoveBlog.Image = global::TumblTwo.Properties.Resources.RemoveBlog;
            this.toolRemoveBlog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRemoveBlog.Name = "toolRemoveBlog";
            this.toolRemoveBlog.Size = new System.Drawing.Size(97, 51);
            this.toolRemoveBlog.Text = "Remove Blog";
            this.toolRemoveBlog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolRemoveBlog.ToolTipText = "Remove selected blogs with all dowloaded images";
            this.toolRemoveBlog.Click += new System.EventHandler(this.RemoveBlog);
            // 
            // toolShowExplorer
            // 
            this.toolShowExplorer.Enabled = false;
            this.toolShowExplorer.Image = global::TumblTwo.Properties.Resources.Explorer;
            this.toolShowExplorer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolShowExplorer.Name = "toolShowExplorer";
            this.toolShowExplorer.Size = new System.Drawing.Size(97, 51);
            this.toolShowExplorer.Text = "Show Files";
            this.toolShowExplorer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolShowExplorer.ToolTipText = "Show files of the selected blogs in the Windows Explorer";
            this.toolShowExplorer.Click += new System.EventHandler(this.mnuShowFilesInExplorer_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
            // 
            // toolAddQueue
            // 
            this.toolAddQueue.Image = global::TumblTwo.Properties.Resources.AddQueue;
            this.toolAddQueue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddQueue.Name = "toolAddQueue";
            this.toolAddQueue.Size = new System.Drawing.Size(97, 51);
            this.toolAddQueue.Text = "Add to Queue";
            this.toolAddQueue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolAddQueue.ToolTipText = "Queue selected blogs for crawling";
            this.toolAddQueue.Click += new System.EventHandler(this.toolAddQueue_Click);
            // 
            // toolRemoveQueue
            // 
            this.toolRemoveQueue.Image = global::TumblTwo.Properties.Resources.RemoveQueue;
            this.toolRemoveQueue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRemoveQueue.Name = "toolRemoveQueue";
            this.toolRemoveQueue.Size = new System.Drawing.Size(97, 51);
            this.toolRemoveQueue.Text = "Remove Queue";
            this.toolRemoveQueue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolRemoveQueue.ToolTipText = "Remove selected blogs from the queue";
            this.toolRemoveQueue.Click += new System.EventHandler(this.toolRemoveQueue_Click);
            // 
            // toolCrawl
            // 
            this.toolCrawl.Image = global::TumblTwo.Properties.Resources.Scan;
            this.toolCrawl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCrawl.Name = "toolCrawl";
            this.toolCrawl.Size = new System.Drawing.Size(97, 51);
            this.toolCrawl.Text = "Crawl";
            this.toolCrawl.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolCrawl.ToolTipText = "Crawl blogs in the queue for images";
            this.toolCrawl.Click += new System.EventHandler(this.mnuRescanBlog_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(97, 6);
            // 
            // toolPause
            // 
            this.toolPause.Enabled = false;
            this.toolPause.Image = global::TumblTwo.Properties.Resources.Pause;
            this.toolPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolPause.Name = "toolPause";
            this.toolPause.Size = new System.Drawing.Size(97, 51);
            this.toolPause.Text = "Pause";
            this.toolPause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolPause.ToolTipText = "Pause crawling";
            this.toolPause.Click += new System.EventHandler(this.toolPause_Click);
            // 
            // toolResume
            // 
            this.toolResume.Enabled = false;
            this.toolResume.Image = global::TumblTwo.Properties.Resources.Resume;
            this.toolResume.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolResume.Name = "toolResume";
            this.toolResume.Size = new System.Drawing.Size(97, 51);
            this.toolResume.Text = "Resume";
            this.toolResume.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolResume.ToolTipText = "Continue crawling";
            this.toolResume.Click += new System.EventHandler(this.toolResume_Click);
            // 
            // toolStop
            // 
            this.toolStop.Enabled = false;
            this.toolStop.Image = global::TumblTwo.Properties.Resources.Stop;
            this.toolStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStop.Name = "toolStop";
            this.toolStop.Size = new System.Drawing.Size(97, 51);
            this.toolStop.Text = "Stop";
            this.toolStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStop.ToolTipText = "Terminates the currently running crawl process";
            this.toolStop.Click += new System.EventHandler(this.toolStop_Click);
            // 
            // toolCheckClipboard
            // 
            this.toolCheckClipboard.Checked = true;
            this.toolCheckClipboard.CheckOnClick = true;
            this.toolCheckClipboard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolCheckClipboard.Image = global::TumblTwo.Properties.Resources.Clipboard;
            this.toolCheckClipboard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCheckClipboard.Name = "toolCheckClipboard";
            this.toolCheckClipboard.Size = new System.Drawing.Size(97, 51);
            this.toolCheckClipboard.Text = "Check Clipboard";
            this.toolCheckClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolCheckClipboard.ToolTipText = "Check the Content of the Clipboard for Tumblr Blogs";
            this.toolCheckClipboard.Click += new System.EventHandler(this.toolCheckClipboard_Click);
            // 
            // toolSettings
            // 
            this.toolSettings.Image = global::TumblTwo.Properties.Resources.Settings;
            this.toolSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSettings.Name = "toolSettings";
            this.toolSettings.Size = new System.Drawing.Size(97, 51);
            this.toolSettings.Text = "Settings";
            this.toolSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolSettings.ToolTipText = "Opens Settings";
            this.toolSettings.Click += new System.EventHandler(this.toolSettings_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(97, 6);
            // 
            // toolAbout
            // 
            this.toolAbout.Image = global::TumblTwo.Properties.Resources.About;
            this.toolAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAbout.Name = "toolAbout";
            this.toolAbout.Size = new System.Drawing.Size(44, 51);
            this.toolAbout.Text = "About";
            this.toolAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolAbout.ToolTipText = "Opens the about window";
            this.toolAbout.Click += new System.EventHandler(this.toolAbout_Click);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Filename";
            this.columnHeader5.Width = 154;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Downloaded";
            this.columnHeader8.Width = 82;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Url";
            this.columnHeader6.Width = 382;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbl23);
            this.groupBox1.Controls.Add(this.lblProcess);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.groupBox1.Location = new System.Drawing.Point(12, 475);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1012, 184);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download";
            // 
            // panelInfo
            // 
            this.panelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInfo.Controls.Add(this.label2);
            this.panelInfo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.panelInfo.Location = new System.Drawing.Point(12, 475);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(1175, 184);
            this.panelInfo.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.Window;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1175, 184);
            this.label2.TabIndex = 0;
            this.label2.Text = "To start, click on a blog, add it to the queue and hit the \'Crawl\' button to star" +
    "t the crawl process. ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // smallImage
            // 
            this.smallImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.smallImage.BackColor = System.Drawing.SystemColors.Menu;
            this.smallImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.smallImage.Location = new System.Drawing.Point(1030, 475);
            this.smallImage.Name = "smallImage";
            this.smallImage.Size = new System.Drawing.Size(157, 184);
            this.smallImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.smallImage.TabIndex = 7;
            this.smallImage.TabStop = false;
            this.smallImage.Click += new System.EventHandler(this.smallImage_Click);
            // 
            // lvQueue
            // 
            this.lvQueue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvQueue.BackColor = System.Drawing.SystemColors.Menu;
            this.lvQueue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chQueueName,
            this.chQueueStatus});
            this.lvQueue.ContextMenuStrip = this.contextBlog;
            this.lvQueue.FullRowSelect = true;
            this.lvQueue.HideSelection = false;
            this.lvQueue.Location = new System.Drawing.Point(12, 322);
            this.lvQueue.Name = "lvQueue";
            this.lvQueue.Size = new System.Drawing.Size(1176, 147);
            this.lvQueue.TabIndex = 12;
            this.lvQueue.UseCompatibleStateImageBehavior = false;
            this.lvQueue.View = System.Windows.Forms.View.Details;
            // 
            // chQueueName
            // 
            this.chQueueName.Text = "Name";
            this.chQueueName.Width = 200;
            // 
            // chQueueStatus
            // 
            this.chQueueStatus.Text = "Queue Status";
            this.chQueueStatus.Width = 900;
            // 
            // lvBlog
            // 
            this.lvBlog.AllowUserToAddRows = false;
            this.lvBlog.AllowUserToDeleteRows = false;
            this.lvBlog.AllowUserToOrderColumns = true;
            this.lvBlog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvBlog.BackgroundColor = System.Drawing.SystemColors.Menu;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lvBlog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.lvBlog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lvBlog.ContextMenuStrip = this.contextBlog;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Menu;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.lvBlog.DefaultCellStyle = dataGridViewCellStyle6;
            this.lvBlog.Location = new System.Drawing.Point(12, 43);
            this.lvBlog.Name = "lvBlog";
            this.lvBlog.ReadOnly = true;
            this.lvBlog.RightToLeft = System.Windows.Forms.RightToLeft.No;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.lvBlog.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.lvBlog.RowHeadersWidth = 4;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvBlog.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.lvBlog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lvBlog.Size = new System.Drawing.Size(1175, 273);
            this.lvBlog.TabIndex = 13;
            // 
            // CrawlerForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1294, 671);
            this.Controls.Add(this.lvBlog);
            this.Controls.Add(this.lvQueue);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.smallImage);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tBlogUrl);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CrawlerForm";
            this.Text = "TumblTwo - A Tumblr Image Crawler - Version 1.0.7";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.contextBlog.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.smallImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvBlog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader8;
        //private IContainer components;
        private ContextMenuStrip contextBlog;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        public Label lbl23;
        public Label lblProcess;
        private ToolStripMenuItem mnuCrawl;
        private ToolStripMenuItem mnuRemoveBlog2;
        private ToolStripMenuItem mnuShowFilesInExplorer;
        private ToolStripMenuItem mnuVisit;
        private Panel panelInfo;
        private TextBox tBlogUrl;
        private ToolStripButton toolAbout;
        private ToolStripButton toolAddBlog;
        private ToolStripButton toolCrawl;
        private ToolStripButton toolPause;
        private ToolStripButton toolRemoveBlog;
        private ToolStripButton toolResume;
        private ToolStripButton toolShowExplorer;
        private ToolStripButton toolStop;
        private ToolStrip toolStrip1;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolSettings;
        public PictureBox smallImage;
        private ToolStripButton toolAddQueue;
        private ToolStripButton toolRemoveQueue;
        private ListView lvQueue;
        private ColumnHeader chQueueName;
        private ColumnHeader chQueueStatus;
        private ToolStripButton toolCheckClipboard;
        private DataGridViewExtended lvBlog;
    }
}
