using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SupperlittTool
{
    public class HttpClientHelper
    {
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url, Dictionary<string, string> headers = null)
        {
            if (url.StartsWith("https"))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }

            HttpClientHandler handler = new HttpClientHandler() { AllowAutoRedirect = true };
            // 低版本不受支持的方法
            // handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            HttpClient httpClient = new HttpClient(handler, true);
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            string result = string.Empty;
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Redirect)
            {
                result = response.Content.ReadAsStringAsync().Result;
            }

            return result;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <param name="contentType">默认application/x-www-form-urlencoded</param>
        /// <param name="contentType">默认null</param>
        /// <returns></returns>
        public static string Post(string url, string postData, string contentType = null, Dictionary<string, string> headers = null)
        {
            if (url.StartsWith("https"))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }

            HttpContent httpContent = new StringContent(postData);
            if (string.IsNullOrEmpty(contentType))
            {
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }
            else
            {
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }

            HttpClientHandler handler = new HttpClientHandler() { AllowAutoRedirect = true };
            HttpClient httpClient = new HttpClient(handler, true);
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;

                return result;
            }

            return string.Empty;
        }
    }
}
