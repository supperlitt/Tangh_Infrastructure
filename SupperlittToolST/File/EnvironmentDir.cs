using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupperlittTool
{
    /// <summary>
    /// 环境特殊文件夹的路径
    /// </summary>
    public class EnvironmentDir
    {
        public static string GetDir(Environment.SpecialFolder folder)
        {
            ///得到特殊目录
            return Environment.GetFolderPath(folder);
        }
    }
}
