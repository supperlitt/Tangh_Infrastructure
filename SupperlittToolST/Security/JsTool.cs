using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Security.Cryptography;
using System.IO;

namespace SupperlittTool
{
    /// <summary>
    /// Js相关的功能代码
    /// </summary>
    public class JsTool
    {
        /// <summary>
        /// 1970年格林威治时间,,不多计算出来有偏差，差9那个值
        /// </summary>
        private static long lLeft = 621355968000000000;

        /// <summary>
        /// 得到10位js时间值
        /// </summary>
        /// <returns></returns>
        public static long GetIntFromTime()
        {
            DateTime dt = DateTime.UtcNow;
            DateTime dt1 = dt.ToUniversalTime();
            long Sticks = (dt1.Ticks - lLeft) / 10000000;
            return Sticks;
        }

        /// <summary>
        /// 得到13位js时间值
        /// </summary>
        /// <returns></returns>
        public static long GetLongFromTime()
        {
            DateTime dt = DateTime.UtcNow;
            DateTime dt1 = dt.ToUniversalTime();
            long Sticks = (dt1.Ticks - lLeft) / 10000;
            return Sticks;
        }

        /// <summary>
        /// 得到js随机数值
        /// </summary>
        /// <returns></returns>
        public static string GetRandrom()
        {
            Random rand = new Random();
            string strNum = "";
            int i = 0;
            while (i < 16)
            {
                i++;
                int randomNum = rand.Next(0, 10);
                strNum += randomNum.ToString();
            }
            strNum = "0." + strNum;
            return strNum;
        }

        /// <summary>
        /// 获取Cookie的值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static string GetCookie(string cookieName, CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c1 in colCookies) lstCookies.Add(c1);
            }

            var model = lstCookies.Find(p => p.Name == cookieName);
            if (model != null)
            {
                return model.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取Cookie的值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static string GetCookieLike(string cookieName, CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c1 in colCookies) lstCookies.Add(c1);
            }

            var model = lstCookies.Find(p => p.Name.StartsWith(cookieName));
            if (model != null)
            {
                return model.Name;
            }

            return string.Empty;
        }

        public static string GetCookieAll(CookieContainer cc)
        {
            StringBuilder content = new StringBuilder();
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c1 in colCookies) lstCookies.Add(c1);
            }

            foreach (var item in lstCookies)
            {
                content.AppendFormat("{0}={1};", item.Name, item.Value);
            }

            return content.ToString();
        }

        /// <summary>
        /// MD5字符串
        /// </summary>
        /// <param name="key">需要加密的字符串</param>
        /// <returns>返回MD5加密后的结果</returns>
        public static string GetMD5String(string key)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(key)))
                {
                    return BitConverter.ToString(md5.ComputeHash(ms)).Replace("-", string.Empty);
                }
            }
        }

        public static string GetSHA1String(string key)
        {
            using (SHA1CryptoServiceProvider md5 = new SHA1CryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(key)))
                {
                    return BitConverter.ToString(md5.ComputeHash(ms)).Replace("-", string.Empty);
                }
            }
        }

        /// <summary>
        /// 获取文件的MD5值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回文件MD5值</returns>
        public static string GetFileMD5(string filePath)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", string.Empty);
                }
            }
        }

        public static byte[] GetMD5Result(byte[] data)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    return md5.ComputeHash(ms);
                }
            }
        }

        /// <summary>
        /// 排序并拼接字符串计算md5
        /// </summary>
        /// <param name="arg8"></param>
        /// <param name="EXTEND"></param>
        /// <returns></returns>
        public static String toCommonPHPMD5(Dictionary<string, string> arg8, string EXTEND)
        {
            StringBuilder v0 = new StringBuilder();
            String[] v1 = arg8.Keys.ToArray();

            int v2 = 0;
            int v3;
            for (v3 = 0; v3 < arg8.Count; ++v3)
            {
                v1[v3] = arg8.Keys.ToArray()[v3];
                for (int v4 = 0; v4 < v1.Length - v3 - 1; v4++)
                {
                    if (v1[v4].CompareTo(v1[v4 + 1]) > 0)
                    {
                        String v5 = v1[v4];
                        v1[v4] = v1[v4 + 1];
                        v1[v4 + 1] = v5;
                    }
                }
            }

            while (v2 < arg8.Count)
            {
                if (arg8[v1[v2]] != null)
                {
                    v0.Append(v1[v2] + "=" + arg8[v1[v2]] + "&");
                }
                else
                {
                    v0.Append(v1[v2] + "=&");
                }

                ++v2;
            }

            v0.Remove(v0.Length - 1, 1);
            v0.Append(EXTEND);

            return JsTool.GetMD5String(v0.ToString());
        }
    }
}