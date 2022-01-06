using Demo.Json;
using Demo.ZMYY.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using ProxyFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Demo.ZMYY
{
    public class ZMYYProxy : IProxyFramework
    {
        private readonly List<string> matchTitleList = new List<string>()
        {
            "九价人乳头瘤病毒疫苗","四价人乳头瘤病毒疫苗"
        };

        private readonly string _customerProductUrl = "https://cloud.cn2030.com/sc/wx/HandlerSubscribe.ashx?act=CustomerProduct";
        private readonly string _custSubscribeDateAll = "https://cloud.cn2030.com/sc/wx/HandlerSubscribe.ashx?act=GetCustSubscribeDateAll";
        private readonly string _custSubscribeDateDetail = "https://cloud.cn2030.com/sc/wx/HandlerSubscribe.ashx?act=GetCustSubscribeDateDetail";

        public override Task<bool> Action(SessionEventArgs e)
        {
            string url = e.HttpClient.Request.Url;
            if (url.Contains(_customerProductUrl, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(true);
            }
            if (url.Contains(_custSubscribeDateAll, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(true);
            }
            if (url.Contains(_custSubscribeDateDetail, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public override async Task Response()
        {
            try
            {
                string oldResponse = await EventArgs.GetResponseBodyAsString();
                if (string.IsNullOrWhiteSpace(oldResponse))
                {
                    return;
                }
                int index = oldResponse.IndexOf("{");
                int index0 = oldResponse.LastIndexOf("}");
                if (index != -1 && index0 != -1)
                {
                    if (index0 <= index)
                    {
                        return;
                    }
                    oldResponse = oldResponse.Substring(index, index0 + 1 - index);
                    if (string.IsNullOrWhiteSpace(oldResponse))
                    {
                        return;
                    }
                }
                string url = EventArgs.HttpClient.Request.Url;
                string result = string.Empty;
                if (url.Contains(_customerProductUrl, StringComparison.OrdinalIgnoreCase))
                {
                    //处理疫苗页面
                    result = ProcessCustomerProduct(oldResponse);
                }
                if (url.Contains(_custSubscribeDateAll, StringComparison.OrdinalIgnoreCase))
                {
                    //处理预约页面
                    result = ProcessCustSubscribeDate(oldResponse);
                }
                if (url.Contains(_custSubscribeDateDetail, StringComparison.OrdinalIgnoreCase))
                {
                    //处理疫苗剩余剂次
                    result = ProcessCustSubscribeDateDetail(oldResponse);
                }
                EventArgs.SetResponseBody(Encoding.UTF8.GetBytes(result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private string ProcessCustomerProduct(string oldResponse)
        {
            SubscribeResponseModel responseModel = JsonUtil.DeserializeStringToObject<SubscribeResponseModel>(oldResponse);
            if (responseModel.status != 200)
            {
                return oldResponse;
            }
            if (responseModel.list == null)
            {
                responseModel.list = new List<ListItemModel>();
                return JsonUtil.SerializeToString(responseModel);
            }
            if (!responseModel.list.Any())
            {
                return oldResponse;
            }
            foreach (var item in responseModel.list)
            {
                if (matchTitleList.Contains(item.text))
                {
                    item.BtnLable = "立即预约";
                    item.date = $"{DateTime.Now.ToString("MM-dd 00:00")} 至 {DateTime.Now.ToString("MM-dd 23:59")}";
                    item.enable = true;
                }
            }
            return JsonUtil.SerializeToString(responseModel);
        }

        private string ProcessCustSubscribeDate(string oldResponse)
        {
            CustSubscribeDateResponseModel responseModel = JsonUtil.DeserializeStringToObject<CustSubscribeDateResponseModel>(oldResponse);
            if (responseModel.status != 200)
            {
                return oldResponse;
            }
            if (responseModel.list == null)
            {
                responseModel.list = new List<SubscribeDateItemModel>();
                return JsonUtil.SerializeToString(responseModel);
            }
            if (!responseModel.list.Any())
            {
                return oldResponse;
            }
            foreach (var item in responseModel.list)
            {
                item.enable = true;
            }
            return JsonUtil.SerializeToString(responseModel);
        }

        private string ProcessCustSubscribeDateDetail(string oldResponse)
        {
            //需要先解密
            string decodeResponse = DecodeResponse(oldResponse, out string token);
            if (string.IsNullOrEmpty(decodeResponse))
            {
                return oldResponse;
            }
            CustSubscribeDateDetailResponseModel responseModel = JsonUtil.DeserializeStringToObject<CustSubscribeDateDetailResponseModel>(decodeResponse);
            if (responseModel.status != 200)
            {
                return oldResponse;
            }
            if (responseModel.list == null)
            {
                responseModel.list = new List<CustSubscribeDateDetailItem>();
                return JsonUtil.SerializeToString(responseModel);
            }
            if (!responseModel.list.Any())
            {
                return oldResponse;
            }
            foreach (var item in responseModel.list)
            {
                //设置剂次为100
                item.qty = 100;
            }
            return AesHelper.AesEncryptor(JsonUtil.SerializeToString(responseModel), token);
        }

        /// <summary>
        /// 解密返回的数据
        /// </summary>
        /// <param name="oldResponse"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string DecodeResponse(string oldResponse, out string token)
        {
            token = string.Empty;
            var cookies = EventArgs.HttpClient.Request.Headers.GetHeaders("Cookie");
            if (cookies == null || cookies.Count == 0)
            {
                return null;
            }
            string sourceJwt = cookies[0]?.Value;
            if (string.IsNullOrEmpty(sourceJwt))
            {
                return null;
            }
            string encodeJwt = sourceJwt.Replace("ASP.NET_SessionId=", "");
            if (string.IsNullOrWhiteSpace(encodeJwt))
            {
                return null;
            }
            IDictionary<string, object> jwtObjects = GetJwtDecode(encodeJwt);
            if (jwtObjects == null || !jwtObjects.ContainsKey("val"))
            {
                return null;
            }
            string sourceKey = jwtObjects["val"].ToString();
            if (string.IsNullOrWhiteSpace(sourceKey))
            {
                return null;
            }
            string key = sourceKey.Replace("\r", "").Split("\n")[0];
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            byte[] keyBytes = Convert.FromBase64String(key);
            key = Encoding.ASCII.GetString(keyBytes);
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            key = key.Substring(9, 16);
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            token = key;
            string decode = AesHelper.AesDecryptor(oldResponse, key);
            return decode;

            IDictionary<string, object> GetJwtDecode(string token)
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var dicInfo = decoder.DecodeToObject(token);
                return dicInfo;
            }
        }
    }
}
