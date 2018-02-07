using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;

namespace Tangh.Infrastructure
{
    public class IPManager
    {
        /// <summary>
        /// 获取局域网IP地址,如果计算机配置了公网IP，可能得到公网的IP
        /// </summary>
        /// <returns></returns>
        public static string GetLanIP()
        {
            string ip = string.Empty;
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

        /// <summary>
        /// 获取IP类
        /// </summary>
        /// <param name="ip">返回上网出口Ip</param>
        /// <param name="addr">出口Ip对应地址</param>
        public static void GetIP138(ref string ip, ref string addr)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string result = client.DownloadString("http://1111.ip138.com/ic.asp?r=" + new Random().Next(1000, 10000));
                Regex fromRegex = new Regex(@"您的IP是：\[(?<ip>[^\]]+)\]\s+(?<addr>[^<]+)");
                Match m = fromRegex.Match(result);
                ip = m.Groups["ip"].Value;
                addr = m.Groups["addr"].Value;
            }
            catch
            {
                ip = string.Empty;
                addr = string.Empty;
            }
        }
    }
}