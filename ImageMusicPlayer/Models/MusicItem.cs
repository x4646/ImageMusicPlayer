namespace ImageMusicPlayer.Models
{
    public class MusicItem
    {
        public string? FilePath { get; set; }
        public string? FolderPath { get; set; }
        public string? FileName => Path.GetFileName(FilePath);
        // 可选：添加更多信息，如艺术家、时长等
        public string? Artist { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
