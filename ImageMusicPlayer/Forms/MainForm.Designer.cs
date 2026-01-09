namespace ImageMusicPlayer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // 主菜单
        private System.Windows.Forms.MenuStrip menuStrip;
        // 图片管理菜单
        private System.Windows.Forms.ToolStripMenuItem menuPicture;
        private System.Windows.Forms.ToolStripMenuItem menuAddImages;
        private System.Windows.Forms.ToolStripMenuItem menuAppendImages;
        private System.Windows.Forms.ToolStripMenuItem menuNextImage;
        // 音乐管理菜单
        private System.Windows.Forms.ToolStripMenuItem menuMusic;
        private System.Windows.Forms.ToolStripMenuItem menuAddMusic;
        private System.Windows.Forms.ToolStripMenuItem menuNextMusic;
        // 新增：音乐管理窗口菜单项（原 menuMusicManagement_Click 入口）
        private System.Windows.Forms.ToolStripMenuItem menuMusicMgmt;
        // 收藏管理菜单
        private System.Windows.Forms.ToolStripMenuItem menuFavorite;
        private System.Windows.Forms.ToolStripMenuItem menuAddFavorite;
        private System.Windows.Forms.ToolStripMenuItem menuShowFavorites;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveFolderImages;
        // 幻灯片菜单
        private System.Windows.Forms.ToolStripMenuItem menuSlide;
        private System.Windows.Forms.ToolStripMenuItem menuStartSlide;
        private System.Windows.Forms.ToolStripMenuItem menuPauseSlide;
        // 全屏菜单（如果有全屏切换）
        private System.Windows.Forms.ToolStripMenuItem menuToggleFullscreen;
        // 滤镜菜单
        private System.Windows.Forms.ToolStripMenuItem menuFilter;
        private System.Windows.Forms.ToolStripMenuItem menuFilterNone;
        private System.Windows.Forms.ToolStripMenuItem menuFilterColorBoost;
        private System.Windows.Forms.ToolStripMenuItem menuFilterClarity;
        private System.Windows.Forms.ToolStripMenuItem menuFilterVintage;
        private System.Windows.Forms.ToolStripMenuItem menuFilterVivid;
        private System.Windows.Forms.ToolStripMenuItem menuFilterMoody;
        private System.Windows.Forms.ToolStripMenuItem menuFilterWarm;
        private System.Windows.Forms.ToolStripMenuItem menuFilterCool;
        private System.Windows.Forms.ToolStripMenuItem menuFilterSoft;
        private System.Windows.Forms.ToolStripMenuItem menuFilterDramatic;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorFilter;
        private System.Windows.Forms.TrackBar trackBarFilterIntensity;
        private System.Windows.Forms.ToolStripControlHost toolStripHostFilterIntensity;
        // 状态栏及核心显示控件
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private ImageViewer imageViewer;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 – 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            // ---------------------------
            // 图片管理菜单项初始化
            // ---------------------------
            this.menuPicture = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddImages = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAppendImages = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNextImage = new System.Windows.Forms.ToolStripMenuItem();
            // ---------------------------
            // 音乐管理菜单项初始化
            // ---------------------------
            this.menuMusic = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddMusic = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNextMusic = new System.Windows.Forms.ToolStripMenuItem();
            // 新增：音乐管理窗口菜单项
            this.menuMusicMgmt = new System.Windows.Forms.ToolStripMenuItem();
            // ---------------------------
            // 收藏管理菜单项初始化
            // ---------------------------
            this.menuFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowFavorites = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveFolderImages = new System.Windows.Forms.ToolStripMenuItem();
            // ---------------------------
            // 幻灯片菜单项初始化
            // ---------------------------
            this.menuSlide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartSlide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPauseSlide = new System.Windows.Forms.ToolStripMenuItem();
            // ---------------------------
            // 全屏菜单项初始化
            // ---------------------------
            this.menuToggleFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            // ---------------------------
            // 滤镜菜单项初始化
            // ---------------------------
            this.menuFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterNone = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterColorBoost = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterClarity = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterVintage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterVivid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterMoody = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterWarm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterCool = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterSoft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterDramatic = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorFilter = new System.Windows.Forms.ToolStripSeparator();
            this.trackBarFilterIntensity = new System.Windows.Forms.TrackBar();
            this.toolStripHostFilterIntensity = new System.Windows.Forms.ToolStripControlHost(this.trackBarFilterIntensity);
            // ---------------------------
            // 状态栏及核心显示控件初始化
            // ---------------------------
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageViewer = new ImageMusicPlayer.ImageViewer();
            // 
            // menuStrip 设置
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuPicture,
                this.menuMusic,
                this.menuFavorite,
                this.menuSlide,
                this.menuToggleFullscreen,
                this.menuFilter});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 33);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuPicture 设置
            // 
            this.menuPicture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuAddImages,
                this.menuAppendImages,
                this.menuNextImage});
            this.menuPicture.Name = "menuPicture";
            this.menuPicture.Size = new System.Drawing.Size(98, 29);
            this.menuPicture.Text = "图片管理";
            // 
            // menuAddImages 设置
            // 
            this.menuAddImages.Name = "menuAddImages";
            this.menuAddImages.Size = new System.Drawing.Size(224, 34);
            this.menuAddImages.Text = "添加图片文件夹";
            this.menuAddImages.Click += new System.EventHandler(this.MenuAddImages_Click);
            // 
            // menuAppendImages 设置
            // 
            this.menuAppendImages.Name = "menuAppendImages";
            this.menuAppendImages.Size = new System.Drawing.Size(224, 34);
            this.menuAppendImages.Text = "追加图片";
            this.menuAppendImages.Click += new System.EventHandler(this.MenuAppendImages_Click);
            // 
            // menuNextImage 设置
            // 
            this.menuNextImage.Name = "menuNextImage";
            this.menuNextImage.Size = new System.Drawing.Size(224, 34);
            this.menuNextImage.Text = "下一张";
            this.menuNextImage.Click += new System.EventHandler(this.MenuNextImage_Click);
            // ---------------------------
            // menuMusic 设置
            // ---------------------------
            this.menuMusic.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuAddMusic,
                this.menuNextMusic,
                this.menuMusicMgmt});
            this.menuMusic.Name = "menuMusic";
            this.menuMusic.Size = new System.Drawing.Size(98, 29);
            this.menuMusic.Text = "音乐管理";
            // 
            // menuAddMusic 设置
            // 
            this.menuAddMusic.Name = "menuAddMusic";
            this.menuAddMusic.Size = new System.Drawing.Size(224, 34);
            this.menuAddMusic.Text = "添加音乐";
            this.menuAddMusic.Click += new System.EventHandler(this.MenuAddMusic_Click);
            // 
            // menuNextMusic 设置
            // 
            this.menuNextMusic.Name = "menuNextMusic";
            this.menuNextMusic.Size = new System.Drawing.Size(224, 34);
            this.menuNextMusic.Text = "下一首";
            this.menuNextMusic.Click += new System.EventHandler(this.MenuNextMusic_Click);
            // 
            // menuMusicMgmt 设置（音乐管理窗口）
            // 
            this.menuMusicMgmt.Name = "menuMusicMgmt";
            this.menuMusicMgmt.Size = new System.Drawing.Size(224, 34);
            this.menuMusicMgmt.Text = "音乐管理窗口";
            this.menuMusicMgmt.Click += new System.EventHandler(this.MenuMusicManagement_Click);

            // ---------------------------
            // menuFavorite 设置
            // ---------------------------
            this.menuFavorite.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuAddFavorite,
                this.menuShowFavorites,
                this.menuRemoveFolderImages});
            this.menuFavorite.Name = "menuFavorite";
            this.menuFavorite.Size = new System.Drawing.Size(98, 29);
            this.menuFavorite.Text = "收藏管理";
            // 
            // menuAddFavorite 设置
            // 
            this.menuAddFavorite.Name = "menuAddFavorite";
            this.menuAddFavorite.Size = new System.Drawing.Size(224, 34);
            this.menuAddFavorite.Text = "收藏当前图";
            this.menuAddFavorite.Click += new System.EventHandler(this.MenuAddFavorite_Click);
            // 
            // menuShowFavorites 设置
            // 
            this.menuShowFavorites.Name = "menuShowFavorites";
            this.menuShowFavorites.Size = new System.Drawing.Size(224, 34);
            this.menuShowFavorites.Text = "收藏管理";
            this.menuShowFavorites.Click += new System.EventHandler(this.MenuShowFavorites_Click);
            // 
            // menuRemoveFolderImages 设置
            // 
            this.menuRemoveFolderImages.Name = "menuRemoveFolderImages";
            this.menuRemoveFolderImages.Size = new System.Drawing.Size(224, 34);
            this.menuRemoveFolderImages.Text = "删除文件夹图片";
            this.menuRemoveFolderImages.Click += new System.EventHandler(this.MenuRemoveFolderImages_Click);
            // ---------------------------
            // menuSlide 设置
            // ---------------------------
            this.menuSlide.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuStartSlide,
                this.menuPauseSlide});
            this.menuSlide.Name = "menuSlide";
            this.menuSlide.Size = new System.Drawing.Size(74, 29);
            this.menuSlide.Text = "幻灯片";
            // 
            // menuStartSlide 设置
            // 
            this.menuStartSlide.Name = "menuStartSlide";
            this.menuStartSlide.Size = new System.Drawing.Size(224, 34);
            this.menuStartSlide.Text = "开始幻灯片";
            this.menuStartSlide.Click += new System.EventHandler(this.MenuStartSlide_Click);
            // 
            // menuPauseSlide 设置
            // 
            this.menuPauseSlide.Name = "menuPauseSlide";
            this.menuPauseSlide.Size = new System.Drawing.Size(224, 34);
            this.menuPauseSlide.Text = "暂停幻灯片";
            this.menuPauseSlide.Click += new System.EventHandler(this.MenuPauseSlide_Click);
            // ---------------------------
            // menuToggleFullscreen 设置
            // ---------------------------
            this.menuToggleFullscreen.Name = "menuToggleFullscreen";
            this.menuToggleFullscreen.Size = new System.Drawing.Size(114, 29);
            this.menuToggleFullscreen.Text = "切换全屏";
            this.menuToggleFullscreen.Click += new System.EventHandler(this.MenuToggleFullscreen_Click);
            // ---------------------------
            // menuFilter 设置
            // ---------------------------
            this.menuFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFilterNone,
                this.menuFilterColorBoost,
                this.menuFilterClarity,
                this.menuFilterVintage,
                this.menuFilterVivid,
                this.menuFilterMoody,
                this.menuFilterWarm,
                this.menuFilterCool,
                this.menuFilterSoft,
                this.menuFilterDramatic,
                this.toolStripSeparatorFilter,
                this.toolStripHostFilterIntensity});
            this.menuFilter.Name = "menuFilter";
            this.menuFilter.Size = new System.Drawing.Size(74, 29);
            this.menuFilter.Text = "滤镜";
            // 
            // menuFilterNone 设置
            // 
            this.menuFilterNone.Name = "menuFilterNone";
            this.menuFilterNone.Size = new System.Drawing.Size(224, 34);
            this.menuFilterNone.Text = "无滤镜";
            this.menuFilterNone.Click += new System.EventHandler(this.MenuFilterNone_Click);
            // 
            // menuFilterColorBoost 设置
            // 
            this.menuFilterColorBoost.Name = "menuFilterColorBoost";
            this.menuFilterColorBoost.Size = new System.Drawing.Size(224, 34);
            this.menuFilterColorBoost.Text = "颜色增强";
            this.menuFilterColorBoost.Click += new System.EventHandler(this.MenuFilterColorBoost_Click);
            // 
            // menuFilterClarity 设置
            // 
            this.menuFilterClarity.Name = "menuFilterClarity";
            this.menuFilterClarity.Size = new System.Drawing.Size(224, 34);
            this.menuFilterClarity.Text = "清晰";
            this.menuFilterClarity.Click += new System.EventHandler(this.MenuFilterClarity_Click);
            // 
            // menuFilterVintage 设置
            // 
            this.menuFilterVintage.Name = "menuFilterVintage";
            this.menuFilterVintage.Size = new System.Drawing.Size(224, 34);
            this.menuFilterVintage.Text = "复古";
            this.menuFilterVintage.Click += new System.EventHandler(this.MenuFilterVintage_Click);
            // 
            // menuFilterVivid 设置
            // 
            this.menuFilterVivid.Name = "menuFilterVivid";
            this.menuFilterVivid.Size = new System.Drawing.Size(224, 34);
            this.menuFilterVivid.Text = "鲜艳";
            this.menuFilterVivid.Click += new System.EventHandler(this.MenuFilterVivid_Click);
            // 
            // menuFilterMoody 设置
            // 
            this.menuFilterMoody.Name = "menuFilterMoody";
            this.menuFilterMoody.Size = new System.Drawing.Size(224, 34);
            this.menuFilterMoody.Text = "阴沉";
            this.menuFilterMoody.Click += new System.EventHandler(this.MenuFilterMoody_Click);
            // 
            // menuFilterWarm 设置
            // 
            this.menuFilterWarm.Name = "menuFilterWarm";
            this.menuFilterWarm.Size = new System.Drawing.Size(224, 34);
            this.menuFilterWarm.Text = "暖调";
            this.menuFilterWarm.Click += new System.EventHandler(this.MenuFilterWarm_Click);
            // 
            // menuFilterCool 设置
            // 
            this.menuFilterCool.Name = "menuFilterCool";
            this.menuFilterCool.Size = new System.Drawing.Size(224, 34);
            this.menuFilterCool.Text = "冷调";
            this.menuFilterCool.Click += new System.EventHandler(this.MenuFilterCool_Click);
            // 
            // menuFilterSoft 设置
            // 
            this.menuFilterSoft.Name = "menuFilterSoft";
            this.menuFilterSoft.Size = new System.Drawing.Size(224, 34);
            this.menuFilterSoft.Text = "柔和";
            this.menuFilterSoft.Click += new System.EventHandler(this.MenuFilterSoft_Click);
            // 
            // menuFilterDramatic 设置
            // 
            this.menuFilterDramatic.Name = "menuFilterDramatic";
            this.menuFilterDramatic.Size = new System.Drawing.Size(224, 34);
            this.menuFilterDramatic.Text = "戏剧性";
            this.menuFilterDramatic.Click += new System.EventHandler(this.MenuFilterDramatic_Click);
            // 
            // toolStripSeparatorFilter 设置
            // 
            this.toolStripSeparatorFilter.Name = "toolStripSeparatorFilter";
            this.toolStripSeparatorFilter.Size = new System.Drawing.Size(221, 6);
            // 
            // trackBarFilterIntensity 设置
            // 
            this.trackBarFilterIntensity.AutoSize = false;
            this.trackBarFilterIntensity.Location = new System.Drawing.Point(0, 0);
            this.trackBarFilterIntensity.Minimum = 0;
            this.trackBarFilterIntensity.Maximum = 100;
            this.trackBarFilterIntensity.Value = 50;
            this.trackBarFilterIntensity.TickFrequency = 10;
            this.trackBarFilterIntensity.Size = new System.Drawing.Size(150, 30);
            // 
            // toolStripHostFilterIntensity 设置
            // 
            this.toolStripHostFilterIntensity.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripHostFilterIntensity.Name = "toolStripHostFilterIntensity";
            this.toolStripHostFilterIntensity.Size = new System.Drawing.Size(150, 30);
            // ---------------------------
            // statusStrip 设置
            // ---------------------------
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 668);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 32);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel 设置
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(61, 25);
            this.statusLabel.Text = "就绪";
            // 
            // imageViewer 设置
            // 
            this.imageViewer.BackColor = System.Drawing.Color.Black;
            this.imageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewer.Location = new System.Drawing.Point(0, 33);
            this.imageViewer.Name = "imageViewer";
            this.imageViewer.Size = new System.Drawing.Size(1200, 635);
            this.imageViewer.TabIndex = 2;
            // 
            // MainForm 设置
            // 
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.imageViewer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "图片+音乐播放器";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFilterIntensity)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
