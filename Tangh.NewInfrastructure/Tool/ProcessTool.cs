using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Tangh.NewInfrastructure
{
    public class ProcessTool
    {
        public static void RunBat()
        {
            Process.Start(new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.bat")));
        }

        public static void Check_KillProcess(string check_commandLine)
        {
            try
            {
                List<string> results = new List<string>();
                string wmiQuery = "select CommandLine from Win32_Process where Name='Test.exe'";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery))
                {
                    using (ManagementObjectCollection retObjectCollection = searcher.Get())
                    {
                        if (retObjectCollection.Count > 0)
                        {
                            foreach (ManagementObject retObject in retObjectCollection)
                            {
                                string commandLine = (string)retObject["CommandLine"];
                                if (!commandLine.EndsWith(check_commandLine))
                                {
                                    // 正确的情况
                                    // 结束微信
                                    Process[] ps = Process.GetProcesses();
                                    foreach (var p in ps)
                                    {
                                        try
                                        {
                                            if (p.MainModule.ModuleName == "Test.exe")
                                            {
                                                p.Kill();
                                                break;
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 开机启动
        /// </summary>
        public static void AutoRun()
        {
            RegistryKey _rlocal = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;   //E:\Code\XXX.JobRunner\bin\Debug\KMHC.OCP.JobRunner.exe     XXX是路径和namespace
            var appName = appPath.Substring(appPath.LastIndexOf('\\') + 1);     //XXX.JobRunner.exe  XXX 是namespace
            try
            {
                _rlocal.SetValue(appName, string.Format(@"""{0}""", appPath));
                _rlocal.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("设置开机是否启动失败: {0}", ex.Message));
            }
        }

        public static void RunExe(string exe_path)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = exe_path;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = exe_path.Substring(0, exe_path.LastIndexOf("\\"));
            psi.CreateNoWindow = true;

            Process.Start(psi);
        }
    }
}
