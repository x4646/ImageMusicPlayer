using ImageMusicPlayer.DataAccess;
using ImageMusicPlayer.Interfaces;
using ImageMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace ImageMusicPlayer
{
    public class FavoriteManager : IFavoriteManager
    {
        private readonly IRepository<List<FavoriteItem>> repository;
        private readonly string favoritesFile = Path.Combine(Application.StartupPath, "favorites.json");
        private List<FavoriteItem> favorites = [];

        public event EventHandler FavoritesChanged;
        public IReadOnlyList<FavoriteItem> Favorites => favorites.AsReadOnly();

        public FavoriteManager()
          : this(new JsonRepository<List<FavoriteItem>>(Path.Combine(Application.StartupPath, "favorites.json")))
        {
        }

        // 可用于依赖注入测试时注入 repository
        public FavoriteManager(IRepository<List<FavoriteItem>> repository)
        {
            this.repository = repository;
        }

        public void LoadFavorites()
        {
            if (File.Exists(favoritesFile))
            {
                try
                {
                    var loaded = repository.Load();
                    favorites = loaded ?? new List<FavoriteItem>();
                    var json = File.ReadAllText(favoritesFile);
                    OnFavoritesChanged();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"加载收藏夹失败: {ex.Message}");
                    MessageBox.Show($"加载收藏夹失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    favorites = new List<FavoriteItem>();
                    OnFavoritesChanged();
                }
            }
            else
            {
                favorites = new List<FavoriteItem>();
                OnFavoritesChanged();
            }
        }

        public void SaveFavorites()
        {
            try
            {
                repository.Save(favorites);
                var json = JsonSerializer.Serialize(favorites, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(favoritesFile, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"保存收藏夹失败: {ex.Message}");
                MessageBox.Show($"保存收藏夹失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddFolderFavorite(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath) && !favorites.Exists(f => f.Type == FavoriteType.Folder && f.Path.Equals(folderPath, StringComparison.OrdinalIgnoreCase)))
            {
                favorites.Add(new FavoriteItem { Type = FavoriteType.Folder, Path = folderPath });
                SaveFavorites();
                OnFavoritesChanged();
            }
        }

        public void AddImageFavorite(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && !favorites.Exists(f => f?.Type == FavoriteType.SingleImage && f.Path.Equals(imagePath, StringComparison.OrdinalIgnoreCase)))
            {
                favorites.Add(new FavoriteItem { Type = FavoriteType.SingleImage, Path = imagePath });
                SaveFavorites();
                OnFavoritesChanged();
            }
        }

        public void RemoveFavorite(FavoriteItem item)
        {
            if (favorites.Remove(item))
            {
                SaveFavorites();
                OnFavoritesChanged();
            }
        }

        public void RemoveFavorite(string path, FavoriteType type)
        {
            var item = favorites.Find(f => f.Type == type && f.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                RemoveFavorite(item);
            }
        }

        protected virtual void OnFavoritesChanged()
        {
            FavoritesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
