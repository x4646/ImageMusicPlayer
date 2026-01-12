using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMusicPlayer.Models
{
    // 自定义优先级枚举
    public enum ImageLoadPriority
    {
        Immediate,  // 同步加载（当前可视项）
        High,       // 优先加载（预加载项）
        Normal,     // 常规加载
        Background  // 空闲时加载
    }

}
