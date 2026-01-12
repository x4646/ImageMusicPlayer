using System.Collections.Generic;

namespace ImageMusicPlayer.Interfaces
{
    /// <summary>
    /// 音乐播放器接口
    /// </summary>
    public interface IMusicPlayer
    {
        /// <summary>
        /// 从当前播放列表中移除当前正在播放的音乐。
        /// </summary>
        void RemoveCurrentFromPlaylist();

        /// <summary>
        /// 停止播放（并释放资源）。
        /// </summary>
        void Stop();

        /// <summary>
        /// 跳转到指定索引的音乐文件进行播放。
        /// </summary>
        /// <param name="index">播放列表中的索引</param>
        void PlayAt(int index);

        /// <summary>
        /// 当前播放路径
        /// </summary>
        string? CurrentPath { get; }

        /// <summary>
        /// 播放模式
        /// </summary>
        PlayMode Mode { get; set; }

        /// <summary>
        /// 播放列表
        /// </summary>
        List<string> PlayPathList { get; set; }

        /// <summary>
        /// 播放指定路径的音乐
        /// </summary>
        /// <param name="path">音乐文件路径</param>
        void Play(string path);

        /// <summary>
        /// 加载上次播放的音乐
        /// </summary>
        /// <returns>上次播放的音乐路径</returns>
        string? LoadLastPlayed();

        /// <summary>
        /// 暂停播放
        /// </summary>
        void Pause();

        /// <summary>
        /// 播放下一首音乐
        /// </summary>
        void PlayNext();

        /// <summary>
        /// 播放上一首音乐
        /// </summary>
        void PlayPrevious();

        /// <summary>
        /// 获取音量
        /// </summary>
        /// <returns>音量值</returns>
        int GetVolume();

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="value">音量值</param>
        void SetVolume(int value);
    }
}
