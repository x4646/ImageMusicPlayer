using Microsoft.Extensions.DependencyInjection;

namespace ImageMusicPlayer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 配置 DI 容器
            IServiceProvider serviceProvider = Startup.ConfigureServices();
            // 获取 MainForm 实例并运行
            MainForm mainForm = serviceProvider.GetRequiredService<MainForm>();
            
            Application.Run(mainForm);
        }
    }
}