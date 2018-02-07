using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangh.NugetToolBox;

namespace ConTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TInfo info = new TInfo();
            info.Name = "test";
            info.Age = 10;
            info.Test = "我是测试字符串";
            var list = File_Common_Cache<TInfo>.GetAll();
            DateTime startTime = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                File_Common_Cache<TInfo>.Add(info);
            }

            TimeSpan span = DateTime.Now - startTime;
            Console.WriteLine(span.TotalMilliseconds);
            Console.ReadLine();
        }
    }

    public class TInfo
    {
        [CacheOrder(Index = 0)]
        public string Name { get; set; }

        [CacheOrder(Index = 2)]
        public int Age { get; set; }

        [CacheOrder(Index = 1)]
        public string Test { get; set; }
    }
}
