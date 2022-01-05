using Demo.Baidu;
using Demo.ZMYY;
using ProxyFramework;
using System;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("注意！注意！注意！\n务必按q退出，否者可能导致系统无法正常上网！");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            //初始化
            Proxy proxy = new Proxy(9527, true, true, false);
            //注入
            proxy.Add(new ZMYYProxy());
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
