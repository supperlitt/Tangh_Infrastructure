using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Update
{
    public class Test
    {
        public static void TestUpdate()
        {
            string update_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Update.exe");
            Process.Start(new ProcessStartInfo(update_path, "http://xxxx/xxxx.exe " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XXX.exe")));
            Process.Start(new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kill.bat")));
        }
    }
}
