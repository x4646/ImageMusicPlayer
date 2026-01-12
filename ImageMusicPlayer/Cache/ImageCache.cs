using ImageMusicPlayer.Models;

/// <summary>
/// 图片缓存管理器，用于实现滑动窗口式的图片缓存机制。
/// 通过 LinkedList 维护当前显示图片附近的缓存，支持向前和向后浏览图片时的动态加载和释放。
/// </summary>
/// <remarks>
/// 该类实现了智能缓存策略：
/// - 向前浏览时，前方缓存占 3/4，后方缓存占 1/4
/// - 向后浏览时，后方缓存占 3/4，前方缓存占 1/4
/// - 自动管理内存，及时释放不需要的图片资源
/// </remarks>
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

    /// <summary>
    /// 获取或设置当前数据源索引
    /// </summary>
    public int CurrentSourceIndex { private get; set; }

    /// <summary>
    /// 获取当前缓存中的图片数量
    /// </summary>
    public int CurrentCachedSize => cache.Count;

    /// <summary>
    /// 初始化 ImageCache 类的新实例
    /// </summary>
    /// <param name="totalCacheSize">缓存大小，如果为 null 或小于 15，则使用默认值 15</param>
    public ImageCache(int? totalCacheSize)
    {
        // 如果没有传入缓存大小，则使用默认值
        // 默认缓存大小为 15
        if (totalCacheSize == null || totalCacheSize < 15)
            totalCacheSize = 15;

        this.totalCacheSize = totalCacheSize.Value;
    }

    /// <summary>
    /// 初始化缓存，加载初始图片集合
    /// </summary>
    /// <param name="sourceCollection">图片路径集合</param>
    /// <remarks>
    /// 该方法会：
    /// 1. 清空现有缓存
    /// 2. 计算向前和向后缓存比例
    /// 3. 加载初始图片到缓存中
    /// 4. 设置当前图片为第一张
    /// </remarks>
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

    /// <summary>
    /// 根据索引获取图片路径
    /// </summary>
    /// <param name="index">图片索引</param>
    /// <returns>图片路径</returns>
    /// <exception cref="ArgumentOutOfRangeException">当索引超出范围时抛出</exception>
    private string GetImagePath(int index)
    {
        if (index < 0 || index >= sourceCollection.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "索引超出范围。");
        return sourceCollection[index];
    }

    /// <summary>
    /// 获取当前显示的图片
    /// </summary>
    /// <returns>当前缓存的图片对象</returns>
    public CachedImage GetCurrent() => current.Value;

    /// <summary>
    /// 清空缓存并释放所有图片资源
    /// </summary>
    /// <remarks>
    /// 该方法会：
    /// 1. 重置当前索引
    /// 2. 释放所有缓存图片的资源
    /// 3. 清空缓存列表
    /// </remarks>
    public void Clear()
    {

        _currentIndex = 0;
        foreach (var img in cache)
        {
            img.Dispose();
        }
    }

    /// <summary>
    /// 获取下一张图片，并更新缓存状态
    /// </summary>
    /// <returns>下一张图片对象</returns>
    /// <remarks>
    /// 该方法实现了智能缓存管理：
    /// 1. 更新当前索引（支持循环浏览）
    /// 2. 移动当前节点或加载新图片
    /// 3. 调整缓存窗口大小，保持合适的缓存比例
    /// 4. 及时释放不需要的图片资源
    /// </remarks>
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


        // 调整缓存窗口：在"往下一张"模式下，期望后方（未来图片）数量为 forwardRatio（包含当前），前方（已读图片）数量为 backwardRatio
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

    /// <summary>
    /// 获取上一张图片，并更新缓存状态
    /// </summary>
    /// <returns>上一张图片对象</returns>
    /// <remarks>
    /// 该方法实现了智能缓存管理：
    /// 1. 移动当前节点或加载新图片
    /// 2. 调整缓存窗口大小，保持合适的缓存比例
    /// 3. 及时释放不需要的图片资源
    /// 4. 支持向后浏览时的缓存优化
    /// </remarks>
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

            // 在"往上一张"模式下，期望前方（已读图片）数量为 forwardRatio（包含当前），后方数量为 backwardRatio
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

    /// <summary>
    /// 计算当前节点之前的节点数（不包括当前）
    /// </summary>
    /// <returns>当前节点之前的节点数量</returns>
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

    /// <summary>
    /// 计算当前节点及之前的节点数（包括当前）
    /// </summary>
    /// <returns>当前节点及之前的节点数量</returns>
    private int CountBeforeCurrentIncludingCurrent() => CountBeforeCurrent() + 1;

    /// <summary>
    /// 计算从当前节点到末尾的数量（包括当前）
    /// </summary>
    /// <returns>从当前节点到末尾的节点数量</returns>
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

    /// <summary>
    /// 计算当前节点之后的数量（不包括当前）
    /// </summary>
    /// <returns>当前节点之后的节点数量</returns>
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

    /// <summary>
    /// 用于调试，打印当前缓存状态
    /// </summary>
    /// <remarks>
    /// 输出格式示例：[当前: image1.jpg] image2.jpg image3.jpg
    /// 其中当前显示的图片会用方括号标记
    /// </remarks>
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