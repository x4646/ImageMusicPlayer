using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ImageMusicPlayer.Models;
using ImageMusicPlayer.Interfaces;

namespace ImageMusicPlayer
{
    public class MusicManager : IMusicManager
    {
        // 所有导入的音乐文件
        public List<MusicItem> AllMusic { get; private set; } = new List<MusicItem>();

        // 按文件夹分组，动态生成字典
        public Dictionary<string, List<MusicItem>> MusicByFolder
        {
            get { return AllMusic.GroupBy(m => m.FolderPath ?? string.Empty).ToDictionary(g => g.Key, g => g.ToList()); }
        }

        /// <summary>
        /// 扫描指定文件夹（包括子文件夹），导入音乐文件（MP3, WAV, WMA）
        /// </summary>
        public async Task ImportMusicFromFolderAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            List<string> files = new List<string>();
            await Task.Run(() =>
            {
                files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".wma", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            });

            foreach (var file in files)
            {
                // 避免重复导入
                if (!AllMusic.Any(m => m.FilePath != null && m.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)))
                {
                    string? fileFolderPath = Path.GetDirectoryName(file);
                    if (fileFolderPath != null && !AllMusic.Any(m => m.FilePath != null && m.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)))
                    {
                        AllMusic.Add(new MusicItem { FilePath = file, FolderPath = fileFolderPath });
                    }
                }
            }
        }

        /// <summary>
        /// 返回指定文件夹（包括子文件夹）下的所有音乐
        /// </summary>
        public List<MusicItem> GetMusicByFolder(string folderPath)
        {
            return AllMusic.Where(m =>
                   m.FolderPath != null && (m.FolderPath.Equals(folderPath, StringComparison.OrdinalIgnoreCase)
                || m.FolderPath.StartsWith(folderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public void DeleteMusic(string musicPath)
        {
            throw new NotImplementedException();
        }

        public List<MusicItem> Search(string keyword)
        {
            throw new NotImplementedException();
        }

        public void RefreshLibrary()
        {
            throw new NotImplementedException();
        }
    }
}
