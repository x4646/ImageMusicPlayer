using ImageMusicPlayer.Models;

public class ImageCache
{
    // 数据源：外部图片集合（比如从磁盘加载后已存在内存中）
    private IList<string> sourceCollection;
    // 缓存管理：内部使用 LinkedList 保存缓存窗口内的图片
    private LinkedList<CachedImage> cache = new LinkedList<CachedImage>();
    // 当前显示的图片节点
    private LinkedListNode<CachedImage> current;
    // 配置的总缓存长度（例如15）
    private readonly int totalCacheSize;
    // 定义比例：当往下一张时，前方（即未来的图片）占3/4，后方占1/4；反之，往上一张时，后方占3/4，前方占1/4
    private int forwardRatio;  // 3/4 的数量
    private int backwardRatio; // 1/4 的数量

    // 委托，用于根据索引从 sourceCollection 中获取图片
    public Func<string, CachedImage> loadImageFunc;
    // 当前在数据源中的索引（对应 current 在集合中的位置）
    private int _currentIndex = 0;

    public int CurrentSourceIndex { private get; set; }

    // 当前缓存中的图片数量
    public int CurrentCachedSize => cache.Count;

    public ImageCache(int? totalCacheSize)
    {
        // 如果没有传入缓存大小，则使用默认值
        // 默认缓存大小为 15
        if (totalCacheSize == null || totalCacheSize < 15)
            totalCacheSize = 15;

        this.totalCacheSize = totalCacheSize.Value;
    }

    public void Init(IList<string> sourceCollection)
    {
        Clear();
        // 初始化：假设初始显示第一张图片，加载从 index=0 到 index=forwardRatio-1 的图片
        _currentIndex = 0;

        this.sourceCollection = sourceCollection;

        // 例如总缓存15，往下一张时前方数量 = 15*3/4 = 11(向上取整)，后方数量 = 4
        forwardRatio = (int)Math.Ceiling(totalCacheSize * 0.75);
        backwardRatio = totalCacheSize - forwardRatio;


        for (int i = 0; i < forwardRatio && i < sourceCollection.Count; i++)
        {
            string path = sourceCollection[i];
            cache.AddLast(loadImageFunc(path));
        }
        current = cache.First;
    }

    private string GetImagePath(int index)
    {
        if (index < 0 || index >= sourceCollection.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "索引超出范围。");
        return sourceCollection[index];
    }

    // 获取当前图片
    public CachedImage GetCurrent() => current.Value;

    public void Clear()
    {

        _currentIndex = 0;
        foreach (var img in cache)
        {
            img.Dispose();
        }
    }

    // 调用 Next()，往前读取下一张图片
    public CachedImage Next()
    {
        Console.WriteLine("读取下一张图片...");
        // 更新数据源索引
        if (_currentIndex < sourceCollection.Count - 1)
        {
            _currentIndex++;
        }
        else
        {
            _currentIndex = 0;
        }


        // 如果当前缓存中有下一节点，则移动，否则尝试从数据源加载
        if (current.Next != null)
        {
            current = current.Next;
        }
        else
        {
            if (CurrentSourceIndex < sourceCollection.Count - 1)
            {
                // 当前处于缓存末尾，从数据源加载下一张图片
                var img = loadImageFunc(GetImagePath(_currentIndex));
                cache.AddLast(img);
                current = current.Next;
            }
        }


        // 调整缓存窗口：在“往下一张”模式下，期望后方（未来图片）数量为 forwardRatio（包含当前），前方（已读图片）数量为 backwardRatio
        // 保证当前节点到末尾不少于 forwardRatio
        while (CountFromCurrent() < forwardRatio && _currentIndex < sourceCollection.Count - 1)
        {
            _currentIndex++;
            cache.AddLast(loadImageFunc(GetImagePath(_currentIndex)));
        }
        // 如果当前节点之前超过 backwardRatio，则从头部删除多余图片并释放资源
        while (CountBeforeCurrent() > backwardRatio)
        {
            var first = cache.First;
            first.Value.Dispose();
            cache.RemoveFirst();
        }
        return current.Value;

    }

    // 调用 Previous()，往后读取上一张图片
    public CachedImage Previous()
    {
        Console.WriteLine("读取上一张图片...");
        if (current.Previous != null)
        {
            current = current.Previous;
            _currentIndex = Math.Max(_currentIndex - 1, 0);
        }
        else
        {
            // 如果已经在缓存最前端，但数据源中仍有更早的图片，可以加载新的图片到头部
            if (_currentIndex > 0)
            {
                _currentIndex--;
                cache.AddFirst(loadImageFunc(GetImagePath(_currentIndex)));
                current = cache.First;
            }
            else
            {
                Console.WriteLine("已经是第一张图片。");
            }
        }

        if (CurrentSourceIndex > 0)
        {

            // 在“往上一张”模式下，期望前方（已读图片）数量为 forwardRatio（包含当前），后方数量为 backwardRatio
            while (CountBeforeCurrentIncludingCurrent() < forwardRatio && _currentIndex > 0)
            {
                _currentIndex--;
                cache.AddFirst(loadImageFunc(GetImagePath(_currentIndex)));
            }
            while (CountAfterCurrent() > backwardRatio)
            {
                var last = cache.Last;
                last.Value.Dispose();
                cache.RemoveLast();
            }
        }
        return current.Value;
    }

    // 计算当前节点之前的节点数（不包括当前）
    private int CountBeforeCurrent()
    {
        int count = 0;
        var node = cache.First;
        while (node != current)
        {
            count++;
            node = node.Next;
        }
        return count;
    }

    // 包括当前
    private int CountBeforeCurrentIncludingCurrent() => CountBeforeCurrent() + 1;

    // 计算从当前节点到末尾的数量（包括当前）
    private int CountFromCurrent()
    {
        int count = 0;
        var node = current;
        while (node != null)
        {
            count++;
            node = node.Next;
        }
        return count;
    }

    // 计算当前节点之后的数量（不包括当前）
    private int CountAfterCurrent()
    {
        int count = 0;
        var node = current.Next;
        while (node != null)
        {
            count++;
            node = node.Next;
        }
        return count;
    }

    // 用于调试，打印当前缓存状态
    public void PrintCacheState()
    {
        Console.WriteLine("当前缓存状态：");
        foreach (var img in cache)
        {
            if (img == current.Value)
                Console.Write($"[当前: {img.Name}] ");
            else
                Console.Write($"{img.Name} ");
        }
        Console.WriteLine();
    }
}
