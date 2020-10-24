using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using Tangh.NewInfrastructure.Http;
using System.Web.Script.Serialization;

namespace Tangh.NewInfrastructure.IP
{
    public class IPManager
    {
        private static JavaScriptSerializer js = new JavaScriptSerializer();

        /// <summary>
        /// 获取局域网IP地址,如果计算机配置了公网IP，可能得到公网的IP
        /// </summary>
        /// <returns></returns>
        public static string GetLanIP()
        {
            string ip = string.Empty;
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection nics = mc.GetInstances();
                foreach (ManagementObject nic in nics)
                {
                    if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                    {
                        ip = (nic["IPAddress"] as string[])[0]; // IP地址
                        // string ipsubnet = (nic["IPSubnet"] as String[])[0]; // 子网掩码
                        if (nic["DefaultIPGateway"] == null)
                        {
                            continue;
                        }

                        break;
                    }
                }

                return ip;
            }
            catch
            {
                return "未找到";
            }
        }

        /// <summary>
        /// 获取外网IP类：根据POST http://ip.url.cn 判断
        /// </summary>
        /// <param name="ip">返回上网出口Ip</param>
        /// <param name="addr">出口Ip对应地址</param>
        public static void GetOutIP(ref string ip, ref string addr)
        {
            try
            {
                TMWebClient client = new TMWebClient(10);
                client.Encoding = Encoding.UTF8;
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string resultStr = client.DownloadString("http://ip.url.cn?t=" + (long)(DateTime.Now - DateTime.Parse("1970-1-1")).TotalSeconds);
                Regex regex = new Regex(@"<tbody>\s+<tr>\s+<td>(?<ip>[^<]+)</td>\s+<td>(?<c>[^<]+)</td>\s+<td>(?<p>[^<]+)</td>\s+<td>(?<ct>[^<]+)");
                ip = regex.Match(resultStr).Groups["ip"].Value;
                addr = string.Format("{0}-{1}-{2}", regex.Match(resultStr).Groups["c"].Value, regex.Match(resultStr).Groups["p"].Value, regex.Match(resultStr).Groups["ct"].Value);
            }
            catch
            {
                ip = string.Empty;
                addr = string.Empty;
            }
        }

        public static string GetOutIP2()
        {
            string ip = string.Empty;
            try
            {
                using (TMWebClient client = new TMWebClient())
                {
                    client.Encoding = Encoding.UTF8;

                    // 110.187.172.250中国-达州-电信
                    // {"data":{"area":"","country":"中国","isp_id":"100017","queryIp":"110.187.172.250","city":"达州","ip":"110.187.172.250","isp":"电信","county":"","region_id":"510000","area_id":"","county_id":null,"region":"四川","country_id":"CN","city_id":"511700"},"msg":"query success","code":0}
                    string result = client.DownloadString("http://ip.taobao.com/outGetIpInfo?ip=myip&accessKey=alibaba-inc");
                    var info = js.Deserialize<OutIpInfo>(result);

                    return info.data.ip + info.data.country + "-" + info.data.city + "-" + info.data.isp;
                }
            }
            catch
            {
                return "未找到";
            }
        }
    }
    public class OutIpInfo
    {
        public int code { get; set; }

        public string msg { get; set; }

        public OutIpDetailInfo data { get; set; }
    }

    public class OutIpDetailInfo
    {
        public string ip { get; set; }

        public string country { get; set; }

        public string city { get; set; }

        public string isp { get; set; }
    }
}