using System.Collections.Generic;

namespace ImageMusicPlayer.Interfaces
{
    /// <summary>
    /// 播放列表管理接口，支持播放列表的加载、保存、增删清除等操作。
    /// </summary>
    public interface IPlaylistManager
    {
        /// <summary>
        /// 获取当前播放列表的只读视图。
        /// </summary>
        IReadOnlyList<string> Playlist { get; }

        /// <summary>
        /// 加载播放列表数据。
        /// </summary>
        void LoadPlaylist();

        /// <summary>
        /// 保存当前播放列表数据。
        /// </summary>
        void SavePlaylist();

        /// <summary>
        /// 向播放列表中添加一个音乐文件路径。
        /// </summary>
        /// <param name="filePath">要添加的音乐文件路径</param>
        void Add(string filePath);

        /// <summary>
        /// 从播放列表中移除指定路径的音乐文件。
        /// </summary>
        /// <param name="filePath">要移除的音乐文件路径</param>
        void Remove(string filePath);

        /// <summary>
        /// 清空整个播放列表。
        /// </summary>
        void Clear();
        /// <summary>
        /// 批量添加多个音乐文件到播放列表中。
        /// </summary>
        /// <param name="filePaths">多个音乐文件路径</param>
        void AddRange(IEnumerable<string> filePaths);

        /// <summary>
        /// 判断指定音乐文件是否已在播放列表中。
        /// </summary>
        /// <param name="filePath">音乐文件路径</param>
        bool Contains(string filePath);

        /// <summary>
        /// 根据索引移除播放列表中的项。
        /// </summary>
        /// <param name="index">索引位置</param>
        void RemoveAt(int index);

    }
}
