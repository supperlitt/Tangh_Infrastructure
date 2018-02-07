using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tangh.NewInfrastructure.Logs
{
    public class Log
    {
        public static void WriteLog(string msg)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            File.AppendAllText(Path.Combine(logPath, "Info" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt"), msg);
        }

        public static void WriteLog(LogType type, string msg)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            File.AppendAllText(Path.Combine(logPath, type.ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt"), msg);
        }

        public static void WriteErrorLog(string msg)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            File.AppendAllText(Path.Combine(logPath, "Error" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt"), msg);
        }
    }

    public enum LogType
    {
        Info = 0,
        Error = 1
    }
}