using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using ProxyFramework.Http;
using ProxyFramework.Models;
using ProxyFramework.Utils;

namespace ProxyFramework
{
    public class Proxy
    {
        private readonly ProxyServer proxyServer = new ProxyServer();
        private ExplicitProxyEndPoint explicitEndPoint = null;
        private readonly HttpService httpService = new HttpService();
        private readonly uint _port = 9527;
        private readonly uint _httpPort = 6066;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <param name="isEnableHttp">是否启动HTTP服务器用于证书下载</param>
        /// <param name="isEnableSystemProxy">是否输出日志</param>
        /// <param name="isEnableLog">是否自动启用系统代理</param>
        public Proxy(uint port = 9527, bool isEnableHttp = true, bool isEnableSystemProxy = true, bool isEnableLog = true)
        {
            this._port = port;
            this.IsEnableHttp = isEnableHttp;
            this.IsEnableSystemProxy = isEnableSystemProxy;
            this.IsEnableLog = isEnableLog;
        }

        /// <summary>
        /// 是否启动HTTP服务器用于证书下载
        /// </summary>
        private bool IsEnableHttp { get; set; } = true;

        /// <summary>
        /// 是否输出日志
        /// </summary>
        private bool IsEnableLog { get; set; } = true;

        /// <summary>
        /// 是否自动启用系统代理
        /// </summary>
        private bool IsEnableSystemProxy { get; set; } = false;

        /// <summary>
        /// 添加代理
        /// </summary>
        /// <param name="proxy"></param>
        public void Add(IProxyFramework proxy)
        {
            if (proxy == null)
            {
                throw new Exception("Proxy can not null.");
            }
            Container.Add(proxy);
        }

        /// <summary>
        /// 移除代理
        /// </summary>
        /// <param name="proxy"></param>
        public void Remove(IProxyFramework proxy)
        {
            if (proxy == null)
            {
                throw new Exception("Proxy can not null.");
            }
            Container.Remove(proxy);
        }

        /// <summary>
        /// 当前代理信息
        /// </summary>
        /// <returns></returns>
        public ProxyInfo Info()
        {
            ProxyInfo info = new ProxyInfo()
            {
                IpAddress = new List<string>(),
                Port = _port,
                HttpPort = _httpPort,
                State = proxyServer.ProxyRunning,
            };
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName()); //取得本机IP
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].ToString().IndexOf('.') > 0)
                {
                    info.IpAddress.Add(ipEntry.AddressList[i].ToString());
                }
            }
            //Log.Info(ipListBuilder.ToString());
            //Log.Info($"请先修改WIFI代理。点击手机连接的WIFI，设置代理主机为电脑IP地址，端口为{_port}。");
            return info;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                if (IsEnableHttp)
                {
                    if (!httpService.Start(_httpPort))
                    {
                        Log.Error("Http服务启动失败。");
                        return false;
                    }
                }
                if (!VerCertInfo())
                {
                    //软件信息改变后，先移除存在的证书
                    DataCertificate.RemoveCertByName("Titanium Root Certificate Authority");
                }

                proxyServer.BeforeRequest += OnRequest;
                proxyServer.BeforeResponse += OnResponse;
                proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
                proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;

                proxyServer.CertificateManager.CreateRootCertificate();
                proxyServer.CertificateManager.TrustRootCertificate();
                proxyServer.EnableConnectionPool = true;

                explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, (int)_port, true);
                // Fired when a CONNECT request is received
                //explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

                // An explicit endpoint is where the client knows about the existence of a proxy
                // So client sends request in a proxy friendly manner
                proxyServer.AddEndPoint(explicitEndPoint);
                proxyServer.Start();

                if (IsEnableSystemProxy)
                {
                    proxyServer.SetAsSystemProxy(explicitEndPoint,ProxyProtocolType.AllHttp);
                    Log.Info("系统代理已启动。");
                }

                while (!proxyServer.ProxyRunning)
                {
                    Thread.Sleep(10);
                }
                Log.Info($"远程代理已启动。请将远程设备http/https代理设置为：【当前设备IP:{_port}】");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"服务启动失败，错误：{ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (!httpService.Stop())
            {
                Log.Error("Http服务停止失败。");
            }
            if (explicitEndPoint != null)
            {
                //explicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
            }
            if (proxyServer != null)
            {
                proxyServer.BeforeRequest -= OnRequest;
                proxyServer.BeforeResponse -= OnResponse;
                proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;
                proxyServer.Stop();
            }
            return true;
        }

        #region private

        /// <summary>
        /// 验证程序运行环境是否改变
        /// </summary>
        /// <returns></returns>
        private bool VerCertInfo()
        {
            string cpuId = HardwareManagement.GetCPUID();
            string file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "proxy.info");
            string verStr = cpuId + ";" + file;
            if (!File.Exists(file))
            {
                File.WriteAllText(file, Md5(verStr));
                return false;
            }
            string oldStr = File.ReadAllText(file);
            if (string.IsNullOrEmpty(oldStr))
            {
                File.WriteAllText(file, Md5(verStr));
                return false;
            }
            if (Md5(verStr) != oldStr)
            {
                File.WriteAllText(file, Md5(verStr));
                return false;
            }
            return true;
        }

        private string Md5(string value)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(value)) return result;
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sBuilder = new StringBuilder();
                foreach (byte t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                result = sBuilder.ToString();
            }
            return result;
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (IsEnableLog)
            {
                string logStr = $"\n==================================================\nType:Request\nURL:{e.HttpClient.Request.Url}\nHeader:{e.HttpClient.Request.HeaderText}\nBody:{await e.GetResponseBodyAsString()}\n==================================================";
                Log.Info(logStr);
            }

            if (Container.Count == 0)
                return;

            List<IProxyFramework> proxies = Container.Get();
            foreach (var item in proxies)
            {
                item.EventArgs = e;
                if (await item.Action(e))
                {
                    await item.Request();
                }
            }
        }

        // Modify response
        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            if (IsEnableLog)
            {
                string logStr = $"\n==================================================\nType:Response\nURL:{e.HttpClient.Request.Url}\nHeader:{e.HttpClient.Request.HeaderText}\nBody:{await e.GetResponseBodyAsString()}\n==================================================";
                Log.Info(logStr);
            }

            if (Container.Count == 0)
                return;

            List<IProxyFramework> proxies = Container.Get();
            foreach (var item in proxies)
            {
                item.EventArgs = e;
                if (await item.Action(e))
                {
                    await item.Response();
                }
            }
        }

        // Allows overriding default certificate validation logic
        private Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.CompletedTask;
        }

        // Allows overriding default client certificate selection logic during mutual authentication
        private Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            // set e.clientCertificate to override
            return Task.CompletedTask;
        }

        #endregion

    }
}
