using ImageMusicPlayer.Models;
using System;
using System.Collections.Generic;

namespace ImageMusicPlayer.Interfaces
{
    /// <summary>
    /// 收藏夹管理器接口
    /// </summary>
    public interface IFavoriteManager
    {
        /// <summary>
        /// 收藏夹发生改变时的事件
        /// </summary>
        event EventHandler FavoritesChanged;

        /// <summary>
        /// 获取收藏夹列表
        /// </summary>
        IReadOnlyList<FavoriteItem> Favorites { get; }

        /// <summary>
        /// 加载收藏夹
        /// </summary>
        void LoadFavorites();

        /// <summary>
        /// 保存收藏夹
        /// </summary>
        void SaveFavorites();

        /// <summary>
        /// 添加文件夹收藏
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        void AddFolderFavorite(string folderPath);

        /// <summary>
        /// 添加图片收藏
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        void AddImageFavorite(string imagePath);

        /// <summary>
        /// 移除收藏
        /// </summary>
        /// <param name="item">要移除的收藏项</param>
        void RemoveFavorite(FavoriteItem item);

        /// <summary>
        /// 移除收藏
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">类型</param>
        void RemoveFavorite(string path, FavoriteType type);
    }
}
