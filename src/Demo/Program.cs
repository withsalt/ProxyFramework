using Demo.Baidu;
using ProxyFramework;
using System;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化
            Proxy proxy = new Proxy(9527);
            //注入
            proxy.Add(new BaiduProxy());
            //启动
            proxy.Start();

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    Console.Write("确认退出代理工具吗？（y退出，任意键取消）");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        proxy.Stop();
                        return;
                    }
                }
            }
        }
    }
}
