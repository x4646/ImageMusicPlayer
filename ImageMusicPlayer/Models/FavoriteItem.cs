namespace ImageMusicPlayer.Models
{
    public enum FavoriteType
    {
        Folder,
        SingleImage
    }

    public class FavoriteItem
    {
        public FavoriteType Type { get; set; }
        public string Path { get; set; }

        // 用于显示时的友好文本
        public string? DisplayText => Path;
    }
}
