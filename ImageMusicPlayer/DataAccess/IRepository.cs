using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMusicPlayer.DataAccess
{
    public interface IRepository<T>
    {
        T Load();
        void Save(T data);
    }
}
