﻿using ProxyFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Demo.Baidu
{
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
}