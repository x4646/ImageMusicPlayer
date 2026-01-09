namespace ImageMusicPlayer
{
    partial class MusicManagerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TreeView treeViewLibrary;
        private System.Windows.Forms.ListView listViewPlaylist;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnRemoveFolder;
        private System.Windows.Forms.Button btnClearPlaylist;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.ComboBox comboPlayMode;
        private System.Windows.Forms.Label labelLibrary;
        private System.Windows.Forms.Label labelPlaylist;

        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelVolume;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            treeViewLibrary = new TreeView();
            listViewPlaylist = new ListView();
            btnAddFolder = new Button();
            btnRemoveFolder = new Button();
            btnClearPlaylist = new Button();
            btnPlay = new Button();
            btnPause = new Button();
            btnNext = new Button();
            btnPrevious = new Button();
            comboPlayMode = new ComboBox();
            labelLibrary = new Label();
            labelPlaylist = new Label();
            trackBarVolume = new TrackBar();
            labelVolume = new Label();
            btnRemoveMuisc = new Button();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).BeginInit();
            SuspendLayout();
            // 
            // treeViewLibrary
            // 
            treeViewLibrary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            treeViewLibrary.CheckBoxes = true;
            treeViewLibrary.Location = new Point(12, 54);
            treeViewLibrary.Name = "treeViewLibrary";
            treeViewLibrary.Size = new Size(862, 485);
            treeViewLibrary.TabIndex = 0;
            treeViewLibrary.AfterCheck += treeViewLibrary_AfterCheck;
            treeViewLibrary.NodeMouseClick += TreeViewLibrary_NodeMouseClick;
            // 
            // listViewPlaylist
            // 
            listViewPlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            listViewPlaylist.Location = new Point(917, 54);
            listViewPlaylist.Name = "listViewPlaylist";
            listViewPlaylist.Size = new Size(829, 483);
            listViewPlaylist.TabIndex = 1;
            listViewPlaylist.UseCompatibleStateImageBehavior = false;
            listViewPlaylist.View = View.List;
            listViewPlaylist.DoubleClick += ListViewPlaylist_DoubleClick;
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new Point(156, 14);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(120, 30);
            btnAddFolder.TabIndex = 2;
            btnAddFolder.Text = "添加文件夹";
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // btnRemoveFolder
            // 
            btnRemoveFolder.Location = new Point(336, 14);
            btnRemoveFolder.Name = "btnRemoveFolder";
            btnRemoveFolder.Size = new Size(140, 30);
            btnRemoveFolder.TabIndex = 3;
            btnRemoveFolder.Text = "移除选中文件夹";
            btnRemoveFolder.Click += btnRemoveFolder_Click;
            // 
            // btnClearPlaylist
            // 
            btnClearPlaylist.Location = new Point(1119, 16);
            btnClearPlaylist.Name = "btnClearPlaylist";
            btnClearPlaylist.Size = new Size(140, 30);
            btnClearPlaylist.TabIndex = 4;
            btnClearPlaylist.Text = "清空播放列表";
            btnClearPlaylist.Click += BtnClearPlaylist_Click;
            // 
            // btnPlay
            // 
            btnPlay.Location = new Point(18, 558);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(80, 30);
            btnPlay.TabIndex = 5;
            btnPlay.Text = "播放";
            btnPlay.Click += btnPlay_Click;
            // 
            // btnPause
            // 
            btnPause.Location = new Point(106, 558);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(80, 30);
            btnPause.TabIndex = 6;
            btnPause.Text = "暂停";
            btnPause.Click += BtnPause_Click;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(196, 558);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(80, 30);
            btnNext.TabIndex = 7;
            btnNext.Text = "下一首";
            btnNext.Click += BtnNext_Click;
            // 
            // btnPrevious
            // 
            btnPrevious.Location = new Point(286, 558);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(80, 30);
            btnPrevious.TabIndex = 8;
            btnPrevious.Text = "上一首";
            btnPrevious.Click += BtnPrevious_Click;
            // 
            // comboPlayMode
            // 
            comboPlayMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboPlayMode.Items.AddRange(new object[] { "正序", "循环", "随机" });
            comboPlayMode.Location = new Point(1064, 543);
            comboPlayMode.Name = "comboPlayMode";
            comboPlayMode.Size = new Size(121, 32);
            comboPlayMode.TabIndex = 9;
            comboPlayMode.SelectedIndexChanged += ComboPlayMode_SelectedIndexChanged;
            // 
            // labelLibrary
            // 
            labelLibrary.Location = new Point(18, 17);
            labelLibrary.Name = "labelLibrary";
            labelLibrary.Size = new Size(99, 27);
            labelLibrary.TabIndex = 10;
            labelLibrary.Text = "🎵 音乐池";
            // 
            // labelPlaylist
            // 
            labelPlaylist.Location = new Point(917, 19);
            labelPlaylist.Name = "labelPlaylist";
            labelPlaylist.Size = new Size(100, 25);
            labelPlaylist.TabIndex = 11;
            labelPlaylist.Text = "▶️ 播放列表";
            // 
            // trackBarVolume
            // 
            trackBarVolume.Location = new Point(1204, 543);
            trackBarVolume.Maximum = 100;
            trackBarVolume.Name = "trackBarVolume";
            trackBarVolume.Size = new Size(337, 69);
            trackBarVolume.TabIndex = 0;
            trackBarVolume.TickFrequency = 10;
            trackBarVolume.Value = 80;
            trackBarVolume.Scroll += trackBarVolume_Scroll;
            // 
            // labelVolume
            // 
            labelVolume.Location = new Point(1578, 568);
            labelVolume.Name = "labelVolume";
            labelVolume.Size = new Size(151, 20);
            labelVolume.TabIndex = 1;
            labelVolume.Text = "音量：80%";
            // 
            // btnRemoveMuisc
            // 
            btnRemoveMuisc.Location = new Point(1325, 17);
            btnRemoveMuisc.Name = "btnRemoveMuisc";
            btnRemoveMuisc.Size = new Size(140, 30);
            btnRemoveMuisc.TabIndex = 12;
            btnRemoveMuisc.Text = "移除选中文件夹";
            btnRemoveMuisc.Click += btnRemoveMuisc_Click;
            // 
            // MusicManagerForm
            // 
            ClientSize = new Size(1758, 624);
            Controls.Add(btnRemoveMuisc);
            Controls.Add(trackBarVolume);
            Controls.Add(labelVolume);
            Controls.Add(treeViewLibrary);
            Controls.Add(listViewPlaylist);
            Controls.Add(btnAddFolder);
            Controls.Add(btnRemoveFolder);
            Controls.Add(btnClearPlaylist);
            Controls.Add(btnPlay);
            Controls.Add(btnPause);
            Controls.Add(btnNext);
            Controls.Add(btnPrevious);
            Controls.Add(comboPlayMode);
            Controls.Add(labelLibrary);
            Controls.Add(labelPlaylist);
            Name = "MusicManagerForm";
            Text = "音乐管理器";
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private Button btnRemoveMuisc;
    }
}
