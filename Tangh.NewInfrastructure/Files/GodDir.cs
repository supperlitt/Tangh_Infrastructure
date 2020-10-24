using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangh.NewInfrastructure
{
    public class GodDir
    {
        public static string GetDir()
        {
            ///得到特殊目录
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
