using ProxyFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Demo.Baidu
{
    public class BaiduProxy : IProxyFramework
    {
        public override async Task<bool> Action(SessionEventArgs e)
        {
            string url = e.HttpClient.Request.Url;
            if (url.Contains("baidu.com", StringComparison.OrdinalIgnoreCase))  //匹配到baidu.com
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
}
