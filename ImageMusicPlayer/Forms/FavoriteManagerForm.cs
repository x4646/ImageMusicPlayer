using ImageMusicPlayer.Interfaces;
using ImageMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ImageMusicPlayer
{
    public partial class FavoriteManagerForm : Form
    {
        private IFavoriteManager favoriteManager;
        // 新增属性，用于返回用户选中的收藏文件夹
        public string SelectedFolder { get; private set; }

        // 用于返回用户选择用于导入图片的收藏文件夹
        public List<FavoriteItem> SelectedFavoritesForImport { get; private set; } = [];

        // 用于返回用户选择用于删除图片的收藏文件夹
        public List<FavoriteItem> SelectedFavoritesForDeletion { get; private set; } = [];


        /// <summary>
        /// 处理加载收藏双击某项图片或是文件夹的事件。
        /// </summary>
        public Action<FavoriteItem> HandleFavoriteLoadManager;

        /// <summary>
        /// 处理删除删除收藏项的事件。
        /// </summary>
        public Action<List<FavoriteItem>> HandleFavoriteDeletionManager;
        public FavoriteManagerForm(IFavoriteManager favoriteManager)
        {
            InitializeComponent();
            this.favoriteManager = favoriteManager;
            LoadFavorites();
        }

        // 修改 LoadFavorites 方法，使用 DisplayText 显示
        private void LoadFavorites()
        {
            listViewFavorites.Items.Clear();
            foreach (var fav in favoriteManager.Favorites)
            {
                listViewFavorites.Items.Add(fav.DisplayText);
            }
        }

        // 新增：添加文件夹收藏
        private void BtnAddFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    favoriteManager.AddFolderFavorite(fbd.SelectedPath);
                    LoadFavorites();
                }
            }
        }

        // 新增：添加图片收藏
        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "图片文件|*.jpg;*.jpeg;*.png";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    favoriteManager.AddImageFavorite(dialog.FileName);
                    LoadFavorites();
                }
            }
        }

        private void ListViewFavorites_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFavorites.SelectedItems.Count == 1)
            {
                // 根据显示文本找到对应的收藏项
                string selectedText = listViewFavorites.SelectedItems[0].Text;
                var fav = favoriteManager.Favorites.FirstOrDefault(f => f.DisplayText == selectedText);
                if (fav != null)
                {
                    SelectedFavoritesForImport.Clear();
                    SelectedFavoritesForImport.Add(fav);

                    HandleFavoriteLoadManager?.Invoke(fav);
                }
            }
        }


        // 原有的添加收藏功能
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("是否收藏当前图片？\n点击“是”收藏当前图片所在文件夹；点击“否”收藏文件夹。",
                                          "添加收藏",
                                          MessageBoxButtons.YesNoCancel,
                                          MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string imageFolder = Prompt.ShowDialog("请输入当前图片所在的文件夹路径：", "添加收藏 - 图片");
                if (!string.IsNullOrEmpty(imageFolder))
                {
                    favoriteManager.AddFolderFavorite(imageFolder);
                    LoadFavorites();
                }
            }
            else if (result == DialogResult.No)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        favoriteManager.AddFolderFavorite(fbd.SelectedPath);
                        LoadFavorites();
                    }
                }
            }
            // Cancel则不做操作
        }

        // 原有删除收藏功能
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (listViewFavorites.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择至少一个收藏项以删除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<FavoriteItem> favs = [];

            foreach (ListViewItem item in listViewFavorites.SelectedItems)
            {
                var fav = favoriteManager.Favorites.FirstOrDefault(f => f.DisplayText == item.Text);
                if (fav != null)
                {
                    favoriteManager.RemoveFavorite(fav);
                    favs.Add(fav);
                }
            }

            LoadFavorites();


            HandleFavoriteDeletionManager?.Invoke(favs);
        }

        // 修改：导入图片按钮，支持多选
        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (listViewFavorites.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择至少一个收藏项以导入图片。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 如果选中多个，则询问确认
            if (listViewFavorites.SelectedItems.Count > 1)
            {
                var confirm = MessageBox.Show("确定导入选中的多个收藏项吗？", "确认导入", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                    return;
            }

            SelectedFavoritesForImport.Clear();
            foreach (ListViewItem item in listViewFavorites.SelectedItems)
            {
                var fav = favoriteManager.Favorites.FirstOrDefault(f => f.DisplayText == item.Text);
                if (fav != null)
                {
                    SelectedFavoritesForImport.Add(fav);
                }
            }
        }


        // 修改：删除图片按钮，支持多选
        private void BtnDeleteImages_Click(object sender, EventArgs e)
        {
            List<FavoriteItem> favs = [];
            if (listViewFavorites.SelectedItems.Count > 0)
            {
                SelectedFavoritesForDeletion.Clear();
                foreach (ListViewItem item in listViewFavorites.SelectedItems)
                {
                    var fav = favoriteManager.Favorites.FirstOrDefault(f => f.DisplayText == item.Text);
                    if (fav != null)
                    {
                        SelectedFavoritesForDeletion.Add(fav);
                        favs.Add(fav);
                    }
                }
                MessageBox.Show($"选中删除收藏项：{string.Join(", ", SelectedFavoritesForDeletion.Select(f => f.Path))}",
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 不关闭窗口
                HandleFavoriteDeletionManager?.Invoke(favs);
            }
            else
            {
                MessageBox.Show("请选择至少一个收藏项以删除。",
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // btnClose_Click 保持不变
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
