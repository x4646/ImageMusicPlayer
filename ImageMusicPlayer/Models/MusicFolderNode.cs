using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMusicPlayer.Models
{
    /// <summary>
    /// 表示音乐文件夹节点的类。
    /// </summary>
    public class MusicFolderNode
    {
        /// <summary>
        /// 获取或设置音乐文件夹的名称。
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 获取或设置音乐文件夹的完整路径。
        /// </summary>
        public string? FullPath { get; set; }

        /// <summary>
        /// 获取或设置音乐文件夹的子文件夹列表。
        /// </summary>
        public List<MusicFolderNode> SubFolders { get; set; } = new List<MusicFolderNode>();

        /// <summary>
        /// 获取或设置音乐文件夹中的音乐文件列表。
        /// </summary>
        public List<string> MusicFiles { get; set; } = new List<string>();
    }
}
