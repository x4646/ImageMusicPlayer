using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace ImageMusicPlayer
{
    public class MusicFolderManager
    {
        private readonly string musicFoldersFile = Path.Combine(Application.StartupPath, "musicfolders.json");
        private List<string> musicFolders = new List<string>();

        public event EventHandler MusicFoldersChanged = delegate { };
        public IReadOnlyList<string> MusicFolders => musicFolders.AsReadOnly();

        public MusicFolderManager()
        {
            // 构造时不自动加载，外部调用 LoadMusicFolders
        }

        public void LoadMusicFolders()
        {
            if (File.Exists(musicFoldersFile))
            {
                try
                {
                    var json = File.ReadAllText(musicFoldersFile);
                    musicFolders = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                    OnMusicFoldersChanged();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"加载音乐文件夹失败: {ex.Message}");
                    MessageBox.Show($"加载音乐文件夹失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    musicFolders = new List<string>();
                    OnMusicFoldersChanged();
                }
            }
            else
            {
                musicFolders = new List<string>();
                OnMusicFoldersChanged();
            }
        }

        public void SaveMusicFolders()
        {
            try
            {
                var json = JsonSerializer.Serialize(musicFolders, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(musicFoldersFile, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"保存音乐文件夹失败: {ex.Message}");
                MessageBox.Show($"保存音乐文件夹失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddMusicFolder(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath) && !musicFolders.Exists(f => f.Equals(folderPath, StringComparison.OrdinalIgnoreCase)))
            {
                musicFolders.Add(folderPath);
                SaveMusicFolders();
                OnMusicFoldersChanged();
            }
        }

        public void RemoveMusicFolder(string folderPath)
        {
            if (musicFolders.RemoveAll(f => f.Equals(folderPath, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                SaveMusicFolders();
                OnMusicFoldersChanged();
            }
        }

        protected virtual void OnMusicFoldersChanged()
        {
            MusicFoldersChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
