﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;

namespace SupperlittTool
{
    public class Logs
    {
        private static object lockObj = new object();

        public static void WriteLog(LogType type, string msg)
        {
            lock (lockObj)
            {
                string pdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"); // 1234567890 的 md5
                if (!Directory.Exists(pdir))
                {
                    Directory.CreateDirectory(pdir);
                }

                string dir = Path.Combine(pdir, type.ToString());
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                try
                {
                    // 删除7天前的日志
                    long value = long.Parse(DateTime.Now.AddDays(-7).ToString("yyyyMMddHH"));
                    string[] files = Directory.GetFiles(dir);
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        try
                        {
                            long current_value = long.Parse(fileInfo.Name.Substring(0, 10));
                            if (current_value < value)
                            {
                                File.Delete(file);
                            }
                        }
                        catch { }
                    }
                }
                catch { }

                string path = Path.Combine(dir, DateTime.Now.ToString("yyyyMMddHH") + ".txt");
                File.AppendAllText(path, string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg), Encoding.UTF8);
            }
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        Info = 0,
        Error = 1,
        Http = 2
    }
}
