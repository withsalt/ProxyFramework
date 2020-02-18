using Demo.QXClub;
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
            proxy.Add(new QXClubProxy());
            //启动
            proxy.Start();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q)
                {
                    Console.Write("确认退出代理工具吗？（y退出，任意键取消）");
                    key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Y)
                    {
                        proxy.Stop();
                        return;
                    }
                }
            }
        }
    }
}
