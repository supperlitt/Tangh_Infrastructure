using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangh.NewInfrastructure.Win32Api
{
    public class WindowsAPIHelper
    {
        // Fields
        protected const uint MEM_COMMIT = 0x1000;
        protected const uint MEM_RELEASE = 0x8000;
        protected const uint MEM_RESERVE = 0x2000;
        protected const uint PAGE_READWRITE = 4;
        protected const uint PROCESS_VM_OPERATION = 8;
        protected const uint PROCESS_VM_READ = 0x10;
        protected const uint PROCESS_VM_WRITE = 0x20;

        // Methods
        [DllImport("kernel32.dll")]
        protected static extern bool CloseHandle(int handle);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string strClassName, string strWindowName);
        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string className, string windowName);
        public int GetProcessId(int hwnd)
        {
            int processId = 0;
            GetWindowThreadProcessId(hwnd, out processId);
            return processId;
        }

        [DllImport("user32.dll")]
        protected static extern int GetWindowThreadProcessId(int hwnd, out int processId);
        public int InjectProcess(int processId)
        {
            return OpenProcess(0x38, false, processId);
        }

        [DllImport("kernel32.dll")]
        protected static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll")]
        protected static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);
        [DllImport("user32.DLL")]
        protected static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        [DllImport("kernel32.dll")]
        protected static extern int VirtualAllocEx(int hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        protected static extern bool VirtualFreeEx(int hProcess, int lpAddress, uint dwSize, uint dwFreeType);
        [DllImport("kernel32.dll")]
        protected static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);
    }



}
