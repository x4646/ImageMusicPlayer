using System;
using System.Runtime.InteropServices;

namespace ImageMusicPlayer.Utils
{
    /// <summary>
    /// 屏幕常亮工具类：调用 Win32 API 阻止系统和显示器进入睡眠／锁屏状态。
    /// </summary>
    public static class ScreenHelper
    {
        // P/Invoke 声明
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        // 标志位枚举
        [Flags]
        private enum ExecutionState : uint
        {
            ES_CONTINUOUS = 0x80000000, // 一直有效，直到再次调用
            ES_SYSTEM_REQUIRED = 0x00000001, // 阻止系统休眠
            ES_DISPLAY_REQUIRED = 0x00000002, // 阻止显示器关闭／锁屏
        }

        /// <summary>
        /// 启用屏幕常亮：阻止系统和显示器进入睡眠／锁屏。
        /// </summary>
        public static void EnableKeepScreenOn()
        {
            SetThreadExecutionState(
                ExecutionState.ES_CONTINUOUS
              | ExecutionState.ES_SYSTEM_REQUIRED
              | ExecutionState.ES_DISPLAY_REQUIRED
            );
        }

        /// <summary>
        /// 恢复系统默认睡眠策略（取消常亮设置）。
        /// </summary>
        public static void DisableKeepScreenOn()
        {
            // 只保留 ES_CONTINUOUS，以恢复系统默认行为
            SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
        }

        /// <summary>
        /// 返回一个 IDisposable，在其生存期内保持屏幕常亮，Dispose 时会自动恢复。
        /// </summary>
        public static IDisposable KeepAwakeScope() => new SleepLock();

        // 内部 IDisposable 实现
        private sealed class SleepLock : IDisposable
        {
            private bool _disposed;

            public SleepLock()
            {
                EnableKeepScreenOn();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    DisableKeepScreenOn();
                    _disposed = true;
                }
            }
        }
    }
}
