using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageMusicPlayer.DataAccess
{
    public class JsonRepository<T> : IRepository<T>
    {
        private readonly string filePath;
        public JsonRepository(string filePath)
        {
            this.filePath = filePath;
        }

        public T Load()
        {
            if (!File.Exists(filePath))
            {
                return default;
            }
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                // 此处也可扩展日志记录
                throw new Exception($"加载 {filePath} 时发生错误: {ex.Message}", ex);
            }
        }

        public void Save(T data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"保存 {filePath} 时发生错误: {ex.Message}", ex);
            }
        }
    }
}
