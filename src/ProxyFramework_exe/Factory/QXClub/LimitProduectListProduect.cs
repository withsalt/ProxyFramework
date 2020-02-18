using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace ProxyFramework.Factory.QXClub
{
    class LimitProduectListProduect : IProduect
    {
        public async Task DoRequest(SessionEventArgs content)
        {
            
        }

        public async Task DoResponse(SessionEventArgs content)
        {
            byte[] bodyBytes = await content.GetResponseBody();
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
                body = body.Replace("\"statusCode\":8", "\"statusCode\":28");
            }
            content.SetResponseBodyString(body);
        }
    }
}
