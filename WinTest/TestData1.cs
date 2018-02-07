using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinTest
{
    public class TestData1
    {
        private static List<string> list = new List<string>();

        private static List<int> listInt = new List<int>();

        private static List<TestDataModel> listData = new List<TestDataModel>();

        static TestData1()
        {
            list.AddRange(new string[] { "test_date1_01", "test_date1_02", "test_date1_03", "test_date1_04" }); ;
            listInt.AddRange(new int[] { 0, 1, 2, 3, 4, 5, 6 });
            listData.Add(new TestDataModel() { Id = 1, Name = "test1", Value = 1.1m, UpdateTime = DateTime.Now });
            listData.Add(new TestDataModel() { Id = 2, Name = "test2", Value = 1.2m, UpdateTime = DateTime.Now.AddSeconds(2) });
            listData.Add(new TestDataModel() { Id = 3, Name = "test3", Value = 1.3m, UpdateTime = DateTime.Now.AddSeconds(3) });
            listData.Add(new TestDataModel() { Id = 4, Name = "test4", Value = 1.4m, UpdateTime = DateTime.Now.AddSeconds(4) });
        }

        public static List<TestDataModel> GetAll()
        {
            return listData;
        }
    }
    public class TestDataModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public DateTime UpdateTime { get; set; }

        private List<TestBigModel> bigModel = new List<TestBigModel>();

        public List<TestBigModel> BigModel
        {
            get { return this.bigModel; }
            set { this.bigModel = value; }
        }

        public TestBigModel TestModel { get; set; }
    }

    public class TestBigModel
    {
        public int Key { get; set; }
    }
}
