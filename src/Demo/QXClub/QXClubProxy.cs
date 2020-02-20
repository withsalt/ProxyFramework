using ProxyFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Demo.QXClub
{
    public class QXClubProxy : IProxyFramework
    {
        public override async Task<bool> Rule(SessionEventArgs e)
        {
            string url = e.HttpClient.Request.Url.ToString().ToLower();
            if (url.Contains("qxclub.cn"))  //匹配到qxclub.cn
            {
                return true;
            }
            return false;
        }

        LimitProduectListProduect limitProduectList = new LimitProduectListProduect();

        public override async Task Response()
        {
            byte[] bodyBytes = await EventArgs.GetResponseBody();
            if (bodyBytes.Length <= 0)
            {
                return;
            }
            string body = Encoding.UTF8.GetString(bodyBytes);
            if (string.IsNullOrEmpty(body))
            {
                return;
            }
            if (body.Contains("\"statusCode\":8"))
            {
                await limitProduectList.DoResponse(EventArgs);
            }
        }
    }
}
