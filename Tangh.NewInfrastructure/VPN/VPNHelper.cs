using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Tangh.NewInfrastructure.VPN
{
    /// <summary>
    /// VPN管理器
    /// </summary>
    public class VPNHelper
    {
        private static string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
        private static string fileName = @"\rasdial.exe";//@"\rasphone.exe";
        private static string rasphone = @"\rasphone.exe";
        private static string VPNDelete = WinDir + rasphone;
        private static string VPNPROCESS = WinDir + fileName;

        private string vpnName = string.Empty;
        private string ip = string.Empty;
        private string userName = string.Empty;
        private string password = string.Empty;

        public VPNHelper(string vpnName, string ip, string userName, string password)
        {
            this.vpnName = vpnName;
            this.ip = ip;
            this.userName = userName;
            this.password = password;
        }

        public bool TestConnection()
        {
            bool RV = false;
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

                if (ping.Send(this.ip).Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    RV = true;
                }
                else
                {
                    RV = false;
                }
                ping = null;
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
                RV = false;
            }
            return RV;
        }

        public bool ConnectToVPN()
        {
            bool RV = false;
            try
            {

                string args = string.Format("{0} {1} {2}", this.vpnName, this.userName, this.password);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = true;

                myProcess.UseShellExecute = false;
                Process.Start(myProcess);
                RV = true;

            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
                RV = false;
            }
            return RV;
        }

        public bool DisconnectFromVPN()
        {
            bool RV = false;
            try
            {
                //System.Diagnostics.Process.Start(VPNPROCESS, " -h " + vpn.VPNConnectionName);
                //System.Diagnostics.Process.Start(VPNPROCESS, string.Format(@"{0} /d",vpn.VPNConnectionName));
                string args = string.Format(@"{0} /d", this.vpnName);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = false;
                myProcess.UseShellExecute = false;
                System.Diagnostics.Process.Start(myProcess);

                //System.Windows.Forms.Application.DoEvents();
                //System.Threading.Thread.Sleep(2000);
                //System.Windows.Forms.Application.DoEvents();
                RV = true;
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
                RV = false;
            }
            return RV;
        }

        public bool DeleteVPN()
        {
            bool RV = false;
            try
            {
                //System.Diagnostics.Process.Start(VPNPROCESS, " -h " + vpn.VPNConnectionName);
                //System.Diagnostics.Process.Start(VPNPROCESS, string.Format(@"{0} /d",vpn.VPNConnectionName));
                string args = string.Format(@"{0} -r {1}", VPNDelete, this.vpnName);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = false;
                myProcess.UseShellExecute = false;
                System.Diagnostics.Process.Start(myProcess);

                //System.Windows.Forms.Application.DoEvents();
                //System.Threading.Thread.Sleep(2000);
                //System.Windows.Forms.Application.DoEvents();
                RV = true;
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
                RV = false;
            }

            return RV;
        }

        /// <summary>
        /// 这个方法，其实并不怎么靠谱。最好是看看ipconfig的内容，使用IsConnectVPN判断
        /// </summary>
        /// <returns></returns>
        public static List<string> ReadConnection()
        {
            List<string> result = new List<string>();
            try
            {
                string args = VPNPROCESS;
                Process myProcess = new Process();
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.FileName = "cmd.exe";
                myProcess.Start();
                myProcess.StandardInput.WriteLine(VPNPROCESS);
                myProcess.StandardInput.WriteLine("exit");
                myProcess.WaitForExit();

                string content = myProcess.StandardOutput.ReadToEnd();
                Regex regexlist = new Regex(@"已连接\s+?(?<list>[\s\S]+?)命令已完成。");
                if (regexlist.IsMatch(content))
                {
                    string list = regexlist.Match(content).Groups["list"].Value;
                    var array = list.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    if (array.Length > 0)
                    {
                        result.AddRange(array);
                    }
                }

                myProcess.Close();
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 判断是否连接到VPN使用cmd ipconfig命令，判断里面的子网掩码和默认网关的值
        /// VPN的默认网关代码为：255.255.255.255
        /// VPN的子网掩码默认值为：0.0.0.0
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectVPN()
        {
            Process myProcess = new Process();
            myProcess.StartInfo.CreateNoWindow = false;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.Start();
            myProcess.StandardInput.WriteLine("ipconfig");
            myProcess.StandardInput.WriteLine("exit");
            myProcess.WaitForExit();

            string content = myProcess.StandardOutput.ReadToEnd();
            if (content.Contains("255.255.255.255"))
            {
                return true;
            }

            return false;
        }
    }
}
