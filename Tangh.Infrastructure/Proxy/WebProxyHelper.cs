using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Tangh.Infrastructure
{
    public class ProxyManager
    {
        /// <summary>
        /// 取消设置Lan代理
        /// </summary>
        /// <returns></returns>
        public static bool UnsetProxy()
        {
            return SetProxy(null, null);
        }

        /// <summary>
        /// 设置Lan代理
        /// </summary>
        /// <param name="strProxy">IP:端口号  eg:192.168.0.1:8080</param>
        /// <returns></returns>
        public static bool SetProxy(string strProxy)
        {
            return SetProxy(strProxy, null);
        }

        public static bool SetProxy(string strProxy, string exceptions)
        {
            InternetPerConnOptionList list = new InternetPerConnOptionList();

            int optionCount = string.IsNullOrEmpty(strProxy) ? 1 : (string.IsNullOrEmpty(exceptions) ? 2 : 3);
            InternetPerConnOption[] options = new InternetPerConnOption[optionCount];
            //not use or use proxy server
            options[0].m_Option = PerConnOption.INTERNET_PER_CONN_FLAGS;
            options[0].m_Value.m_Int = (int)((optionCount < 2) ? PerConnFlags.PROXY_TYPE_DIRECT : (PerConnFlags.PROXY_TYPE_DIRECT | PerConnFlags.PROXY_TYPE_PROXY));
            //proxy server
            if (optionCount > 1)
            {
                options[1].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
                options[1].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(strProxy);
                //except for these addresses ...
                if (optionCount > 2)
                {
                    options[2].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS;
                    options[2].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(exceptions);
                }
            }

            // default stuff
            list.dwSize = Marshal.SizeOf(list);
            list.pszConnection = IntPtr.Zero;
            list.dwOptionCount = options.Length;
            list.dwOptionError = 0;

            int optSize = Marshal.SizeOf(typeof(InternetPerConnOption));
            // make a pointer out of all that ...
            IntPtr optionsPtr = Marshal.AllocCoTaskMem(optSize * options.Length);
            // copy the array over into that spot in memory ...
            for (int j = 0; j < options.Length; ++j)
            {
                IntPtr opt = new IntPtr(optionsPtr.ToInt32() + (j * optSize));
                Marshal.StructureToPtr(options[j], opt, false);
            }

            list.pOptions = optionsPtr;

            // and then make a pointer out of the whole list
            IntPtr ipcoListPtr = Marshal.AllocCoTaskMem((Int32)list.dwSize);
            Marshal.StructureToPtr(list, ipcoListPtr, false);

            // and finally, call the API method!
            int returnvalue = NativeMethods.InternetSetOption(IntPtr.Zero,
               InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
               ipcoListPtr, list.dwSize) ? -1 : 0;
            if (returnvalue == 0)
            {  // get the error codes, they might be helpful
                returnvalue = Marshal.GetLastWin32Error();
            }
            // FREE the data ASAP
            Marshal.FreeCoTaskMem(optionsPtr);
            Marshal.FreeCoTaskMem(ipcoListPtr);
            if (returnvalue > 0)
            {  // throw the error codes, they might be helpful
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (returnvalue < 0);
        }

        #region WinInet structures

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetPerConnOptionList
        {
            /// <summary>
            /// Size of the structure, in bytes.
            /// </summary>
            public int dwSize;
            /// <summary>
            /// Pointer to a string that contains the name of the RAS connection or NULL, which indicates the default or LAN connection, to set or query options on.
            /// </summary>
            public IntPtr pszConnection;
            /// <summary>
            /// Number of options to query or set.
            /// </summary>
            public int dwOptionCount;
            /// <summary>
            /// Options that failed, if an error occurs.
            /// </summary>
            public int dwOptionError;
            /// <summary>
            /// Pointer to an array of INTERNET_PER_CONN_OPTION structures containing the options to query or set.
            /// </summary>
            public IntPtr pOptions;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetPerConnOption
        {
            static readonly int Size;
            public PerConnOption m_Option;
            public InternetPerConnOptionValue m_Value;
            static InternetPerConnOption()
            {
                InternetPerConnOption.Size = Marshal.SizeOf(typeof(InternetPerConnOption));
            }

            // Nested Types
            [StructLayout(LayoutKind.Explicit)]
            public struct InternetPerConnOptionValue
            {
                [FieldOffset(0)]
                public System.Runtime.InteropServices.ComTypes.FILETIME m_FileTime;
                [FieldOffset(0)]
                public int m_Int;
                [FieldOffset(0)]
                public IntPtr m_StringPtr;
            }
        }
        #endregion

        #region WinInet enums

        /// <summary>
        /// options manifests for Internet{Query|Set}Option
        /// </summary>
        public enum InternetOption : uint
        {
            /// <summary>
            /// Causes the proxy data to be reread from the registry for a handle.
            /// </summary>
            INTERNET_OPTION_REFRESH = 37,
            /// <summary>
            /// Notifies the system that the registry settings have been changed so that it verifies the settings on the next call to InternetConnect. 
            /// </summary>
            INTERNET_OPTION_SETTINGS_CHANGED = 39,
            /// <summary>
            /// Sets or retrieves an INTERNET_PER_CONN_OPTION_LIST structure that specifies a list of options for a particular connection.
            /// </summary>
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75
        }

        /// <summary>
        /// Options used in INTERNET_PER_CONN_OPTON struct
        /// </summary>
        public enum PerConnOption
        {
            /// <summary>
            /// Sets or retrieves the connection type. The Value member will contain one or more of the values from PerConnFlags.
            /// </summary>
            INTERNET_PER_CONN_FLAGS = 1,
            /// <summary>
            /// Sets or retrieves a string containing the proxy servers.  
            /// </summary>
            INTERNET_PER_CONN_PROXY_SERVER = 2,
            /// <summary>
            /// Sets or retrieves a string containing the URLs that do not use the proxy server.
            /// </summary>
            INTERNET_PER_CONN_PROXY_BYPASS = 3,
            /// <summary>
            /// Sets or retrieves a string containing the URL to the automatic configuration script.
            /// </summary>
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4
        }

        /// <summary>
        /// PER_CONN_FLAGS
        /// </summary>
        [Flags]
        public enum PerConnFlags
        {
            /// <summary>
            /// The connection does not use a proxy server.
            /// </summary>
            PROXY_TYPE_DIRECT = 0x00000001,
            /// <summary>
            /// The connection uses an explicitly set proxy server.
            /// </summary>
            PROXY_TYPE_PROXY = 0x00000002,
            /// <summary>
            /// The connection downloads and processes an automatic configuration script at a specified URL.
            /// </summary>
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,
            /// <summary>
            /// The connection automatically detects settings.
            /// </summary>
            PROXY_TYPE_AUTO_DETECT = 0x00000008
        }
        #endregion

        #region WinInet methods

        internal static class NativeMethods
        {
            [DllImport("WinInet.dll", SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool InternetSetOption(IntPtr hInternet, InternetOption dwOption, IntPtr lpBuffer, int dwBufferLength);
        }

        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
    }

    /// <summary>
    /// Web代理帮助类
    /// </summary>
    public class WebProxyHelper
    {
        /// <summary>
        /// 页码
        /// </summary>
        private int pageIndex = 0;

        /// <summary>
        /// Ip索引
        /// </summary>
        private int index = 0;

        private int maxPage = 0;

        private List<IPModel> currentList = new List<IPModel>();

        public WebProxyHelper(int pageIndex = 1, int index = -1)
        {
            this.pageIndex = pageIndex;
            this.index = index;
        }

        public string ReadIPPortStr()
        {
            if (currentList.Count == 0)
            {
                LoadCurrentIPList();
            }

            index++;
            if (currentList.Count > index)
            {
                return string.Format("{0}:{1}", currentList[index].IP, currentList[index].Port);
            }
            else
            {
                if (pageIndex < maxPage)
                {
                    pageIndex++;
                    index = 0;
                    LoadCurrentIPList();
                    if (currentList.Count == 0)
                    {
                        pageIndex = 1;
                    }

                    LoadCurrentIPList();
                }
                else
                {
                    pageIndex = 1;
                    index = 0;
                    LoadCurrentIPList();
                }
            }

            return string.Format("{0}:{1}", currentList[index].IP, currentList[index].Port);
        }

        private void LoadCurrentIPList()
        {
            currentList = new List<IPModel>();
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string result = client.DownloadString(string.Format("http://www.xici.net.co/nn/{0}", pageIndex));
            Regex regex = new Regex(@"<td>(?<ip>[^<]+)</td>\s+<td>(?<port>\d+)</td>\s+<td>\s+(?<info>[^td]{0,})\s+</td>\s+<td>(?<ni>[^<]+)</td>\s+<td>(?<type>[^<]+)</td>");

            IPModel model = null;
            foreach (Match m in regex.Matches(result))
            {
                string type = m.Groups["type"].Value;
                if (type.ToUpper().Contains("HTTP"))
                {
                    model = new IPModel();
                    model.IP = m.Groups["ip"].Value;
                    model.Port = Convert.ToInt32(m.Groups["port"].Value);

                    currentList.Add(model);
                }
            }

            Regex maxpageRegex = new Regex(@">(?<maxpage>\d+)</a>\s+<a\s+class=""next_page""");
            string maxPageStr = maxpageRegex.Match(result).Groups["maxpage"].Value;
            this.maxPage = Convert.ToInt32(maxPageStr);
        }
    }

    /// <summary>
    /// IP对象
    /// </summary>
    public class IPModel
    {
        public string IP { get; set; }

        public int Port { get; set; }
    }
}
