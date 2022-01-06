using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ProxyFramework
{
    class Container
    {
        private static ConcurrentDictionary<int, IProxyFramework> _proxys = new ConcurrentDictionary<int, IProxyFramework>();

        public static int Count
        {
            get
            {
                return _proxys.Count;
            }
        }

        public static ReadOnlyCollection<IProxyFramework> Get()
        {
            ReadOnlyCollection<IProxyFramework> proxyFrameworks = new ReadOnlyCollection<IProxyFramework>(_proxys.Values.ToList());
            return proxyFrameworks;
        }

        public static bool Add(IProxyFramework proxy)
        {
            if (_proxys.ContainsKey(proxy.GetHashCode()))
            {
                return true;
            }
            return _proxys.TryAdd(proxy.GetHashCode(), proxy);
        }

        public static bool Remove(IProxyFramework proxy)
        {
            if (!_proxys.ContainsKey(proxy.GetHashCode()))
            {
                return true;
            }
            return _proxys.TryRemove(proxy.GetHashCode(), out _);
        }

        public static void Clear()
        {
            _proxys.Clear();
        }
    }
}
