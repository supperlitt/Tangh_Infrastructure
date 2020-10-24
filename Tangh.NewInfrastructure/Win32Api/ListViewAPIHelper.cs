using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangh.NewInfrastructure.Win32Api
{
    public class ListViewAPIHelper : WindowsAPIHelper
    {
        // Fields
        protected int HDI_TEXT = 2;
        protected const uint HDM_FIRST = 0x1200;
        protected const uint HDM_GETITEMA = 0x1203;
        protected const uint HDM_GETITEMCOUNT = 0x1200;
        protected const uint HDM_GETITEMW = 0x120b;
        protected int LVIF_TEXT = 1;
        protected const uint LVM_FIRST = 0x1000;
        protected const uint LVM_GETHEADER = 0x101f;
        protected const uint LVM_GETITEMCOUNT = 0x1004;
        protected const uint LVM_GETITEMTEXTA = 0x102d;
        protected const uint LVM_GETITEMTEXTW = 0x1073;

        // Methods
        public int GetColumnCount(int hwndHeader)
        {
            return WindowsAPIHelper.SendMessage(hwndHeader, 0x1200, 0, 0);
        }

        public List<string> GetColumnsHeaderText(int processHandle, int headerhwnd, int colCount)
        {
            List<string> list = new List<string>();
            uint dwSize = 0x100;
            int lpBaseAddress = WindowsAPIHelper.VirtualAllocEx(processHandle, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(HDITEM)), 0x3000, 4);
            int num3 = WindowsAPIHelper.VirtualAllocEx(processHandle, IntPtr.Zero, dwSize, 0x3000, 4);
            for (int i = 0; i < colCount; i++)
            {
                byte[] arr = new byte[dwSize];
                HDITEM structure = new HDITEM
                {
                    mask = (uint)this.HDI_TEXT,
                    fmt = 0,
                    cchTextMax = (int)dwSize,
                    pszText = (IntPtr)num3
                };
                IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
                Marshal.StructureToPtr(structure, ptr, false);
                uint vNumberOfBytesRead = 0;
                bool flag = WindowsAPIHelper.WriteProcessMemory(processHandle, lpBaseAddress, ptr, Marshal.SizeOf(typeof(HDITEM)), ref vNumberOfBytesRead);
                WindowsAPIHelper.SendMessage(headerhwnd, 0x1203, i, lpBaseAddress);
                WindowsAPIHelper.ReadProcessMemory(processHandle, num3, Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0), (int)dwSize, ref vNumberOfBytesRead);
                string str = Encoding.Default.GetString(arr, 0, (int)vNumberOfBytesRead);
                string item = "";
                foreach (char ch in str)
                {
                    if (ch == '\0')
                    {
                        break;
                    }
                    item = item + ch;
                }
                list.Add(item);
            }
            WindowsAPIHelper.VirtualFreeEx(processHandle, lpBaseAddress, 0, 0x8000);
            WindowsAPIHelper.VirtualFreeEx(processHandle, num3, 0, 0x8000);
            return list;
        }

        public int GetHeaderHwnd(int hwndListView)
        {
            return WindowsAPIHelper.SendMessage(hwndListView, 0x101f, 0, 0);
        }

        public string[,] GetItemCellsText(int processHandle, int hwndListView, int rows, int cols)
        {
            string[,] strArray = new string[rows, cols];
            uint dwSize = 0x100;
            int lpBaseAddress = WindowsAPIHelper.VirtualAllocEx(processHandle, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(HDITEM)), 0x3000, 4);
            int num3 = WindowsAPIHelper.VirtualAllocEx(processHandle, IntPtr.Zero, dwSize, 0x3000, 4);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    byte[] arr = new byte[dwSize];
                    LVITEM structure = new LVITEM
                    {
                        mask = this.LVIF_TEXT,
                        iItem = i,
                        iSubItem = j,
                        cchTextMax = (int)dwSize,
                        pszText = (IntPtr)num3
                    };
                    IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
                    Marshal.StructureToPtr(structure, ptr, false);
                    uint vNumberOfBytesRead = 0;
                    WindowsAPIHelper.WriteProcessMemory(processHandle, lpBaseAddress, ptr, Marshal.SizeOf(typeof(LVITEM)), ref vNumberOfBytesRead);
                    WindowsAPIHelper.SendMessage(hwndListView, 0x102d, i, lpBaseAddress);
                    WindowsAPIHelper.ReadProcessMemory(processHandle, num3, Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0), arr.Length, ref vNumberOfBytesRead);
                    string str = Encoding.Default.GetString(arr, 0, (int)vNumberOfBytesRead);
                    strArray[i, j] = str;
                }
            }
            WindowsAPIHelper.VirtualFreeEx(processHandle, lpBaseAddress, 0, 0x8000);
            WindowsAPIHelper.VirtualFreeEx(processHandle, num3, 0, 0x8000);
            return strArray;
        }

        public int GetRowCount(int hwndListView)
        {
            return WindowsAPIHelper.SendMessage(hwndListView, 0x1004, 0, 0);
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        protected class HDITEM
        {
            public uint mask;
            public int cxy;
            public IntPtr pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public int fmt;
            public int lParam;
            public int iImage;
            public int iOrder;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
        }
    }
}
