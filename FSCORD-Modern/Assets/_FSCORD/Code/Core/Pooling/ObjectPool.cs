using System;
using System.Collections.Generic;

namespace FSCORD.Core
{
    /// <summary>
    /// Generic, allocation-free object pool for plain C# objects. For pooled
    /// GameObjects use PoolService instead.
    /// </summary>
    public sealed class ObjectPool<T> where T : class
    {
        readonly Func<T> _factory;
        readonly Action<T> _onGet;
        readonly Action<T> _onReturn;
        readonly Stack<T> _idle = new();

        public int CountInactive => _idle.Count;

        public ObjectPool(Func<T> factory, Action<T> onGet = null, Action<T> onReturn = null, int prewarm = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet;
            _onReturn = onReturn;
            for (int i = 0; i < prewarm; i++) _idle.Push(_factory());
        }

        public T Get()
        {
            var item = _idle.Count > 0 ? _idle.Pop() : _factory();
            _onGet?.Invoke(item);
            return item;
        }

        public void Return(T item)
        {
            if (item == null) return;
            _onReturn?.Invoke(item);
            _idle.Push(item);
        }
    }
}
