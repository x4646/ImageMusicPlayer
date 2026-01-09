using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ImageMusicPlayer.Interfaces;

namespace ImageMusicPlayer.Services
{
    public class ImageManager : IImageManager
    {
        private ImageViewer viewer;
        private Action updateStatus;
        private List<string> imagePaths = [];
        private int imageIndex = 0;
        private Dictionary<int, Image> imageCache = [];
        //private readonly ImageCache imageCache;
        private CancellationTokenSource slideShowCTS;
        private DateTime lastSwitchTime = DateTime.MinValue; // 用于防抖
        private const int SwitchDebounceMs = 50; // 防抖间隔
        private int targetIndex = 0; // 目标索引，用于处理快速切换

        // 控制图片切换的并发安全
        private SemaphoreSlim switchSemaphore = new SemaphoreSlim(1, 1);
        // 用于保护 imagePaths 集合
        private readonly object imagePathsLock = new object();

        public ImageManager()
        {
        }


        public void Init(ImageViewer viewer, Action updateStatus, int cacheSize)
        {
            this.viewer = viewer;
            this.updateStatus = updateStatus;
            //imageCache = new ImageCache(cacheSize);
        }

        #region 图片加载

        public async Task SelectAndLoadFromFolderAsync()
        {
            using (FolderBrowserDialog dialog = new())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    await LoadFromFolderAsync(dialog.SelectedPath);
                }
            }
        }

        public async Task LoadFromFolderAsync(string? folderPath)
        {
            if (folderPath == null)
                throw new ArgumentNullException(nameof(folderPath));

            List<string> newFiles = [];

            await Task.Run(() =>
            {
                newFiles = [.. Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))];
            });

            lock (imagePathsLock)
            {
                // 刷新缓存，确保后续缓存索引与 imagePaths 对应
                ClearCache();
                var unique = new HashSet<string>(imagePaths, StringComparer.OrdinalIgnoreCase);
                for (int i = newFiles.Count - 1; i >= 0; i--)
                {
                    var file = newFiles[i];
                    if (unique.Add(file))
                        imagePaths.Insert(0, file);
                }
                imageIndex = 0;
            }

            await ShowImageAsync();
        }



        public async Task SelectAndAppendImagesAsync()
        {
            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "图片文件|*.jpg;*.jpeg;*.png";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    await Task.Run(() =>
                    {
                        lock (imagePathsLock)
                        {
                            var unique = new HashSet<string>(imagePaths, StringComparer.OrdinalIgnoreCase);
                            foreach (var file in dialog.FileNames)
                            {
                                if (unique.Add(file))
                                    imagePaths.Add(file);
                            }
                        }
                    });
                    lock (imagePathsLock)
                    {
                        if (imageIndex >= imagePaths.Count)
                            imageIndex = 0;
                    }
                    await ShowImageAsync();
                }
            }
        }

        public async Task SelectAndAppendImagesAsync(string?[] files)
        {
            await Task.Run(() =>
            {
                lock (imagePathsLock)
                {
                    var unique = new HashSet<string>(imagePaths, StringComparer.OrdinalIgnoreCase);
                    foreach (var file in files)
                    {
                        if (unique.Add(file))
                            imagePaths.Add(file);
                    }
                }
            });
            lock (imagePathsLock)
            {
                if (imageIndex >= imagePaths.Count)
                    imageIndex = 0;
            }
            await ShowImageAsync();
        }


        Task IImageManager.RemoveImage(string imagePath)
        {
            throw new NotImplementedException();
        }


        public async Task RemoveImagesFromFolder(string? folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                folderPath += Path.DirectorySeparatorChar;

            string? currentFile;
            lock (imagePathsLock)
            {
                currentFile = imagePaths.Count > imageIndex ? imagePaths[imageIndex] : null;
                imagePaths.RemoveAll(file =>
                {
                    string dir = Path.GetDirectoryName(file);
                    if (string.IsNullOrEmpty(dir))
                        return false;
                    if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        dir += Path.DirectorySeparatorChar;
                    return dir.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase);
                });
            }
            ClearCache();
            lock (imagePathsLock)
            {
                if (imagePaths.Count == 0)
                {
                    imageIndex = 0;
                    updateStatus?.Invoke();
                    Debug.WriteLine($"从文件夹 {folderPath} 删除了图片。当前无图片。");
                    viewer.LoadImage(null);
                    return;
                }
                else
                {
                    int newIndex = imagePaths.FindIndex(f => f.Equals(currentFile, StringComparison.OrdinalIgnoreCase));
                    if (newIndex != -1)
                    {
                        imageIndex = newIndex;
                    }
                    else
                    {
                        if (imageIndex >= imagePaths.Count)
                            imageIndex = imagePaths.Count - 1;
                    }
                }
            }
            _ = ShowImageAsync();
            Debug.WriteLine($"从文件夹 {folderPath} 删除图片完成。");
            return;
        }

        public void RemoveImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                lock (imagePathsLock)
                {
                    imagePaths.RemoveAll(f => f.Equals(imagePath, StringComparison.OrdinalIgnoreCase));
                }
                ClearCache();
                lock (imagePathsLock)
                {
                    if (imagePaths.Count == 0)
                    {
                        imageIndex = 0;
                        updateStatus?.Invoke();
                        viewer.LoadImage(null);
                        Debug.WriteLine($"从图片列表中移除了图片：{imagePath}");
                        return;
                    }
                    else if (imageIndex >= imagePaths.Count)
                    {
                        imageIndex = imagePaths.Count - 1;
                    }
                }
                _ = ShowImageAsync();
                Debug.WriteLine($"从图片列表中移除了图片：{imagePath}");
            }
        }

        // 将删除当前图片方法改为异步
        public async Task DeleteCurrentImageAsync()
        {
            lock (imagePathsLock)
            {
                if (imagePaths.Count == 0 || imageIndex < 0 || imageIndex >= imagePaths.Count)
                {
                    Debug.WriteLine($"DeleteCurrentImage: 图片列表为空或索引无效，imageIndex={imageIndex}, imagePaths.Count={imagePaths.Count}");
                    return;
                }
                Debug.WriteLine($"DeleteCurrentImage: 从图片列表中移除，Path={imagePaths[imageIndex]}");
                imagePaths.RemoveAt(imageIndex);
            }
            ClearCache();
            lock (imagePathsLock)
            {
                if (imagePaths.Count == 0)
                {
                    imageIndex = 0;
                    updateStatus?.Invoke();
                    viewer.LoadImage(null);
                    Debug.WriteLine($"DeleteCurrentImage: 图片列表为空，imageIndex={imageIndex}");
                    return;
                }
                else if (imageIndex >= imagePaths.Count)
                {
                    imageIndex = imagePaths.Count - 1;
                }
            }
            await ShowImageAsync();
        }

        #endregion

        #region 图片显示与预加载

        private async Task ShowImageAsync()
        {
            string pathToLoad = null;
            lock (imagePathsLock)
            {
                if (imagePaths.Count == 0 || imageIndex < 0 || imageIndex >= imagePaths.Count)
                {
                    viewer.LoadImage(null);
                    updateStatus?.Invoke();
                    Debug.WriteLine($"ShowImageAsync: 图片列表为空或索引无效，imageIndex={imageIndex}, imagePaths.Count={imagePaths.Count}");
                    return;
                }
                pathToLoad = imagePaths[imageIndex];
            }
            var img = await LoadImageAsync(imageIndex);
            if (img == null)
            {
                Debug.WriteLine($"ShowImageAsync: 加载图片失败，imageIndex={imageIndex}, 尝试加载下一张");
                int attempts = imagePaths.Count;
                while (attempts-- > 0)
                {
                    lock (imagePathsLock)
                    {
                        imageIndex = (imageIndex + 1) % imagePaths.Count;
                        pathToLoad = imagePaths[imageIndex];
                    }
                    img = await LoadImageAsync(imageIndex);
                    if (img != null)
                        break;
                }
            }
            viewer.LoadImage(img);
            updateStatus?.Invoke();
            _ = PreloadAdjacentImagesAsync(imageIndex);
            Debug.WriteLine($"ShowImageAsync: 显示图片，imageIndex={imageIndex}, Path={(img != null ? pathToLoad : "null")}");
        }


        private async Task<Image?> LoadImageAsync(int index)
        {
            string? filePath = null;
            lock (imagePathsLock)
            {
                if (index < 0 || index >= imagePaths.Count)
                {
                    Debug.WriteLine($"LoadImageAsync: 索引无效，index={index}, imagePaths.Count={imagePaths.Count}");
                    return null;
                }
                filePath = imagePaths[index];
                if (imageCache.TryGetValue(index, out var cached) && IsImageValid(cached))
                {
                    Debug.WriteLine($"LoadImageAsync: 从缓存加载，index={index}, Path={filePath}");
                    return cached;
                }
                else imageCache.Remove(index);
            }

            Image? image = await Task.Run(() => LoadImageFromFile(filePath));
            if (image == null)
            {
                Debug.WriteLine($"LoadImageAsync: 无法加载图片: {filePath}，将其移除");
                lock (imagePathsLock)
                {
                    if (index < imagePaths.Count && imagePaths[index].Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        imagePaths.RemoveAt(index);
                        var newCache = new Dictionary<int, Image>();
                        foreach (var kvp in imageCache)
                        {
                            int oldIndex = kvp.Key;
                            if (oldIndex > index)
                                newCache[oldIndex - 1] = kvp.Value;
                            else if (oldIndex < index)
                                newCache[oldIndex] = kvp.Value;
                        }
                        imageCache.Clear();
                        foreach (var kvp in newCache)
                            imageCache[kvp.Key] = kvp.Value;

                        if (imagePaths.Count == 0)
                        {
                            imageIndex = 0;
                            Debug.WriteLine($"LoadImageAsync: 图片列表为空，imageIndex={imageIndex}");
                            return null;
                        }
                        if (index <= imageIndex)
                        {
                            imageIndex = Math.Max(0, imageIndex - 1);
                            Debug.WriteLine($"LoadImageAsync: 移除当前或前面的图片，调整 imageIndex={imageIndex}");
                        }
                        else
                        {
                            Debug.WriteLine($"LoadImageAsync: 移除后面的图片，imageIndex不变={imageIndex}");
                        }
                    }
                }
                return await LoadImageAsync(imageIndex);
            }
            lock (imageCache)
            {
                imageCache[index] = image;
                ManageCache(index);
            }
            Debug.WriteLine($"LoadImageAsync: 加载成功并加入缓存，index={index}, Path={filePath}");
            return image;
        }

        private bool IsImageValid(Image img)
        {
            if (img == null)
                return false;
            try
            {
                var width = img.Width;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"IsImageValid: 图片无效，错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 加载图片时进行下采样处理：如果图片尺寸超过预设阈值（如1920像素），则生成一个缩小版以提高显示性能
        /// </summary>
        private static Image? LoadImageFromFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        byte[] imageData = ms.ToArray();
                        using (MemoryStream msCopy = new MemoryStream(imageData))
                        {
                            Image originalImage = Image.FromStream(msCopy);
                            // 设定最大尺寸，例如 1920 像素（根据实际情况调整）
                            int maxDimension = 3048;
                            if (originalImage.Width > maxDimension || originalImage.Height > maxDimension)
                            {
                                float scale = Math.Min((float)maxDimension / originalImage.Width, (float)maxDimension / originalImage.Height);
                                int newWidth = (int)(originalImage.Width * scale);
                                int newHeight = (int)(originalImage.Height * scale);
                                Bitmap scaledImage = new Bitmap(newWidth, newHeight);
                                using (Graphics g = Graphics.FromImage(scaledImage))
                                {
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                                }
                                originalImage.Dispose();
                                return scaledImage;
                            }
                            else
                            {
                                // 如果图片尺寸不大，直接返回全分辨率 Bitmap
                                return new Bitmap(originalImage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载图片失败: {filePath}, 错误: {ex.Message}");
                return null;
            }
        }

        private void ClearCache()
        {
            lock (imageCache)
            {
                foreach (var img in imageCache.Values)
                {
                    img?.Dispose();
                }
                imageCache.Clear();
            }
        }

        private void ManageCache(int currentIndex)
        {
            lock (imageCache)
            {
                int preloadWindow = 160;
                var keysToRemove = imageCache.Keys.Where(i => Math.Abs(i - currentIndex) > preloadWindow).ToList();
                foreach (var key in keysToRemove)
                {
                    if (imageCache.TryGetValue(key, out var img))
                    {
                        img?.Dispose();
                        imageCache.Remove(key);
                    }
                }
            }
        }

        private async Task PreloadAdjacentImagesAsync(int currentIndex)
        {
            if (imagePaths.Count == 0 || currentIndex < 0 || currentIndex >= imagePaths.Count)
            {
                Debug.WriteLine($"PreloadAdjacentImagesAsync: 列表为空或索引无效，currentIndex={currentIndex}, imagePaths.Count={imagePaths.Count}");
                return;
            }

            int previousIndex = (currentIndex - 1 + imagePaths.Count) % imagePaths.Count;
            int nextIndex = (currentIndex + 1) % imagePaths.Count;
            int[] indices = new int[] { previousIndex, nextIndex };

            foreach (var i in indices)
            {
                if (i < 0 || i >= imagePaths.Count)
                {
                    Debug.WriteLine($"PreloadAdjacentImagesAsync: 索引 {i} 无效，跳过");
                    continue;
                }
                lock (imageCache)
                {
                    if (imageCache.TryGetValue(i, out var cached) && IsImageValid(cached))
                    {
                        Debug.WriteLine($"PreloadAdjacentImagesAsync: 索引 {i} 已缓存，跳过");
                        continue;
                    }
                }
                var img = await Task.Run(() => LoadImageFromFile(imagePaths[i]));
                if (img == null)
                {
                    Debug.WriteLine($"PreloadAdjacentImagesAsync: 预加载失败，索引 {i}，Path={imagePaths[i]}");
                    continue;
                }
                lock (imageCache)
                {
                    if (i >= 0 && i < imagePaths.Count && (!imageCache.ContainsKey(i) || !IsImageValid(imageCache[i])))
                    {
                        imageCache[i] = img;
                        Debug.WriteLine($"PreloadAdjacentImagesAsync: 预加载成功，索引 {i}, Path={imagePaths[i]}");
                    }
                }
            }
            ManageCache(currentIndex);
        }

        #endregion

        #region 图片浏览操作

        public void NextImage()
        {
            _ = NextImageAsync();
        }

        private async Task NextImageAsync()
        {
            // 如果已有加载在进行，直接忽略本次请求
            if (!switchSemaphore.Wait(0))
            {
                Debug.WriteLine("NextImage: 图片加载进行中，忽略本次切换请求");
                return;
            }
            try
            {
                if (imagePaths.Count == 0)
                {
                    Debug.WriteLine("NextImage: 图片列表为空");
                    return;
                }
                // 更新目标索引
                targetIndex = (targetIndex + 1) % imagePaths.Count;
                if ((DateTime.Now - lastSwitchTime).TotalMilliseconds < SwitchDebounceMs)
                {
                    Debug.WriteLine($"NextImage: 防抖中，targetIndex={targetIndex}");
                    return;
                }
                imageIndex = targetIndex;
                lastSwitchTime = DateTime.Now;
                // 只有加载完当前图片，才允许下一次切换
                await ShowImageAsync();
            }
            finally
            {
                switchSemaphore.Release();
            }
        }

        public void PreviousImage()
        {
            _ = PreviousImageAsync();
        }

        private async Task PreviousImageAsync()
        {
            if (!switchSemaphore.Wait(0))
            {
                Debug.WriteLine("PreviousImage: 图片加载进行中，忽略本次切换请求");
                return;
            }
            try
            {
                if (imagePaths.Count == 0)
                {
                    Debug.WriteLine("PreviousImage: 图片列表为空");
                    return;
                }
                targetIndex = (targetIndex - 1 + imagePaths.Count) % imagePaths.Count;
                if ((DateTime.Now - lastSwitchTime).TotalMilliseconds < SwitchDebounceMs)
                {
                    Debug.WriteLine($"PreviousImage: 防抖中，targetIndex={targetIndex}");
                    return;
                }
                imageIndex = targetIndex;
                lastSwitchTime = DateTime.Now;
                await ShowImageAsync();
            }
            finally
            {
                switchSemaphore.Release();
            }
        }

        public string CurrentImagePath
        {
            get
            {
                lock (imagePathsLock)
                {
                    return imagePaths.Count > 0 && imageIndex < imagePaths.Count ? imagePaths[imageIndex] : null;
                }
            }
        }
        public int CurrentIndex => imageIndex;
        public int TotalCount
        {
            get
            {
                lock (imagePathsLock)
                {
                    return imagePaths.Count;
                }
            }
        }
        public int CacheCount
        {
            get
            {
                lock (imageCache)
                {
                    return imageCache.Count;
                }
            }
        }

        #endregion

        #region 幻灯片播放

        /// <summary>
        /// 启动幻灯片播放，使用动态延时参数（通过委托获取最新延时值）
        /// </summary>
        public async Task StartSlideShowAsync(Func<int> getDelay)
        {
            PauseSlideShow();
            slideShowCTS = new CancellationTokenSource();
            CancellationToken token = slideShowCTS.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    NextImage();
                    int currentDelay = getDelay();
                    await Task.Delay(currentDelay, token);
                }
            }
            catch (TaskCanceledException)
            {
                // 忽略取消异常
            }
        }

        /// <summary>
        /// 暂停幻灯片播放
        /// </summary>
        public void PauseSlideShow()
        {
            if (slideShowCTS != null)
            {
                slideShowCTS.Cancel();
                slideShowCTS.Dispose();
                slideShowCTS = null;
            }
        }

        #endregion
    }
}
