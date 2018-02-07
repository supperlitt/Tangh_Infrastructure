using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tangh.Infrastructure
{
    /// <summary>
    /// 文本-日志处理类
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object lockObj = new object();

        /// <summary>
        /// 记录日志文本
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="content">日志内容</param>
        public static void WriteLog(LogType type, string content)
        {
            // 异步执行记录日志操作
            Action<LogType, string> writeAction = new Action<LogType, string>(PWriteLog);
            writeAction.BeginInvoke(type, content, null, null);
        }

        /// <summary>
        /// 记录日志文本
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="content">日志内容</param>
        public static void WriteLog(string type, string content)
        {
            // 异步执行记录日志操作
            Action<string, string> writeAction = new Action<string, string>(IWriteLog);
            writeAction.BeginInvoke(type, content, null, null);
        }

        /// <summary>
        /// 记录文本日志具体操作
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="content">内容</param>
        private static void PWriteLog(LogType type, string content)
        {
            lock (lockObj)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, type.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHH") + ".txt");
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + content);
                    }
                }
            }
        }

        /// <summary>
        /// 记录文本日志具体操作
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="content">内容</param>
        private static void IWriteLog(string type, string content)
        {
            lock (lockObj)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, type.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHH") + ".txt");
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + content);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        Info = 0,
        Error = 1
    }
}
