using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Update
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 接收传入的两个参数 1：下载增量包 url;   2：需要运行的程序的名称（包含.exe),后面修改配置文件，需要通过这个来计算
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args != null && args.Length > 0)
            {
                string url = args[0];
                string fileName = args[1];
                Application.Run(new UpdateForm(args[0], fileName));
            }
            else
            {
                Application.Run(new UpdateForm());
            }
        }
    }
}
