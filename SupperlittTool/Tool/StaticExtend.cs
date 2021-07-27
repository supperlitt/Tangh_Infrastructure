using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupperlittTool
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class StaticExtend
    {
        /// <summary>
        /// 转int32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt32(this string str)
        {
            return Convert.ToInt32(str);
        }

        /// <summary>
        /// 转datetime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToInt64(this string str)
        {
            return Convert.ToInt64(str);
        }

        /// <summary>
        /// 转datetime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            return Convert.ToDateTime(str);
        }
    }
}
