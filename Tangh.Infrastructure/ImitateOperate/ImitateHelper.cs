using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Tangh.Infrastructure
{
    public class ImitateHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        /// <summary>
        /// ListBox
        ///  选择一个字符串，并将其所在的条目滚动到视野内。当新的字符串被选定，
        ///  列表框的高亮显示将从原有的选中字符串移动到这个新的字符串上
        /// </summary>
        int LB_SETCURSEL = 0x0186;

        /// <summary>
        /// Combobox
        ///  选择一个字符串，并将其所在的条目滚动到视野内。当新的字符串被选定，
        ///  列表框的高亮显示将从原有的选中字符串移动到这个新的字符串上
        /// </summary>
        int CB_SETCURSEL = 0x014E;

        /// <summary>
        /// double click
        /// </summary>
        int WM_NCLBUTTONDBLCLK = 0x00A3;

        // 控制鼠标类型
        private readonly int MOUSEEVENTF_MOVE = 0x0001;//模拟鼠标移动
        private readonly int MOUSEEVENTF_LEFTDOWN = 0x0002;//模拟鼠标左键按下
        private readonly int MOUSEEVENTF_LEFTUP = 0x0004;//模拟鼠标左键抬起
        private readonly int MOUSEEVENTF_ABSOLUTE = 0x8000;//鼠标绝对位置
        private readonly int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        private readonly int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        private readonly int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        private readonly int MOUSEEVENTF_MIDDLEUP = 0x0040;// 模拟鼠标中键抬起 

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
        /// 查找窗口
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 查找子窗口
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="hwndChildAfter"></param>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        /// <summary>
        /// 发送Window消息
        /// </summary>
        /// <param name="Handle"></param>
        /// <param name="wMsg"></param>
        /// <param name="WParam"></param>
        /// <param name="LParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr Handle, int wMsg, int WParam, int LParam);

        /// <summary>
        /// 设置鼠标位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern int SetCursorPos(int x, int y);

        /// <summary>
        /// 获取窗口大小及位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// 3-最大化窗口，2-最小化窗口，1-正常大小窗口；
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 键盘事件，按下
        /// </summary>
        /// <param name="bVk">(byte)Keys.D</param>
        /// <param name="bScan">0</param>
        /// <param name="dwFlags"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,
            byte bScan,
            int dwFlags,  //这里是整数类型  0 为按下，2为释放
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0
        );

        public IntPtr GetWindows(string className, string title)
        {
            return FindWindow(className, title);
        }

        /// <summary>
        /// 获取指定窗口的左上角坐标
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public Point GetXYPoint(string className, string title)
        {
            IntPtr ptr = FindWindow(className, title);
            RECT rect = new RECT();

            // 得到左上角定点
            GetWindowRect(ptr, ref rect);
            int width = rect.Right - rect.Left; //窗口的宽度
            int height = rect.Bottom - rect.Top; //窗口的高度
            int x = rect.Left;
            int y = rect.Top;

            return new Point(x, y);
        }

        /// <summary>
        /// 设置窗体状态
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type">3-最大化窗口，2-最小化窗口，1-正常大小窗口；</param>
        /// <returns>返回操作结果</returns>
        public int SetWindowStatus(IntPtr handle, int type)
        {
            return ShowWindow(handle, type);
        }

        /// <summary>
        /// 鼠标单击某点
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public void Click(int rX, int rY)
        {
            SetCursorPos(rX, rY);

            // 使用组合构造一次单击
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, rX, rY, 0, 0);
        }

        /// <summary>
        /// 鼠标双击某点
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public void DoubleClick(int rX, int rY)
        {
            SetCursorPos(rX, rY);

            // 使用组合构造一次单击
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, rX, rY, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, rX, rY, 0, 0);
        }

        public void KeyDown(Keys keys)
        {
            keybd_event((byte)keys, 0, 0, 0);
            keybd_event((byte)keys, 0, 2, 0);
        }

        public void KeyDown(Keys key1, Keys key2)
        {
            keybd_event((byte)key1, 0, 0, 0);
            keybd_event((byte)key2, 0, 0, 0);
            
            keybd_event((byte)key1, 0, 2, 0);
            keybd_event((byte)key2, 0, 2, 0);
        }
    }
}
