using System;
using System.Drawing;

public interface IImageViewer
{
    /// <summary>
    /// 获取或设置缩放比例
    /// </summary>
    float Zoom { get; }

    /// <summary>
    /// 加载图像
    /// </summary>
    /// <param name="img">要加载的图像</param>
    void LoadImage(Image? img);

    /// <summary>
    /// 重置视图
    /// </summary>
    void ResetView();

    /// <summary>
    /// 缩放比例改变事件
    /// </summary>
    event EventHandler ZoomChanged;
}
