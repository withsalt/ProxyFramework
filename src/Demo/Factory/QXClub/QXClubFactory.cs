using ProxyFramework.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace ProxyFramework.Factory.QXClub
{
    class QXClubFactory : IFactory
    {
        private static QXClubFactory _factory = null;
        private static readonly object _signLocker = new object();
        private QXClubFactory()
        {

        }

        public static QXClubFactory Instance
        {
            get
            {
                if (_factory == null)
                {
                    lock (_signLocker)
                    {
                        if (_factory == null)
                        {
                            _factory = new QXClubFactory();
                        }
                    }
                }
                return _factory;
            }
        }
        LimitProduectListProduect _limitProduectList = new LimitProduectListProduect();

        public async Task Excute(SessionEventArgs content, MethodType method)
        {
            if (method == MethodType.Response)
            {
                await DoResponse(content);
            }
            else
            {
                await DoRequest(content);
            }
        }

        private async Task DoResponse(SessionEventArgs content)
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
                await _limitProduectList.DoResponse(content);
            }
        }

        private async Task DoRequest(SessionEventArgs content)
        {
            
        }
    }
}
