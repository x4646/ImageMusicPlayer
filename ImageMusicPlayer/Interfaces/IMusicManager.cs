using ImageMusicPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageMusicPlayer.Interfaces
{
    /// <summary>
    /// 音乐管理器接口
    /// </summary>
    public interface IMusicManager
    {
        /// <summary>
        /// 所有导入的音乐文件
        /// </summary>
        List<MusicItem> AllMusic { get; }

        /// <summary>
        /// 按文件夹分组，动态生成字典
        /// </summary>
        Dictionary<string, List<MusicItem>> MusicByFolder { get; }

        /// <summary>
        /// 扫描指定文件夹（包括子文件夹），导入音乐文件（MP3, WAV, WMA）
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>异步任务</returns>
        Task ImportMusicFromFolderAsync(string folderPath);

        /// <summary>
        /// 返回指定文件夹（包括子文件夹）下的所有音乐
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>音乐列表</returns>
        List<MusicItem> GetMusicByFolder(string folderPath);

        /// <summary>
        /// 删除指定路径的音乐项。
        /// </summary>
        /// <param name="musicPath">音乐文件路径</param>
        void DeleteMusic(string musicPath);

        /// <summary>
        /// 根据关键词在所有音乐中搜索匹配项。
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>匹配的音乐项列表</returns>
        List<MusicItem> Search(string keyword);

        /// <summary>
        /// 刷新音乐列表，例如重新扫描文件夹。
        /// </summary>
        void RefreshLibrary();

    }
}
