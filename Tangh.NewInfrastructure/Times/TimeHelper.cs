using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangh.NewInfrastructure.Http;
using System.Text.RegularExpressions;
using System.Net;

namespace Tangh.NewInfrastructure.Times
{
    /// <summary>
    /// 时间管理
    /// </summary>
    public class TimeHelper
    {
        /// <summary>
        /// 获取当前时间:获取北京时间
        /// 请求地址 http://time.tianqi.com/
        /// 返回数据：约10k
        /// </summary>
        /// <returns></returns>
        public static DateTime GetBeijinTime()
        {
            try
            {
                string url = "http://time.tianqi.com/";
                WebClient client = new WebClient();
                client.Encoding = Encoding.GetEncoding("gb2312");
                string content = client.DownloadString(url);
                Regex regex = new Regex(@"new\s+Date\(\((?<time>\d+)");
                string time = regex.Match(content).Groups["time"].Value;
                long t = Convert.ToInt64(time);
                DateTime startTime = DateTime.Parse("1970-1-1");

                // 北京时间东8区，加上8小时
                return startTime.AddHours(8).AddSeconds(t);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置系统时间，调用win32Helper的设置系统时间方法
        /// </summary>
        /// <param name="dt">设置时间会使用当前时间的是UTC</param>
        public static void SetSystemTime(DateTime dt)
        {
            Win32Api.Win32Helper.SetSystemTime(dt);
        }
    }
}