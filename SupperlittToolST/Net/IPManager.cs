using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace SupperlittTool
{
    public class IPManager
    {
        /// <summary>
        /// 获得当前IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLanIP()
        {
            IPAddress ipAddr = Dns.Resolve(Dns.GetHostName()).AddressList[0];

            return ipAddr.ToString();
        }

        /// <summary>
        /// 获取局域网IP地址,如果计算机配置了公网IP，可能得到公网的IP
        /// </summary>
        /// <returns></returns>
        public static string GetLanIP_WMS()
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
                return "";
            }
        }

        /// <summary>
        /// 获取外网IP类：根据POST ip138.com 判断
        /// </summary>
        public static string GetOutIP()
        {
            try
            {
                Regex regex = new Regex(@"您的IP地址是：(?<ip>[^<]+)");
                using (TMWebClient client = new TMWebClient(10))
                {
                    client.Encoding = Encoding.UTF8;
                    string result = client.DownloadString("https://" + DateTime.Now.Year + ".ip138.com");

                    return regex.Match(result).Groups["ip"].Value;
                }
            }
            catch
            {
                return "";
            }
        }
    }
}