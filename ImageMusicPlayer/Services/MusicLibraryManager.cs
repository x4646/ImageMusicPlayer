using ImageMusicPlayer.Interfaces;
using ImageMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace ImageMusicPlayer
{
    public class MusicLibraryManager : IMusicLibraryManager
    {
        private readonly string libraryFile = Path.Combine(Application.StartupPath, "musicFolders.json");

        public List<MusicFolderNode> RootFolders { get; private set; } = new List<MusicFolderNode>();

        public void LoadLibrary()
        {
            if (File.Exists(libraryFile))
            {
                try
                {
                    string json = File.ReadAllText(libraryFile);
                    RootFolders = JsonSerializer.Deserialize<List<MusicFolderNode>>(json) ?? new List<MusicFolderNode>();
                }
                catch
                {
                    RootFolders = new List<MusicFolderNode>();
                }
            }
        }

        public void SaveLibrary()
        {
            try
            {
                string json = JsonSerializer.Serialize(RootFolders, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(libraryFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存音乐文件夹出错: {ex.Message}");
            }
        }

        public void AddFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            var node = BuildFolderNode(folderPath);
            RootFolders.Add(node);
            SaveLibrary();
        }

        public void RemoveFolder(string folderFullPath)
        {
            RootFolders.RemoveAll(n => string.Equals(n.FullPath, folderFullPath, StringComparison.OrdinalIgnoreCase));
            SaveLibrary();
        }

        public List<string> GetAllMusicFiles()
        {
            var all = new List<string>();
            foreach (var root in RootFolders)
            {
                TraverseNode(root, all);
            }
            return all;
        }

        private void TraverseNode(MusicFolderNode node, List<string> list)
        {
            list.AddRange(node.MusicFiles);
            foreach (var sub in node.SubFolders)
            {
                TraverseNode(sub, list);
            }
        }

        private MusicFolderNode BuildFolderNode(string folderPath)
        {
            var node = new MusicFolderNode
            {
                Name = Path.GetFileName(folderPath),
                FullPath = folderPath
            };

            var musicFiles = Directory.GetFiles(folderPath)
                .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                .ToList();

            node.MusicFiles.AddRange(musicFiles);

            var subFolders = Directory.GetDirectories(folderPath);
            foreach (var sub in subFolders)
            {
                node.SubFolders.Add(BuildFolderNode(sub));
            }

            return node;
        }
    }
}
