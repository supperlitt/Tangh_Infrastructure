using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinTest
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class MonitorAttribute : Attribute
    {
        public MonitorAttribute()
        {
        }
    }
}
