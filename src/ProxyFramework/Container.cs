using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyFramework
{
    class Container
    {
        private static List<IProxyFramework> _proxys = new List<IProxyFramework>();

        private static readonly object locker = new object();

        public static int Count
        {
            get
            {
                int length = 0;
                lock (locker)
                {
                    length = _proxys.Count;
                }
                return length;
            }
        }

        public static List<IProxyFramework> Get()
        {
            return _proxys;
        }

        public static void Add(IProxyFramework proxy)
        {
            if (_proxys.Contains(proxy))
            {
                return;
            }
            lock (locker)
            {
                _proxys.Add(proxy);
            }
        }

        public static void Remove(IProxyFramework proxy)
        {
            if (_proxys.Contains(proxy))
            {
                return;
            }
            lock (locker)
            {
                _proxys.Remove(proxy);
            }
        }

        public static void Clear()
        {
            lock (locker)
            {
                _proxys.Clear();
            }
        }
    }
}
