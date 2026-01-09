// Startup.cs
using ImageMusicPlayer.DataAccess;
using ImageMusicPlayer.Interfaces;
using ImageMusicPlayer.Models;
using ImageMusicPlayer.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace ImageMusicPlayer
{
    public static class Startup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 注册业务逻辑层的服务
            services.AddSingleton<IFavoriteManager, FavoriteManager>();

            services.AddSingleton<IRepository<List<FavoriteItem>>>(sp => new JsonRepository<List<FavoriteItem>>(Path.Combine(Application.StartupPath, "favorites.json")));
    

            // 根据需要选择 ImageManager 的某一个版本，可考虑使用配置或策略模式切换
            services.AddSingleton<IImageManager, ImageManager>();
            services.AddSingleton<IMusicManager, MusicManager>();
            services.AddSingleton<IMusicPlayer, MusicPlayer>();
            services.AddSingleton<IMusicLibraryManager, MusicLibraryManager>();
            services.AddSingleton<IPlaylistManager, PlaylistManager>();
            services.AddSingleton<MusicFolderManager>(); // 若需要直接使用

            // 注册 UI 层
            services.AddSingleton<MainForm>();
            services.AddSingleton<MusicManagerForm>();
            services.AddSingleton<FavoriteManagerForm>();

            // 如果需要全局日志、配置等服务，也可在此注册

            return services.BuildServiceProvider();
        }
    }
}
