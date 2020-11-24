using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace Tangh.NewInfrastructure.Win32Api
{
    public class Win32Helper2
    {
        #region 热键操作
        //如果函数执行成功，返回值不为0。            
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <param name="fsModifiers"></param>
        /// <param name="vk">(byte)Keys.D</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）           
            KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            byte vk                     //定义热键的内容
            );

        /*
         使用方式：
         * RegisterHotKey(this.Handle,100,KeyModifiers.None,Keys.F10);
         */

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );


        #endregion

        #region 查找窗体

        /// <summary>
        /// 遍历所有窗口对于的委托
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lParam">上层传入的值</param>
        /// <returns></returns>
        public delegate bool EnumWindowsProc(int hWnd, int lParam);

        /// <summary>
        /// 遍历进程中所有窗口
        /// </summary>
        /// <param name="ewp"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder title, int size);

        /// <summary>
        /// 获取当前窗口线程对于的进程ID
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 查找子窗口
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="hwndChildAfter"></param>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        /// <summary>
        /// 返回与指定窗口有特定关系的窗口句柄
        /// 在循环体中调用函数EnumChildWindow比调用GetWindow函数可靠。调用GetWindow函数实现该任务的应用程序可能会陷入死循环或退回一个已被销毁的窗口句柄。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="uCmd">参数类型：</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        #endregion

        #region 发送消息
        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hwnd, uint Msg, long wParam, long lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PostMessageW", ExactSpelling = true)]
        public static extern bool PostMessageW(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessageInt(IntPtr hWnd, int Msg, int wParm, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessageStrs(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SendMessageW", ExactSpelling = true)]
        public static extern bool SendMessageW(IntPtr hWnd, int msg, int wParam, int lParam);
        #endregion

        #region 窗体位置

        /// <summary>
        /// 设置鼠标位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        public static extern int SetCursorPos(int x, int y);

        /// <summary>
        /// 获取窗口大小及位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// 3-最大化窗口，2-最小化窗口，1-正常大小窗口；
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 设置目标窗体大小，位置
        /// </summary>
        /// <param name="hWnd">目标句柄</param>
        /// <param name="x">目标窗体新位置X轴坐标</param>
        /// <param name="y">目标窗体新位置Y轴坐标</param>
        /// <param name="nWidth">目标窗体新宽度</param>
        /// <param name="nHeight">目标窗体新高度</param>
        /// <param name="BRePaint">是否刷新窗体</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        /// <summary>
        /// 改变窗口的大小、位置和设置子窗口、弹出窗口或顶层窗口的排列顺序
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter">排列顺序的句柄</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="flags">
        /// 窗口定位标识
        /// SWP_DRAWFRAME,SWP_FRAMECHANGED,SWP_HIDEWINDOW,SWP_NOACTIVATE,...
        /// SWP_SHOWWINDOW 显示窗口
        /// </param>
        /// <returns>如果返回值非零表示成功，返回零表示失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

        /// <summary>
        /// 得到当前活动的窗口
        /// 获取当前系统中被激活的窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern System.IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        #endregion

        #region 键鼠操作
        /// <summary>
        /// 鼠标移动控制
        /// </summary>
        /// <param name="dwFlags">控制鼠标类型</param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dwData">0</param>
        /// <param name="dwExtraInfo">0</param>
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// 键盘事件，按下
        /// </summary>
        /// <param name="bVk">(byte)Keys.D</param>
        /// <param name="bScan">0</param>
        /// <param name="dwFlags">这里是整数类型  0 为按下，2为释放</param>
        /// <param name="dwExtraInfo">这里是整数类型 一般情况下设成为 0</param>
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;

        public const int MK_CONTROL = 0x0008;
        public const int MK_LBUTTON = 0x1;
        public const int MK_MBUTTON = 0x0010;
        public const int MK_RBUTTON = 0x0002;
        public const int MK_SHIFT = 0x0004;
        public const int MK_XBUTTON1 = 0x0020;
        public const int MK_XBUTTON2 = 0x0040;

        public const int WM_SYSKEYUP = 0X105;
        public const int WM_SYSKEYDOWN = 0X104;

        public const int WM_PASTE = 0x302;

        public const int WM_KEYUP = 0x0101;
        public const int WM_KEYDOWN = 0x0100;

        public const int VK_RETURN = 0x0D;

        public const int WM_IME_CHAR = 0x286;
        public const int WM_CHAR = 0x0102;

        // 缓冲区变量
        private const uint PURGE_TXABORT = 0x0001; // Kill the pending/current writes to the comm port. 
        private const uint PURGE_RXABORT = 0x0002; // Kill the pending/current reads to the comm port. 
        private const uint PURGE_TXCLEAR = 0x0004; // Kill the transmit queue if there. 
        private const uint PURGE_RXCLEAR = 0x0008; // Kill the typeahead buffer if there. 

        #endregion

        #region Kernel32

        public static int PROCESS_TERMINATE = (0x0001);
        public static int PROCESS_CREATE_THREAD = (0x0002);
        public static int PROCESS_SET_SESSIONID = (0x0004);
        public static int PROCESS_VM_OPERATION = (0x0008);
        public static int PROCESS_VM_READ = (0x0010);
        public static int PROCESS_VM_WRITE = (0x0020);
        public static int PROCESS_DUP_HANDLE = (0x0040);
        public static int PROCESS_CREATE_PROCESS = (0x0080);
        public static int PROCESS_SET_QUOTA = (0x0100);
        public static int PROCESS_SET_INFORMATION = (0x0200);
        public static int PROCESS_QUERY_INFORMATION = (0x0400);
        public static int PROCESS_SUSPEND_RESUME = (0x0800);
        public static int PROCESS_QUERY_LIMITED_INFORMATION = (0x1000);
        public static int PROCESS_SET_LIMITED_INFORMATION = (0x2000);
        public static uint PROCESS_ALL_ACCESS = (uint)(0x000F0000L | 0x00100000L | 0xFFFF);

        public static uint PAGE_EXECUTE_READWRITE = 0x40;
        public static uint MEM_COMMIT = 0x00001000;
        public static uint MEM_RESERVE = 0x00002000;

        [DllImport("Kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        [DllImport("Kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("Kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("Kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);
        [DllImport("Kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, int dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpLibFileNmae);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        #endregion

        #region 时间操作

        //设定，获取系统时间,SetSystemTime()默认设置的为UTC时间，比北京时间少了8个小时。  
        [DllImport("Kernel32.dll")]
        public static extern bool SetSystemTime(ref SYSTEMTIME time);
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);
        [DllImport("Kernel32.dll")]
        public static extern void GetSystemTime(ref SYSTEMTIME time);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SYSTEMTIME time);

        #endregion

        #region 具体操作

        /// <summary>
        /// 读取 SysListView32 中的数据到DataTable，列名从0-~
        /// </summary>
        /// <param name="hwndListView"></param>
        /// <returns></returns>
        public static DataTable GetListView(IntPtr hwndListView)
        {
            DataTable result = new DataTable();
            IntPtr headerPtr = (IntPtr)SendMessageInt(hwndListView, 0x101f, 0, 0);
            int columnCount = SendMessageInt(headerPtr, 0x1200, 0, 0);
            int rowCount = SendMessageInt(hwndListView, 0x1004, 0, 0);
            int pId;
            GetWindowThreadProcessId(hwndListView, out pId);
            IntPtr hProcess = OpenProcess((uint)0x38, false, (uint)pId);
            IntPtr lpBaseAddress = VirtualAllocEx(hProcess, IntPtr.Zero, 0x100, 0x3000, 4);
            try
            {
                for (int k = 0; k < columnCount; k++)
                {
                    result.Columns.Add(k.ToString());
                }
            }
            catch { }

            try
            {
                for (int i = 0; i < rowCount; i++)
                {
                    var dr = result.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        byte[] arr = new byte[0x100];
                        LVITEM[] lvitemArray = new LVITEM[1];
                        lvitemArray[0].mask = 1;
                        lvitemArray[0].iItem = i;
                        lvitemArray[0].iSubItem = j;
                        lvitemArray[0].cchTextMax = arr.Length;
                        lvitemArray[0].pszText = (IntPtr)(((int)lpBaseAddress) + Marshal.SizeOf(typeof(LVITEM)));
                        uint vNumberOfBytesRead = 0;
                        WriteProcessMemory(hProcess, lpBaseAddress, Marshal.UnsafeAddrOfPinnedArrayElement(lvitemArray, 0), Marshal.SizeOf(typeof(LVITEM)), ref vNumberOfBytesRead);
                        SendMessageInt(hwndListView, 0x1005, i, lpBaseAddress.ToInt32());
                        ReadProcessMemory(hProcess, (IntPtr)(((int)lpBaseAddress) + Marshal.SizeOf(typeof(LVITEM))), Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0), arr.Length, ref vNumberOfBytesRead);
                        int count = 0;
                        for (int k = 0; k < arr.Length; k++)
                        {
                            if (arr[k].ToString() == "0")
                            {
                                count = k;
                                break;
                            }
                        }

                        string str2 = "";
                        if (count > 0)
                        {
                            str2 = Encoding.Default.GetString(arr, 0, count).Trim();
                        }

                        dr[j] = str2;
                    }

                    result.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                VirtualFreeEx(hProcess, lpBaseAddress, 0, 0x8000);
                CloseHandle(hProcess);
            }

            return result;
        }

        /// <summary>
        /// 单击左键
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public static void MouseClick(int rX, int rY)
        {
            SetCursorPos(rX, rY);
            // 使用组合构造一次单击
            mouse_event(Win32Message.MOUSEEVENTF_LEFTDOWN | Win32Message.MOUSEEVENTF_LEFTUP, rX, rY, 0, 0);
        }

        /// <summary>
        /// 单击左键
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public static void MouseClick(Point p)
        {
            SetCursorPos(p.X, p.Y);
            // 使用组合构造一次单击
            mouse_event(Win32Message.MOUSEEVENTF_LEFTDOWN | Win32Message.MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
        }

        public static void KeyDown(byte key)
        {
            keybd_event((byte)key, 0, 0, 0);
            Thread.Sleep(50);
            keybd_event((byte)key, 0, 2, 0);
        }

        /// <summary>
        /// 设置窗口在最前面
        /// </summary>
        /// <param name="handle"></param>
        public static void SetWindowFirst(IntPtr handle)
        {
            ShowWindow(handle, 1);
            Thread.Sleep(300);
            SetWindowPos(handle, -1, 0, 0, 0, 0, 1 | 2);
        }

        public static void SetSystemTime(DateTime dt)
        {
            SYSTEMTIME time = new SYSTEMTIME();
            time.FromDateTime(dt);
            SetSystemTime(ref time);
        }
        #endregion

        /// <summary>
        /// 注入DLL，DLL内部可以开启Http服务，处理任务
        /// </summary>
        /// <param name="app_name"></param>
        /// <param name="inject_path"></param>
        public static void InjectDll(string app_name, string inject_path)
        {
            int pid = 0;
            var ps = Process.GetProcesses();
            foreach (var p in ps)
            {
                try
                {
                    if (p.MainModule.ModuleName == app_name)
                    {
                        pid = p.Id;
                        break;
                    }
                }
                catch { }
            }

            if (pid > 0)
            {
                IntPtr hProcess = OpenProcess((uint)PROCESS_ALL_ACCESS, false, (uint)pid);
                IntPtr lpBaseAddress = VirtualAllocEx(hProcess, IntPtr.Zero, 0x100, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

                uint numberOfSize = 0;
                bool is_ok = WriteProcessMemory(hProcess, lpBaseAddress, Marshal.StringToBSTR(inject_path), inject_path.Length + 1, ref numberOfSize);

                IntPtr loadAddr = GetProcAddress(GetModuleHandle("Kernel32"), "LoadLibraryA");

                CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadAddr, lpBaseAddress, 0, IntPtr.Zero);
            }
        }
    }
}