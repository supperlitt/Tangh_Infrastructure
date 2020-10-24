using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJuQue
{
    /// <summary>
    /// 自定义主键ID
    /// </summary>
    public class KeyGenerator
    {
        private static object lockObj = new object();
        private static List<KeyInfo> keyList = new List<KeyInfo>();

        /// <summary>
        /// 生成主键ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回：年月日时分秒(14)类型(2)自增索引(5)</returns>
        public static string GetKeyId(int type)
        {
            if (type.ToString().Length > 2)
            {
                throw new Exception("类型不能超过两位数");
            }

            lock (lockObj)
            {
                string time = DateTime.Now.ToString("yyyyMMddHHmmss") + type.ToString().PadLeft(2, '0');
                var item = keyList.Find(p => p.type == type);
                if (item == null)
                {
                    item = new KeyInfo() { index = 1, top = time, type = type };
                    keyList.Add(item);
                }
                else
                {
                    if (item.top == time)
                    {
                        // index+1,然后存入，返回
                        item.index += 1;
                    }
                    else
                    {
                        item.top = time;
                        item.index = 1;
                    }
                }

                // 新增，默认index=1
                return string.Format("{0}{1}", item.top, item.index.ToString().PadLeft(5, '0'));
            }
        }
    }

    public class KeyInfo
    {
        /// <summary>
        /// 0 - xxx
        /// </summary>
        public int type { get; set; }

        public string top { get; set; }

        public int index { get; set; }
    }
}