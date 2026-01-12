using ImageMusicPlayer;
using ImageMusicPlayer.Models;
using System.Collections.Generic;

namespace ImageMusicPlayer.Interfaces
{

    /// <summary>
    /// 音乐库管理接口，用于加载、保存音乐库结构及文件夹管理。
    /// </summary>
    public interface IMusicLibraryManager
    {
        /// <summary>
        /// 获取或设置音乐库的根文件夹列表。
        /// </summary>
        List<MusicFolderNode> RootFolders { get; }

        /// <summary>
        /// 加载音乐库。
        /// </summary>
        void LoadLibrary();

        /// <summary>
        /// 保存音乐库。
        /// </summary>
        void SaveLibrary();

        /// <summary>
        /// 添加文件夹到音乐库。
        /// </summary>
        /// <param name="folderPath">文件夹路径。</param>
        void AddFolder(string folderPath);

        /// <summary>
        /// 从音乐库中移除文件夹。
        /// </summary>
        /// <param name="folderFullPath">文件夹完整路径。</param>
        void RemoveFolder(string folderFullPath);

        /// <summary>
        /// 获取所有音乐文件。
        /// </summary>
        /// <returns>音乐文件列表。</returns>
        List<string> GetAllMusicFiles();

    }
}