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
                if (url.Contains(_customerProductUrl, StringComparison.OrdinalIgnoreCase))
                {
                    //处理疫苗页面
                    ProcessCustomerProduct(oldResponse);
                }
                if (url.Contains(_custSubscribeDateAll, StringComparison.OrdinalIgnoreCase))
                {
                    //处理预约页面
                    ProcessCustSubscribeDate(oldResponse);
                }
                if (url.Contains(_custSubscribeDateDetail, StringComparison.OrdinalIgnoreCase))
                {
                    //处理疫苗剩余剂次
                    ProcessCustSubscribeDateDetail(oldResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void ProcessCustomerProduct(string oldResponse)
        {
            SubscribeResponseModel responseModel = JsonUtil.DeserializeStringToObject<SubscribeResponseModel>(oldResponse);
            if (responseModel.status != 200)
            {
                return;
            }
            if (responseModel.list == null || !responseModel.list.Any())
            {
                return;
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
            EventArgs.SetResponseBodyString(JsonUtil.SerializeToString(responseModel));
        }

        private void ProcessCustSubscribeDate(string oldResponse)
        {
            CustSubscribeDateResponseModel responseModel = JsonUtil.DeserializeStringToObject<CustSubscribeDateResponseModel>(oldResponse);
            if (responseModel.status != 200)
            {
                return;
            }
            if (responseModel.list == null || !responseModel.list.Any())
            {
                return;
            }
            foreach (var item in responseModel.list)
            {
                item.enable = true;
            }
            EventArgs.SetResponseBodyString(JsonUtil.SerializeToString(responseModel));
        }

        private void ProcessCustSubscribeDateDetail(string oldResponse)
        {
            //需要先解密
            string decodeResponse = DecodeResponse(oldResponse);
            if (string.IsNullOrEmpty(decodeResponse))
            {
                return;
            }
            CustSubscribeDateDetailResponseModel responseModel = JsonUtil.DeserializeStringToObject<CustSubscribeDateDetailResponseModel>(oldResponse);

        }

        /// <summary>
        /// 根据jwtToken  获取实体
        /// </summary>
        /// <param name="token">jwtToken</param>
        /// <returns></returns>
        public static IDictionary<string, object> GetJwtDecode(string token)
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

        private string DecodeResponse(string oldResponse)
        {
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
            string decode =  AesHelper.AesDecryptor(oldResponse, key);
            return decode;
        }
    }
}
