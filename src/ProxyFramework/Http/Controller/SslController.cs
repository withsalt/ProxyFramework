using NetworkSocket.Http;
using ProxyFramework.Logger;
using ProxyFramework.Utils;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProxyFramework.Http.Controller
{
    class SslController : HttpController
    {
        /// <summary>
        /// 认证首页，输入用户信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [Route("/{controller}")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Download()
        {
            try
            {
                if (!Directory.Exists("Temp"))
                {
                    Directory.CreateDirectory("Temp");
                }
                string cerPath = Path.Combine("Temp", "Titanium_Root_Certificate_Authority.cer");
                if (System.IO.File.Exists(cerPath))
                {
                    System.IO.File.Delete(cerPath);
                }
                if (!DataCertificate.ExportToCerFile("Titanium Root Certificate Authority", cerPath))
                {
                    return Content("错误，证书文件导出失败！");
                }
                if (!System.IO.File.Exists(cerPath))
                {
                    return Content("错误，证书文件不存在！");
                }
                return new FileResult()
                {
                    FileName = cerPath,
                    ContentType = "application/x-x509-ca-cert",
                    ContentDisposition = "attachment;filename=Titanium_Root_Certificate_Authority.cer"
                };
            }
            catch (Exception ex)
            {
                Log.Error($"证书导出失败！错误：{ex.Message}", ex);
                return Content($"错误，证书导出失败！{ex.Message}");
            }
        }

        /// <summary>
        /// 加载视图
        /// </summary>
        /// <returns></returns>
        public ActionResult View()
        {
            try
            {
                HttpAction aciton = this.CurrentContext.Action;
                string file = Path.Combine("Http", "Pages", "Index.html");
                string html = System.IO.File.ReadAllText(file, Encoding.UTF8);
                return Content(html);
            }
            catch (Exception ex)
            {
                string error = $"Create view error. {ex.Message}";
                Log.Error(error, ex);
                return Content("Error page.");
            }
        }
    }
}
