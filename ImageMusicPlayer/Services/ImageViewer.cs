using ImageMusicPlayer.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ImageMusicPlayer
{
    public class ImageViewer : Panel , IImageViewer
    {
        // 当前显示的图片对象
        private Image? image;
        // 当前缩放比例
        private float currentZoom = 1.0f;
        // 目标缩放比例（用于平滑动画过渡）
        private float targetZoom = 1.0f;
        // 最小缩放比例
        private const float minZoom = 0.1f;
        // 最大缩放比例
        private const float maxZoom = 5.0f;
        // 缩放动画定时器
        private Timer zoomTimer;
        // 是否正在拖动图片
        private bool isDragging = false;
        // 上一次鼠标位置（用于拖动计算）
        private Point lastMousePosition;
        // 缩放时鼠标位置（用于以鼠标为中心缩放）
        private PointF lastMousePosForZoom;
        // 图片平移偏移量
        private PointF panOffset = new PointF(0, 0);
        // 上一次滚轮事件时间（用于节流）
        private DateTime lastWheelEvent = DateTime.MinValue;
        // 鼠标相对图片的X坐标（用于缩放中心计算）
        private float mouseXRelative;
        // 鼠标相对图片的Y坐标（用于缩放中心计算）
        private float mouseYRelative;

        // 当前滤镜类型
        public FilterType CurrentFilter { get; set; } = FilterType.None;

        /// <summary>
        /// 滤镜强度，范围 0～1，0 表示无滤镜效果，1 表示预设滤镜全效果
        /// </summary>
        public float FilterIntensity { get; set; } = 1f;

        public event EventHandler ZoomChanged;

        // 当前缩放比例只读属性
        public float Zoom => currentZoom;

        public ImageViewer()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.AutoScroll = false;

            zoomTimer = new Timer
            {
                Interval = 30 // 约33FPS
            };

            zoomTimer.Tick += ZoomTimer_Tick;

            // 注册鼠标事件（包含滚轮、拖动等）
            this.MouseWheel += ImageViewer_MouseWheel;
            this.MouseDown += ImageViewer_MouseDown;
            this.MouseMove += ImageViewer_MouseMove;
            this.MouseUp += ImageViewer_MouseUp;
            this.Resize += ImageViewer_Resize;
        }

        /// <summary>
        /// 加载新图片，并重置视图（缩放、平移）
        /// </summary>
        public void LoadImage(Image? img)
        {
            if (this.image != null)
            {
                this.image.Dispose();
            }

            if (!IsImageValid(img)) return;

            this.image = img.Clone() as Image;

            if (this.image == null || !IsImageValid(this.image))
            {
                ResetView();
                Invalidate();
                return;
            }
            // 加载时自动适应窗体
            FitToWindow();
            Invalidate();
        }

        /// <summary>
        /// 重置视图：将图片缩放至适应窗体并居中显示
        /// </summary>
        public void ResetView()
        {
            FitToWindow();
            Invalidate();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 计算适合窗口的缩放比例，并居中图片
        /// </summary>
        private void FitToWindow()
        {
            if (image == null || !IsImageValid(image))
            {
                currentZoom = 1.0f;
                targetZoom = 1.0f;
                panOffset = new PointF(0, 0);
                return;
            }

            float clientWidth = this.ClientSize.Width;
            float clientHeight = this.ClientSize.Height;
            if (clientWidth <= 0 || clientHeight <= 0)
                return;

            float imageWidth = image.Width;
            float imageHeight = image.Height;
            if (imageWidth <= 0 || imageHeight <= 0)
                return;

            float widthRatio = clientWidth / imageWidth;
            float heightRatio = clientHeight / imageHeight;
            float fitZoom = Math.Min(widthRatio, heightRatio);

            currentZoom = targetZoom = Math.Max(minZoom, Math.Min(maxZoom, fitZoom));
            CenterImage();
        }

        /// <summary>
        /// 计算居中显示的平移偏移量
        /// </summary>
        private void CenterImage()
        {
            if (image == null || !IsImageValid(image))
            {
                panOffset = new PointF(0, 0);
                return;
            }

            float imageWidth = image.Width * currentZoom;
            float imageHeight = image.Height * currentZoom;
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;

            float x = (this.ClientSize.Width - imageWidth) / 2.0f;
            float y = (this.ClientSize.Height - imageHeight) / 2.0f;
            x = Math.Max(0, x);
            y = Math.Max(0, y);
            panOffset = new PointF(x, y);
            RestrictPanOffset();
        }

        /// <summary>
        /// 限制平移偏移量，防止图片移出可视区域
        /// </summary>
        private void RestrictPanOffset()
        {
            if (image == null || !IsImageValid(image))
                return;

            float imageWidth = image.Width * currentZoom;
            float imageHeight = image.Height * currentZoom;
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;

            float minX, maxX, minY, maxY;
            if (imageWidth < this.ClientSize.Width)
            {
                minX = (this.ClientSize.Width - imageWidth) / 2.0f;
                maxX = minX;
            }
            else
            {
                minX = this.ClientSize.Width - imageWidth;
                maxX = 0;
            }
            if (imageHeight < this.ClientSize.Height)
            {
                minY = (this.ClientSize.Height - imageHeight) / 2.0f;
                maxY = minY;
            }
            else
            {
                minY = this.ClientSize.Height - imageHeight;
                maxY = 0;
            }
            panOffset.X = Math.Max(minX, Math.Min(maxX, panOffset.X));
            panOffset.Y = Math.Max(minY, Math.Min(maxY, panOffset.Y));
        }

        /// <summary>
        /// 缩放时根据鼠标位置调整平移偏移，使缩放中心保持在鼠标处
        /// </summary>
        private void UpdatePanOffsetForZoom()
        {
            if (image == null || !IsImageValid(image))
                return;
            panOffset.X = lastMousePosForZoom.X - mouseXRelative * currentZoom;
            panOffset.Y = lastMousePosForZoom.Y - mouseYRelative * currentZoom;
        }

        /// <summary>
        /// 重绘图片，包括缩放、平移和滤镜效果
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (image == null || !IsImageValid(image))
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // 交互时优先流畅度，停止后优先清晰度
            bool isInteracting = isDragging || zoomTimer.Enabled;
            if (isInteracting)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.CompositingQuality = CompositingQuality.HighSpeed;
            }
            else
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
            }

            // 构造平移和缩放的矩阵变换
            using (Matrix transform = new Matrix())
            {
                transform.Translate(panOffset.X, panOffset.Y);
                transform.Scale(currentZoom, currentZoom);
                g.Transform = transform;
            }

            // 绘制图片，支持滤镜
            if (CurrentFilter == FilterType.None)
            {
                g.DrawImage(image, new Point(0, 0));
            }
            else
            {
                ColorMatrix cm = GetColorMatrix();
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
                }
            }
        }

        /// <summary>
        /// 根据当前滤镜、亮度和对比度构造 ColorMatrix
        /// </summary>
        private ColorMatrix GetColorMatrix()
        {
            // Identity矩阵：无效果
            ColorMatrix identity = new ColorMatrix(new float[][]
            {
                new float[]{1, 0, 0, 0, 0},
                new float[]{0, 1, 0, 0, 0},
                new float[]{0, 0, 1, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{0, 0, 0, 0, 1}
            });
            ColorMatrix filterMatrix = identity;

            switch (CurrentFilter)
            {
                case FilterType.ColorBoost:
                    filterMatrix = MultiplyColorMatrix(GetContrastMatrix(1.2f), GetSaturationMatrix(1.5f));
                    break;
                case FilterType.Clarity:
                    filterMatrix = GetContrastMatrix(1.4f);
                    break;
                case FilterType.Vintage:
                    filterMatrix = GetVintageMatrix();
                    break;
                case FilterType.Vivid:
                    filterMatrix = MultiplyColorMatrix(GetContrastMatrix(1.3f), GetSaturationMatrix(1.7f));
                    break;
                case FilterType.Moody:
                    filterMatrix = GetMoodyMatrix();
                    break;
                case FilterType.Warm:
                    // 暖调：提高对比度与饱和度，并加入红黄色调
                    filterMatrix = MultiplyColorMatrix(GetContrastMatrix(1.1f), MultiplyColorMatrix(GetSaturationMatrix(1.2f), GetWarmMatrix()));
                    break;
                case FilterType.Cool:
                    // 冷调：提高对比度与饱和度，并加入蓝调
                    filterMatrix = MultiplyColorMatrix(GetContrastMatrix(1.1f), MultiplyColorMatrix(GetSaturationMatrix(1.2f), GetCoolMatrix()));
                    break;
                case FilterType.Soft:
                    // 柔和：降低对比度和饱和度，使图像更温柔
                    filterMatrix = MultiplyColorMatrix(GetContrastMatrix(0.9f), GetSaturationMatrix(0.8f));
                    break;
                case FilterType.Dramatic:
                    // 戏剧性：大幅提升对比度与饱和度，并略微降低亮度
                    filterMatrix = MultiplyColorMatrix(GetContrastMatrix(1.5f), MultiplyColorMatrix(GetSaturationMatrix(1.8f), GetDramaticMatrix()));
                    break;
                case FilterType.None:
                default:
                    filterMatrix = identity;
                    break;
            }
            // 线性插值：最终矩阵 = Identity*(1-Intensity) + filterMatrix*Intensity
            return InterpolateColorMatrix(identity, filterMatrix, FilterIntensity);
        }


        // 辅助方法：生成亮度调整矩阵
        private ColorMatrix GetBrightnessMatrix(float brightness)
        {
            return new ColorMatrix(new float[][]
            {
                new float[]{1, 0, 0, 0, 0},
                new float[]{0, 1, 0, 0, 0},
                new float[]{0, 0, 1, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{brightness, brightness, brightness, 0, 1}
            });
        }

        // 辅助方法：生成对比度调整矩阵
        private ColorMatrix GetContrastMatrix(float contrast)
        {
            float t = 0.5f * (1 - contrast);
            return new ColorMatrix(new float[][]
            {
                new float[]{contrast, 0, 0, 0, 0},
                new float[]{0, contrast, 0, 0, 0},
                new float[]{0, 0, contrast, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{t, t, t, 0, 1}
            });
        }

        // 辅助方法：生成饱和度调整矩阵
        private ColorMatrix GetSaturationMatrix(float saturation)
        {
            return new ColorMatrix(new float[][]
            {
                new float[]{0.213f + 0.787f * saturation, 0.213f - 0.213f * saturation, 0.213f - 0.213f * saturation, 0, 0},
                new float[]{0.715f - 0.715f * saturation, 0.715f + 0.285f * saturation, 0.715f - 0.715f * saturation, 0, 0},
                new float[]{0.072f - 0.072f * saturation, 0.072f - 0.072f * saturation, 0.072f + 0.928f * saturation, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{0, 0, 0, 0, 1}
            });
        }

        // 辅助方法：复古滤镜矩阵
        private ColorMatrix GetVintageMatrix()
        {
            // 这里采用较轻的复古色调（类似于淡褪的 sepia 效果）
            return new ColorMatrix(new float[][]
            {
                new float[]{0.9f, 0, 0, 0, 0},
                new float[]{0, 0.85f, 0, 0, 0},
                new float[]{0, 0, 0.7f, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{0.1f, 0.05f, -0.05f, 0, 1}
            });
        }

        // 辅助方法：阴沉滤镜矩阵
        private ColorMatrix GetMoodyMatrix()
        {
            // 降低亮度和饱和度，并加入轻微蓝调
            return new ColorMatrix(new float[][]
            {
                new float[]{0.8f, 0, 0, 0, 0},
                new float[]{0, 0.8f, 0, 0, 0},
                new float[]{0, 0, 0.8f, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{-0.1f, -0.1f, -0.1f, 0, 1}
            });
        }

        // 线性插值两个 5x5 的 ColorMatrix
        private ColorMatrix InterpolateColorMatrix(ColorMatrix identity, ColorMatrix filter, float intensity)
        {
            ColorMatrix result = new ColorMatrix();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    // Identity 的对角线值为1，其余为0
                    float idVal = (i == j) ? 1f : 0f;
                    result[i, j] = idVal * (1 - intensity) + filter[i, j] * intensity;
                }
            }
            return result;
        }

        // 获取暖调滤镜矩阵
        private ColorMatrix GetWarmMatrix()
        {
            return new ColorMatrix(new float[][]
            {
                new float[]{1, 0, 0, 0, 0},
                new float[]{0, 1, 0, 0, 0},
                new float[]{0, 0, 1, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{0.1f, 0.05f, -0.05f, 0, 1}
            });
        }

        // 获取冷调滤镜矩阵
        private ColorMatrix GetCoolMatrix()
        {
            return new ColorMatrix(new float[][]
            {
                new float[]{1, 0, 0, 0, 0},
                new float[]{0, 1, 0, 0, 0},
                new float[]{0, 0, 1, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{-0.05f, -0.05f, 0.1f, 0, 1}
            });
        }

        // 获取戏剧性滤镜矩阵（这里用于略微降低亮度）
        private ColorMatrix GetDramaticMatrix()
        {
            return new ColorMatrix(new float[][]
            {
                new float[]{1, 0, 0, 0, 0},
                new float[]{0, 1, 0, 0, 0},
                new float[]{0, 0, 1, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{-0.05f, -0.05f, -0.05f, 0, 1}
            });
        }

        // 辅助方法：矩阵相乘（5x5）
        private ColorMatrix MultiplyColorMatrix(ColorMatrix A, ColorMatrix B)
        {
            ColorMatrix result = new ColorMatrix();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < 5; k++)
                    {
                        sum += A[i, k] * B[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }

        /// <summary>
        /// 缩放动画定时器事件，平滑过渡currentZoom到targetZoom
        /// </summary>
        private void ZoomTimer_Tick(object? sender, EventArgs e)
        {
            float diff = targetZoom - currentZoom;
            if (Math.Abs(diff) < 0.01f)
            {
                currentZoom = targetZoom;
                zoomTimer.Stop();
            }
            else
            {
                currentZoom += diff * 0.5f;
            }
            UpdatePanOffsetForZoom();
            RestrictPanOffset();
            Invalidate();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 鼠标滚轮事件，支持以鼠标为中心缩放
        /// </summary>
        private void ImageViewer_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (image == null || !IsImageValid(image))
                return;

            // 节流滚轮事件，避免过于频繁触发
            if ((DateTime.Now - lastWheelEvent).TotalMilliseconds < 20)
                return;
            lastWheelEvent = DateTime.Now;

            // 记录鼠标位置以便以鼠标为中心进行缩放
            lastMousePosForZoom = e.Location;

            // 使用倍率型（指数）缩放，手感更一致：每个滚轮刻度约 10% 缩放
            float steps = e.Delta / 120f;
            float factor = (float)Math.Pow(1.1d, steps);

            // 以 targetZoom 为基准累计缩放，避免平滑缩放过程中手感“滞后”
            float baseZoom = targetZoom;
            targetZoom = Math.Max(minZoom, Math.Min(maxZoom, baseZoom * factor));
            mouseXRelative = (lastMousePosForZoom.X - panOffset.X) / currentZoom;
            mouseYRelative = (lastMousePosForZoom.Y - panOffset.Y) / currentZoom;
            if (!zoomTimer.Enabled)
                zoomTimer.Start();
        }

        /// <summary>
        /// 鼠标按下事件，支持拖动图片
        /// </summary>
        private void ImageViewer_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePosition = e.Location;
                this.Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// 鼠标移动事件，拖动图片时更新偏移
        /// </summary>
        private void ImageViewer_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                PointF delta = new PointF(e.X - lastMousePosition.X, e.Y - lastMousePosition.Y);
                panOffset = new PointF(panOffset.X + delta.X, panOffset.Y + delta.Y);
                lastMousePosition = e.Location;
                RestrictPanOffset();
                Invalidate();
            }
        }

        /// <summary>
        /// 鼠标释放事件，结束拖动
        /// </summary>
        private void ImageViewer_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 控件尺寸变化时自适应图片
        /// </summary>
        private void ImageViewer_Resize(object? sender, EventArgs e)
        {
            if (image != null && IsImageValid(image))
            {
                FitToWindow();
                Invalidate();
            }
        }

        /// <summary>
        /// 判断图片对象是否有效
        /// </summary>
        private bool IsImageValid(Image img)
        {
            if (img == null)
                return false;
            try
            {
                int width = img.Width;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (image != null)
                {
                    image.Dispose();
                }
                zoomTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}