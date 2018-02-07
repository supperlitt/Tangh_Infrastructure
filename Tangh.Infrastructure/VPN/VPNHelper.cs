using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using DotRas;
using System.Net;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Tangh.Infrastructure
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

        static VPNHelper()
        {
            // 初始化VPN名称
            Random rand = new Random(DateTime.Now.Millisecond);
            int num = rand.Next(1000000, 10000000);
            VPNConnectionName = num.ToString();
        }

        public static string IPToPing
        {
            get { return ConfigurationManager.AppSettings["serverIp"].ToString(); }
        }

        public static string VPNConnectionName
        {
            get;
            set;
        }

        public static string UserName
        {
            get { return ConfigurationManager.AppSettings["userName"].ToString(); }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["password"].ToString();
            }
        }

        public static bool TestConnection()
        {
            VPNHelper vpn = new VPNHelper();//为了以后更多属性，其实现在完全可以不要
            bool RV = false;
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

                if (ping.Send(IPToPing).Status == System.Net.NetworkInformation.IPStatus.Success)
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

        public static bool ConnectToVPN()
        {
            VPNHelper vpn = new VPNHelper();
            bool RV = false;
            try
            {

                string args = string.Format("{0} {1} {2}", VPNConnectionName, UserName, Password);
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

        public static bool DisconnectFromVPN()
        {
            VPNHelper vpn = new VPNHelper();
            bool RV = false;
            try
            {
                //System.Diagnostics.Process.Start(VPNPROCESS, " -h " + vpn.VPNConnectionName);
                //System.Diagnostics.Process.Start(VPNPROCESS, string.Format(@"{0} /d",vpn.VPNConnectionName));
                string args = string.Format(@"{0} /d", VPNConnectionName);
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

        public static void CreateVPN()
        {
            VPNHelper vpn = new VPNHelper();
            using (DotRas.RasDialer dialer = new DotRas.RasDialer())
            {
                DotRas.RasPhoneBook allUsersPhoneBook = new DotRas.RasPhoneBook();
                allUsersPhoneBook.Open();
                if (allUsersPhoneBook.Entries.Contains(VPNConnectionName))
                {
                    return;
                }
                RasEntry entry = RasEntry.CreateVpnEntry(VPNConnectionName, IPToPing, RasVpnStrategy.PptpFirst, RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn));
                allUsersPhoneBook.Entries.Add(entry);
                dialer.EntryName = VPNConnectionName;
                dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
                try
                {
                    dialer.Credentials = new NetworkCredential(UserName, Password);
                    dialer.DialAsync();
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public static bool DeleteVPN()
        {
            VPNHelper vpn = new VPNHelper();
            bool RV = false;
            try
            {
                //System.Diagnostics.Process.Start(VPNPROCESS, " -h " + vpn.VPNConnectionName);
                //System.Diagnostics.Process.Start(VPNPROCESS, string.Format(@"{0} /d",vpn.VPNConnectionName));
                string args = string.Format(@"{0} -r {1}", VPNDelete, VPNConnectionName);
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

        public static List<string> ReadConnection()
        {
            List<string> result = new List<string>();
            VPNHelper vpn = new VPNHelper();
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
