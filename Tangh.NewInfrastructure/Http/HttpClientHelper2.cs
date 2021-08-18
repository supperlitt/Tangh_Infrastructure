using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tangh.NewInfrastructure
{
    public class HttpClientHelper2
    {
        private static Dictionary<string, string> headers = null;

        public static void InitHeader(Dictionary<string, string> headers)
        {
            HttpClientHelper2.headers = headers;
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //HttpClientHandler handler = new HttpClientHandler() { AllowAutoRedirect = false };
            //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            var handler = new HttpClientHandler();
            
            // 高版本才有.net core
            // var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true };
            HttpClient httpClient = new HttpClient(handler);
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

            if (string.IsNullOrEmpty(result))
            {

            }

            return string.Empty;
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetResponse_Bytes(string url)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClientHandler handler = new HttpClientHandler() { AllowAutoRedirect = false };

            // 高版本才有.net core
            // handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;

            HttpClient httpClient = new HttpClient(handler);
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            byte[] result = new byte[0];
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Redirect)
            {
                result = response.Content.ReadAsByteArrayAsync().Result;
            }

            return result;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClientHandler handler = new HttpClientHandler() { AllowAutoRedirect = false };

            // 高版本才有.net core
            // handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpClient httpClient = new HttpClient(handler);
            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostResponse_JSON(string url, string postData)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClientHandler handler = new HttpClientHandler() { AllowAutoRedirect = false };
            // 高版本才有.net core
            // handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient(handler);
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
