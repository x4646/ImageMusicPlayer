using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMusicPlayer.Models
{
    public class CachedImage(string name, Image img) : IDisposable
    {
        public string Name { get; } = name;
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// 图片
        /// </summary>
        public Image Img { get; private set; } = img;

        public void Dispose()
        {
            if (!Disposed)
            {
                Console.WriteLine($"释放图片：{Name}");
                Disposed = true;


            }
        }

        public override string ToString() => Name;
    }
}
