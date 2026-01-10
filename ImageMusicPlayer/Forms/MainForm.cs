using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using ImageMusicPlayer.Models;
using ImageMusicPlayer.Services;
using ImageMusicPlayer.Interfaces;
using ImageMusicPlayer.Utils;

namespace ImageMusicPlayer
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 用于键盘事件节流的 Stopwatch。
        /// </summary>
        private readonly Stopwatch keyStopwatch = new();

        /// <summary>
        /// 用于全屏模式下自动隐藏 UI 的定时器。
        /// </summary>
        private readonly Timer hideUITimer;

        /// <summary>
        /// 幻灯片延时，单位毫秒（例如 1500 毫秒）。
        /// </summary>
        private int slideshowDelay = 1500;

        /// <summary>
        /// 用于判断幻灯片是否在播放中的任务引用。
        /// </summary>
        private Task? slideShowTask;

        /// <summary>
        /// 管理图片相关操作的对象。
        /// </summary>
        private readonly IImageManager imageManager;

        /// <summary>
        /// 管理收藏夹相关操作的对象。
        /// </summary>
        private readonly IFavoriteManager favoriteManager;


        /// <summary>
        /// 管理音乐相关操作的对象。
        /// </summary>
        private readonly IMusicManager musicManager;


        /// <summary>
        /// 管理音乐库相关操作的对象。
        /// </summary>
        private readonly IMusicLibraryManager musicLibraryManager;

        /// <summary>
        /// 管理播放列表相关操作的对象。
        /// </summary>
        private readonly IPlaylistManager playlistManager;

        /// <summary>
        /// 用于播放音乐的对象。
        /// </summary>
        private readonly IMusicPlayer musicPlayer;

        private FavoriteManagerForm? favoriteManagerFormInstance;


        public MainForm(IImageManager imageManager,
                    IFavoriteManager favoriteManager,
                    IMusicPlayer musicPlayer,
                    IMusicManager musicManager,
                    IMusicLibraryManager musicLibraryManager,
                    IPlaylistManager playlistManager)
        {
            InitializeComponent();
            this.KeyPreview = true; // 让空格等快捷键在控件获得焦点时也能被窗体捕获
            this.imageManager = imageManager;
            this.imageManager.Init(imageViewer, UpdateStatus, 5);
            this.musicPlayer = musicPlayer;
            this.favoriteManager = favoriteManager;
            this.musicManager = musicManager;
            this.musicLibraryManager = musicLibraryManager;
            this.playlistManager = playlistManager;


            if (this.favoriteManager == null) throw new Exception("FavoriteManager is null");
            if (this.imageManager == null) throw new Exception("imageManager is null");
            if (this.musicPlayer == null) throw new Exception("musicPlayer is null");
            if (this.musicManager == null) throw new Exception("musicManager is null");
            if (this.musicLibraryManager == null) throw new Exception("musicLibraryManager is null");
            if (this.playlistManager == null) throw new Exception("playlistManager is null");

            this.favoriteManager.FavoritesChanged += FavoriteManager_FavoritesChanged;
            imageViewer.ZoomChanged += (s, e) => UpdateStatus();

            keyStopwatch.Start();
            hideUITimer = new Timer { Interval = 3000 };

            hideUITimer.Tick += HideUITimer_Tick;
            //滤镜滑块移动
            trackBarFilterIntensity.Scroll += (s, e) =>
            {
                imageViewer.FilterIntensity = trackBarFilterIntensity.Value / 100f;
                imageViewer.Invalidate();
            };


            this.Load += (sender, e) => { ScreenHelper.EnableKeepScreenOn(); };

            this.FormClosing += (sender, e) => { ScreenHelper.DisableKeepScreenOn(); };
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            // 初始化完成后先刷新一次状态栏
            UpdateStatus();
        }


        /// <summary>
        /// 窗体加载时的处理。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            favoriteManager?.LoadFavorites();
        }

        /// <summary>
        /// 收藏夹变化事件处理。
        /// </summary>
        private void FavoriteManager_FavoritesChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine("FavoritesChanged event triggered in MainForm.");
            // 如有需要，可更新收藏相关UI
        }

        // ------------------- 幻灯片播放控制（用于空格暂停/继续） ----------------------

        private bool IsSlideShowRunning => slideShowTask != null && !slideShowTask.IsCompleted;

        private void StartSlideShow()
        {
            if (IsSlideShowRunning) return;
            // 在 UI 线程上启动（不要 Task.Run），避免跨线程操作控件
            slideShowTask = RunSlideShowAsync();
        }

        private async Task RunSlideShowAsync()
        {
            try
            {
                await imageManager.StartSlideShowAsync(() => slideshowDelay);
            }
            finally
            {
                slideShowTask = null;
            }
        }

        private void ToggleSlideShow()
        {
            if (IsSlideShowRunning)
            {
                imageManager.PauseSlideShow();
            }
            else
            {
                StartSlideShow();
            }

            UpdateStatus();
        }

        // ------------------- 菜单项事件处理 ----------------------

        /// <summary>
        /// 添加图片菜单项点击事件处理。
        /// </summary>
        private async void MenuAddImages_Click(object? sender, EventArgs e)
        {
            await imageManager.SelectAndLoadFromFolderAsync();
        }

        /// <summary>
        /// 追加图片菜单项点击事件处理。
        /// </summary>
        private async void MenuAppendImages_Click(object? sender, EventArgs e)
        {
            await imageManager.SelectAndAppendImagesAsync();
        }

        /// <summary>
        /// 下一张图片菜单项点击事件处理。
        /// </summary>
        private void MenuNextImage_Click(object? sender, EventArgs e)
        {
            imageManager.NextImage();
            UpdateStatus();
        }

        /// <summary>
        /// 添加音乐菜单项点击事件处理。
        /// </summary>
        private async void MenuAddMusic_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new())
            {
                fbd.Description = "请选择要导入音乐的文件夹：";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    await musicManager.ImportMusicFromFolderAsync(fbd.SelectedPath);
                }
            }
        }

        /// <summary>
        /// 下一首音乐菜单项点击事件处理。
        /// </summary>
        private void MenuNextMusic_Click(object? sender, EventArgs e)
        {
            musicPlayer?.PlayNext();
            UpdateStatus();
        }

        /// <summary>
        /// 开始幻灯片菜单项点击事件处理。
        /// </summary>
        private void MenuStartSlide_Click(object? sender, EventArgs e)
        {
            StartSlideShow();
        }

        /// <summary>
        /// 暂停幻灯片菜单项点击事件处理。
        /// </summary>
        private void MenuPauseSlide_Click(object? sender, EventArgs e)
        {
            imageManager.PauseSlideShow();
            UpdateStatus();
        }

        /// <summary>
        /// 切换全屏模式菜单项点击事件处理。
        /// </summary>
        private void MenuToggleFullscreen_Click(object? sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        /// <summary>
        /// 添加当前图片到收藏夹菜单项点击事件处理。
        /// </summary>
        private void MenuAddFavorite_Click(object? sender, EventArgs e)
        {
            var path = imageManager.CurrentImagePath;
            if (!string.IsNullOrEmpty(path))
            {
                string? directory = Path.GetDirectoryName(path);
                if (directory != null)
                {
                    favoriteManager?.AddFolderFavorite(directory);
                }
            }
        }

        /// <summary>
        /// 显示收藏夹菜单项点击事件处理。
        /// </summary>
        private async void MenuShowFavorites_Click(object? sender, EventArgs e)
        {
            if (favoriteManagerFormInstance == null || favoriteManagerFormInstance.IsDisposed)
            {
                favoriteManagerFormInstance = new FavoriteManagerForm(favoriteManager);
                favoriteManagerFormInstance.FormClosed += (s, args) => favoriteManagerFormInstance = null;
                favoriteManagerFormInstance.HandleFavoriteLoadManager += async (path) =>
                {
                    await HandleFavoriteManagerLoadFormActions([path]);
                    UpdateStatus();
                };

                favoriteManagerFormInstance.HandleFavoriteDeletionManager +=
                    async (favs) =>
                    {
                        await HandleFavoriteManagerDeletionFormActions(favs);
                        UpdateStatus();
                    };


                favoriteManagerFormInstance.ShowDialog();
            }
            else
            {
                favoriteManagerFormInstance.Activate();
            }
        }


        /// <summary>
        /// 处理收藏夹管理窗体的操作。
        /// </summary>
        private async Task HandleFavoriteManagerLoadFormActions(List<FavoriteItem> favs)
        {
            foreach (var fav in favs)
            {
                if (fav.Type == FavoriteType.Folder)
                {
                    await imageManager.LoadFromFolderAsync(fav.Path);
                }
                else if (fav.Type == FavoriteType.SingleImage)
                {
                    await imageManager.SelectAndAppendImagesAsync([fav.Path]);
                }
            }
        }

        private async Task HandleFavoriteManagerDeletionFormActions(List<FavoriteItem> favs)
        {
            foreach (var fav in favs)
            {
                if (fav.Type == FavoriteType.Folder)
                {
                    await imageManager.RemoveImagesFromFolder(fav.Path);
                }
                else if (fav.Type == FavoriteType.SingleImage)
                {
                    await imageManager.RemoveImage(fav.Path);
                }
            }
        }

        /// <summary>
        /// 移除文件夹中的图片菜单项点击事件处理。
        /// </summary>
        private void MenuRemoveFolderImages_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择要删除图片的文件夹（包括其子目录）：";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var confirm = MessageBox.Show($"确定删除从文件夹 [{fbd.SelectedPath}] 导入的所有图片吗？",
                                                  "确认删除",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);
                    if (confirm == DialogResult.Yes)
                    {
                        imageManager.RemoveImagesFromFolder(fbd.SelectedPath);
                    }
                }
            }
        }

        // --------------------- 音乐管理窗口事件处理 ---------------------
        /// <summary>
        /// 音乐管理菜单项点击事件处理。
        /// </summary>
        private async void MenuMusicManagement_Click(object? sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                var musicForm = new MusicManagerForm(musicLibraryManager, playlistManager, musicPlayer);
                this.Invoke((Action)(() => musicForm.Show()));
            });
            UpdateStatus();
        }

        // --------------------- 滤镜菜单事件处理 ---------------------

        private void MenuFilterNone_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.None);
        }

        private void MenuFilterColorBoost_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.ColorBoost);
        }

        private void MenuFilterClarity_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Clarity);
        }

        private void MenuFilterVintage_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Vintage);
        }

        private void MenuFilterVivid_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Vivid);
        }

        private void MenuFilterMoody_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Moody);
        }

        private void MenuFilterWarm_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Warm);
        }

        private void MenuFilterCool_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Cool);
        }

        private void MenuFilterSoft_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Soft);
        }

        private void MenuFilterDramatic_Click(object? sender, EventArgs e)
        {
            ApplyFilter(FilterType.Dramatic);
        }

        /// <summary>
        /// 应用指定的滤镜类型。
        /// </summary>
        /// <param name="filterType">滤镜类型。</param>
        private void ApplyFilter(FilterType filterType)
        {
            imageViewer.CurrentFilter = filterType;
            imageViewer.Invalidate();
        }

        // --------------------- 滑块事件绑定（若未在 Designer 中绑定，可备用） ---------------------

        /// <summary>
        /// 滤镜强度滑块滚动事件处理。
        /// </summary>
        private void TrackBarFilterIntensity_Scroll(object? sender, EventArgs e)
        {
            imageViewer.FilterIntensity = trackBarFilterIntensity.Value / 100f;
            imageViewer.Invalidate();
        }

        // --------------------- 其它功能 ------------------------

        /// <summary>
        /// 切换全屏模式。
        /// </summary>
        private void ToggleFullScreen()
        {
            if (this.FormBorderStyle != FormBorderStyle.None)
            {
                EnterFullScreenMode();
            }
            else
            {
                ExitFullScreenMode();
            }
        }

        /// <summary>
        /// 进入全屏模式。
        /// </summary>
        private void EnterFullScreenMode()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            menuStrip.Visible = false;
            statusStrip.Visible = false;
            hideUITimer.Stop();
            hideUITimer.Start();
        }

        /// <summary>
        /// 退出全屏模式。
        /// </summary>
        private void ExitFullScreenMode()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
            hideUITimer.Stop();
            menuStrip.Visible = true;
            statusStrip.Visible = true;
        }

        /// <summary>
        /// 更新状态栏信息。
        /// </summary>
        private void UpdateStatus()
        {
            string filename = Path.GetFileName(imageManager.CurrentImagePath) ?? "无图片";
            int current = imageManager.CurrentIndex + 1;
            int total = imageManager.TotalCount;
            int cached = imageManager.CacheCount;
            statusLabel.Text = $"图片: {filename} ({current}/{total}) | 缓存: {cached} 张 | 幻灯片速度: {slideshowDelay}ms | 音量: {musicPlayer.GetVolume() * 0.01:P0} | 缩放比例: {imageViewer.Zoom * 100:F0}%";
        }

        /// <summary>
        /// 调整音量。
        /// </summary>
        /// <param name="delta">音量变化值。</param>
        private void AdjustVolume(int delta)
        {
            int volume = musicPlayer.GetVolume() + delta;
            musicPlayer.SetVolume(Math.Clamp(volume, 0, 100));
            UpdateStatus();
        }

        /// <summary>
        /// 隐藏 UI 定时器的 Tick 事件处理。
        /// </summary>
        private void HideUITimer_Tick(object? sender, EventArgs e)
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                menuStrip.Visible = false;
                statusStrip.Visible = false;
            }
        }

        /// <summary>
        /// 处理键盘快捷键。
        /// </summary>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            // 如果上次按键时间小于200毫秒，则忽略本次按键
            if (keyStopwatch.ElapsedMilliseconds < 200)
                return base.ProcessCmdKey(ref msg, keyData);
            keyStopwatch.Restart();

            // 如果当前是全屏模式，显示菜单和状态栏，并重置隐藏UI的定时器
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                menuStrip.Visible = true;
                statusStrip.Visible = true;
                hideUITimer.Stop();
                hideUITimer.Start();
            }

            // 处理不同的按键事件
            switch (keyData)
            {
                case Keys.Right: // 右箭头键，切换到下一张图片
                    imageManager.NextImage();
                    UpdateStatus();
                    return true;
                case Keys.Left: // 左箭头键，切换到上一张图片
                    imageManager.PreviousImage();
                    UpdateStatus();
                    return true;
                case Keys.Up: // 上箭头键，增加幻灯片延时
                    slideshowDelay += 100;
                    UpdateStatus();
                    return true;
                case Keys.Down: // 下箭头键，减少幻灯片延时
                    slideshowDelay = Math.Max(100, slideshowDelay - 100);
                    UpdateStatus();
                    return true;
                case Keys.PageUp: // PageUp键，增加音量
                    AdjustVolume(10);
                    return true;
                case Keys.PageDown: // PageDown键，减少音量
                    AdjustVolume(-10);
                    return true;
                case Keys.Space: // 空格键：暂停 / 继续幻灯片
                    ToggleSlideShow();
                    return true;
                case Keys.F11: // F11键，切换全屏模式
                    ToggleFullScreen();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}

