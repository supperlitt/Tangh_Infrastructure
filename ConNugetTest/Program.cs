using SupperlittTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConNugetTest
{
    class Program
    {
        //private static CacheHelper<UserInfo> helper = new CacheHelper<UserInfo>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"), "id");

        static void Main(string[] args)
        {
            //  helper.Add(new UserInfo() { id = 1, name = "test1", pwd = "ttt", age = 20 });
            //  helper.Add(new UserInfo() { id = 2, name = "test2", pwd = "ttt2", age = 21 });
            //  helper.Add(new UserInfo() { id = 3, name = "test3", pwd = "ttt3", age = 22 });
            //  helper.Add(new UserInfo() { id = 4, name = "test4", pwd = "ttt4", age = 23 });
            // 
            //  helper.QueryInfo("");
        }
    }

    public class UserInfo
    {
        public int id { get; set; }

        public string name { get; set; }

        public string pwd { get; set; }

        public int age { get; set; }
    }
}
