using ProxyFramework.Enum;
using ProxyFramework.Factory.QXClub;
using ProxyFramework.Http;
using ProxyFramework.Logger;
using ProxyFramework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace ProxyFramework
{
    class Program
    {
        private static readonly int _bindPort = 9527;

        private static readonly ProxyServer proxyServer = new ProxyServer();
        private static ExplicitProxyEndPoint explicitEndPoint = null;
        private static HttpService httpService = new HttpService();

        static void Init()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP
            Log.Info($"代理端口：{_bindPort}");
            StringBuilder ipListBuilder = new StringBuilder();
            ipListBuilder.Append("本机IP v4列表：");
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].ToString().IndexOf('.') > 0)
                {
                    if (i == ipEntry.AddressList.Length - 1)
                        ipListBuilder.Append(ipEntry.AddressList[i].ToString());
                    else
                        ipListBuilder.Append(ipEntry.AddressList[i].ToString() + "，");
                }
            }
            Log.Info(ipListBuilder.ToString());
            Log.Info($"请先修改WIFI代理。点击手机连接的WIFI，设置代理主机为电脑IP地址，端口为{_bindPort}。");

            if (RunTime.IsWindows)
            {
                // fix console hang due to QuickEdit mode
                ConsoleHelper.DisableQuickEditMode();
            }
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                Stop();
            };
        }

        private static void Start(params string[] args)
        {
            try
            {
                if (!httpService.Start())
                {
                    Log.Error("Http服务启动失败。");
                    return;
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

                explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, _bindPort, true);
                // Fired when a CONNECT request is received
                explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

                // An explicit endpoint is where the client knows about the existence of a proxy
                // So client sends request in a proxy friendly manner
                proxyServer.AddEndPoint(explicitEndPoint);
                proxyServer.Start();

                if (args.Length > 0)
                {
                    List<string> argsList = new List<string>(args);
                    if (argsList.Contains("-l") || argsList.Contains("-L"))
                    {
                        // Only explicit proxies can be set as system proxy!
                        proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
                        proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
                        Log.Info("系统代理已启动。");
                    }
                }

                while (!proxyServer.ProxyRunning)
                {
                    Thread.Sleep(10);
                }
                Log.Info("远程代理已启动。");

                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q)
                    {
                        Console.Write("确认退出代理工具吗？（y退出，任意键取消）");
                        key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Y)
                        {
                            Stop();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"服务启动失败，错误：{ex.Message}", ex);
            }
        }

        private static void Stop()
        {
            if (!httpService.Stop())
            {
                Log.Error("Http服务停止失败。");
            }
            if (explicitEndPoint != null)
            {
                explicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
            }
            if (proxyServer != null)
            {
                proxyServer.BeforeRequest -= OnRequest;
                proxyServer.BeforeResponse -= OnResponse;
                proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;
                proxyServer.Stop();
            }
        }

        /// <summary>
        /// 验证程序运行环境是否改变
        /// </summary>
        /// <returns></returns>
        private static bool VerCertInfo()
        {
            string cpuId = HardwareManagement.GetCPUID();
            string file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "info.txt");
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
            }
            return true;
        }

        private static string Md5(string value)
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

        private static async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;

            //if (hostname.Contains("baidu.com"))
            //{
            //    // Exclude Https addresses you don't want to proxy
            //    // Useful for clients that use certificate pinning
            //    // for example dropbox.com
            //    e.DecryptSsl = false;
            //}
        }

        private static async Task OnRequest(object sender, SessionEventArgs e)
        {
            string logStr = $"\n==================================================\nType:Request\nURL:{e.HttpClient.Request.Url}\nHeader:{e.HttpClient.Request.HeaderText}\nBody:{await e.GetResponseBodyAsString()}\n==================================================";
            Log.Info(logStr);
            //Console.WriteLine(logStr);

            //var method = e.HttpClient.Request.Method.ToUpper();
            //if ((method == "POST" || method == "PUT" || method == "PATCH"))
            //{
            //    // Get/Set request body bytes
            //    byte[] bodyBytes = await e.GetRequestBody();
            //    e.SetRequestBody(bodyBytes);

            //    // Get/Set request body as string
            //    string bodyString = await e.GetRequestBodyAsString();
            //    e.SetRequestBodyString(bodyString);

            //    // store request 
            //    // so that you can find it from response handler 
            //    e.UserData = e.HttpClient.Request;
            //}

            // To cancel a request with a custom HTML content
            // Filter URL
            //if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("google.com"))
            //{
            //    e.Ok("<!DOCTYPE html>" +
            //        "<html><body><h1>" +
            //        "Website Blocked" +
            //        "</h1>" +
            //        "<p>Blocked by titanium web proxy.</p>" +
            //        "</body>" +
            //        "</html>");
            //}
        }

        // Modify response
        private static async Task OnResponse(object sender, SessionEventArgs e)
        {
            // read response headers
            if (e.HttpClient.Request.RequestUri.Host.ToLower().Contains("wx.qxclub.cn"))
            {
                string logStr = $"\n==================================================\nType:Response\nURL:{e.HttpClient.Request.Url}\nHeader:{e.HttpClient.Request.HeaderText}\nBody:{await e.GetResponseBodyAsString()}\n==================================================";
                Log.Info(logStr);

                await QXClubFactory.Instance.Excute(e, MethodType.Response);
            }
            else
            {
                return;
            }

            if (e.UserData != null)
            {
                // access request from UserData property where we stored it in RequestHandler
                var request = (Request)e.UserData;
            }
        }

        // Allows overriding default certificate validation logic
        private static Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.CompletedTask;
        }

        // Allows overriding default client certificate selection logic during mutual authentication
        private static Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            // set e.clientCertificate to override
            return Task.CompletedTask;
        }

        static void Main(string[] args)
        {
            Init();
            Start(args);

            Console.ReadKey(false);
        }
    }
}
