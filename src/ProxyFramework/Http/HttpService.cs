using NetworkSocket;
using NetworkSocket.Http;
using ProxyFramework.Http.Filter;
using ProxyFramework.Logger;
using System;
using System.Net;
using System.Threading;

namespace ProxyFramework.Http
{
    public class HttpService
    {
        TcpListener _listener = null;

        public bool State
        {
            get
            {
                if (_listener == null)
                {
                    return false;
                }
                return _listener.IsListening;
            }
        }

        public bool Start()
        {
            try
            {
                _listener = new TcpListener();
                _listener.Use<HttpMiddleware>().GlobalFilters.Add(new HttpGlobalFilter());
                _listener.Start(new IPEndPoint(IPAddress.Any, 9528), 2048);

                while (!_listener.IsListening)
                {
                    Thread.Sleep(10);
                }
                Log.Info($"HTTP服务已启动，可通过访问[IP地址:9528/ssl]下载证书。");
                return true;
            }
            catch (Exception ex)
            {
                Log.Info($"Http service started failed. Error is {ex.Message}", ex);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                _listener.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Http service stop failed. Error is {ex.Message}", ex);
                return false;
            }
        }
    }
}
