# ProxyFramework
一个代理框架，可以自定义篡改收到的或者请求的数据。基于Titanium.Web.Proxy。

## 特点
1. 自动信任HTTPS证书，同时提供手机证书下载，在手机上面访问【ip:6066/ssl】进入下载页面下载证书并安装。  
2. 使用注入的方式进行代理添加和移除。  

## 存在的问题
1. 目前来看性能不是很好，只能说勉强够用。后续继续调优...

## How to start
下载并引入ProxyFramework，编写代理类（以修改Baidu首页为例）：
``````csharp
class BaiduProxy : IProxyFramework
{
    public override async Task<bool> Rule(SessionEventArgs e)
    {
        string url = e.HttpClient.Request.Url.ToString().ToLower();
        if (url.Contains("baidu.com"))  //匹配到qxclub.cn
        {
            return true;
        }
        return false;
    }

    public override async Task Response()
    {
        EventArgs.SetResponseBodyString("想百度一下？没门！！");
    }
}
``````
集成接口IProxyFramework，并重写Rule和选择重写Response或Request方法。如上面代码所示，重写Response后，使用EventArgs中的SetResponseBodyString方法将网页修改为"想百度一下？没门！！"。  
然后在Main方法中启动代理。
```csharp
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
```
然后在浏览器中输入baidu.com，查看效果吧~


