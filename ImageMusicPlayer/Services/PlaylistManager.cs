using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ImageMusicPlayer.Interfaces;

namespace ImageMusicPlayer
{
    public class PlaylistManager : IPlaylistManager
    {
        private readonly string playlistFile = Path.Combine(Application.StartupPath, "playlist.json");
        private List<string> playlist = new List<string>();

        public IReadOnlyList<string> Playlist => playlist.AsReadOnly();

        public void LoadPlaylist()
        {
            if (File.Exists(playlistFile))
            {
                try
                {
                    string json = File.ReadAllText(playlistFile);
                    playlist = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
                catch
                {
                    playlist = new List<string>();
                }
            }
        }

        public void SavePlaylist()
        {
            string json = JsonSerializer.Serialize(playlist, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(playlistFile, json);
        }

        public void Add(string filePath)
        {
            if (!playlist.Contains(filePath, StringComparer.OrdinalIgnoreCase))
                playlist.Add(filePath);
        }

        public void Remove(string filePath)
        {
            playlist.RemoveAll(p => p.Equals(filePath, StringComparison.OrdinalIgnoreCase));
        }

        public void Clear()
        {
            playlist.Clear();
        }

        public void AddRange(IEnumerable<string> filePaths)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string filePath)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
