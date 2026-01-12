using ImageMusicPlayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WMPLib;

namespace ImageMusicPlayer
{
    public class MusicPlayer : IMusicPlayer
    {
        private WindowsMediaPlayer player = new WindowsMediaPlayer();
        private List<string> playPathList = new List<string>();
        private int currentIndex = 0;
        private readonly string lastPlayedFile = Path.Combine(Application.StartupPath, "lastplayed.json");
        public string? CurrentPath { get; private set; }

        public PlayMode Mode { get; set; } = PlayMode.Sequence;

        public List<string> PlayPathList
        {
            get => playPathList;
            set
            {
                playPathList = value ?? new List<string>();
                currentIndex = 0;
            }
        }

        public void Play(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return;

            // 清空当前播放列表，并创建一个新的播放列表（base URL 设为空）
            player.currentPlaylist.clear();
            IWMPPlaylist playlist = player.newPlaylist("MyPlaylist", "");

            // 查找当前要播放歌曲在列表中的索引
            currentIndex = playPathList.FindIndex(p => p.Equals(path, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0) return;

            // 重新组合播放列表：先添加从当前歌曲到列表末尾的歌曲，再添加从开头到当前歌曲之前的歌曲
            for (int i = currentIndex; i < playPathList.Count; i++)
            {
                playlist.appendItem(player.newMedia(playPathList[i]));
            }
            for (int i = 0; i < currentIndex; i++)
            {
                playlist.appendItem(player.newMedia(playPathList[i]));
            }
            player.currentPlaylist = playlist;

            // 根据不同播放模式设置：顺序、循环、随机
            switch (Mode)
            {
                case PlayMode.Sequence:
                    player.settings.setMode("loop", false);
                    player.settings.setMode("shuffle", false);
                    break;
                case PlayMode.Loop:
                    player.settings.setMode("loop", true);
                    player.settings.setMode("shuffle", false);
                    break;
                case PlayMode.Random:
                    player.settings.setMode("shuffle", true);
                    // 如有需要，也可以开启 loop 使随机播放列表循环
                    // player.settings.setMode("loop", true);
                    break;
                default:
                    break;
            }

            CurrentPath = path;
            player.controls.play();
            File.WriteAllText(lastPlayedFile, JsonSerializer.Serialize(path));
        }

        public string? LoadLastPlayed()
        {
            try
            {
                if (File.Exists(lastPlayedFile))
                {
                    string json = File.ReadAllText(lastPlayedFile);
                    return JsonSerializer.Deserialize<string>(json);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public void Pause()
        {
            player.controls.pause();
        }

        public void PlayNext()
        {
            if (playPathList.Count == 0) return;

            switch (Mode)
            {
                case PlayMode.Sequence:
                    currentIndex = (currentIndex + 1) % playPathList.Count;
                    break;
                case PlayMode.Loop:
                    // 如果希望单曲循环可以不改变 currentIndex，这里采用整个列表循环（根据需求调整）
                    currentIndex = (currentIndex + 1) % playPathList.Count;
                    break;
                case PlayMode.Random:
                    Random rnd = new Random();
                    currentIndex = rnd.Next(playPathList.Count);
                    break;
            }

            Play(playPathList[currentIndex]);
        }

        public void PlayPrevious()
        {
            if (playPathList.Count == 0) return;

            if (Mode == PlayMode.Random)
            {
                Random rnd = new Random();
                currentIndex = rnd.Next(playPathList.Count);
            }
            else
            {
                currentIndex = (currentIndex - 1 + playPathList.Count) % playPathList.Count;
            }

            Play(playPathList[currentIndex]);
        }

        public int GetVolume()
        {
            return player.settings.volume;
        }

        public void SetVolume(int value)
        {
            player.settings.volume = value;
        }

        public void RemoveCurrentFromPlaylist()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void PlayAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
