using System;
using System.Threading.Tasks;

namespace ImageMusicPlayer.Interfaces
{
    public interface IImageManager
    {
        void Init(ImageViewer viewer, Action updateStatus, int cacheSize);
        /// <summary>
        /// 获取当前显示图片的完整路径。
        /// </summary>
        string CurrentImagePath { get; }

        /// <summary>
        /// 获取当前图片在集合中的索引位置。
        /// </summary>
        int CurrentIndex { get; }

        /// <summary>
        /// 获取当前图片集合中的总图片数。
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// 获取当前缓存中的图片数量。
        /// </summary>
        int CacheCount { get; }

        /// <summary>
        /// 弹出文件夹选择窗口并异步加载该文件夹中的所有图片。
        /// </summary>
        Task SelectAndLoadFromFolderAsync();

        /// <summary>
        /// 从指定文件夹路径异步加载所有图片。
        /// </summary>
        /// <param name="folderPath">要加载图片的文件夹路径</param>
        Task LoadFromFolderAsync(string folderPath);

        /// <summary>
        /// 弹出文件选择对话框，并将用户选择的图片追加到当前图片集合中。
        /// </summary>
        Task SelectAndAppendImagesAsync();

        /// <summary>
        /// 将指定的图片文件路径数组追加到当前图片集合中。
        /// </summary>
        /// <param name="files">要追加的图片文件路径数组</param>
        Task SelectAndAppendImagesAsync(string[] files);

        /// <summary>
        /// 切换到下一张图片。
        /// </summary>
        void NextImage();

        /// <summary>
        /// 切换到上一张图片。
        /// </summary>
        void PreviousImage();

        /// <summary>
        /// 从指定文件夹中移除该文件夹下的所有已加载图片。
        /// </summary>
        /// <param name="folderPath">图片所在文件夹路径</param>
        Task RemoveImagesFromFolder(string folderPath);

        /// <summary>
        /// 从当前图片集合中移除指定路径的图片。
        /// </summary>
        /// <param name="imagePath">要移除的图片文件路径</param>
        Task RemoveImage(string imagePath);

        /// <summary>
        /// 删除当前显示的图片文件，并从集合中移除。
        /// </summary>
        Task DeleteCurrentImageAsync();

        /// <summary>
        /// 启动幻灯片播放模式，根据提供的延迟函数自动播放图片。
        /// </summary>
        /// <param name="getDelay">返回每张图片显示间隔（毫秒）的委托</param>
        Task StartSlideShowAsync(Func<int>? getDelay);

        /// <summary>
        /// 暂停幻灯片播放。
        /// </summary>
        void PauseSlideShow();

    }
}