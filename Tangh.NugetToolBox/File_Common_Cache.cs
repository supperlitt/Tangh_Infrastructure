using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tangh.NugetToolBox
{
    public class File_Common_Cache<T> where T : new()
    {
        private static readonly string SEP_STR = "---";

        private static List<T> dataList = new List<T>();

        private static string cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", typeof(T).Name.ToString() + ".txt");

        static File_Common_Cache()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(cachePath))
            {
                Type t = typeof(T);
                string[] lines = File.ReadAllLines(cachePath, Encoding.UTF8);
                foreach (var line in lines)
                {
                    string[] lineArray = line.Split(new string[] { SEP_STR }, StringSplitOptions.None);
                    if (line.Contains(SEP_STR))
                    {
                        List<PropertyIndexInfo> list = new List<PropertyIndexInfo>();
                        T model = new T();
                        PropertyInfo[] ps = t.GetProperties();
                        foreach (var p in ps)
                        {
                            var ads = p.GetCustomAttributesData();
                            if (ads.Count > 0)
                            {
                                int index = Convert.ToInt32(ads[0].NamedArguments[0].TypedValue.Value);
                                list.Add(new PropertyIndexInfo() { Index = index, PropertyInfo = p });
                            }
                        }

                        list = list.OrderBy(p => p.Index).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            var pt = list[i].PropertyInfo.PropertyType;
                            if (pt == typeof(int))
                            {
                                list[i].PropertyInfo.SetValue(model, Convert.ToInt32(lineArray[i]), null);
                            }
                            else if (pt == typeof(string))
                            {
                                list[i].PropertyInfo.SetValue(model, lineArray[i], null);
                            }
                            else if (pt == typeof(DateTime))
                            {
                                list[i].PropertyInfo.SetValue(model, Convert.ToDateTime(lineArray[i]), null);
                            }
                            else
                            {
                                try
                                {
                                    list[i].PropertyInfo.SetValue(model, (object)lineArray[i], null);
                                }
                                catch
                                {
                                    throw new Exception("不支持属性类型(仅支持，int,string,DateTime,object)");
                                }
                            }
                        }

                        dataList.Add(model);
                    }
                }
            }
        }

        public static void Add(T t)
        {
            lock (dataList)
            {
                dataList.Add(t);

                AppendFile(t);
            }
        }

        public static void Remove(T t)
        {

        }

        public static List<T> GetAll()
        {
            lock (dataList)
            {
                return dataList;
            }
        }

        private static void WriteFile()
        {
            StringBuilder content = new StringBuilder();
            foreach (var item in dataList)
            {
                List<CacheIndexInfo> list = new List<CacheIndexInfo>();
                var ps = typeof(T).GetProperties();
                foreach (var p in ps)
                {
                    var ads = p.GetCustomAttributesData();
                    if (ads.Count > 0)
                    {
                        int index = Convert.ToInt32(ads[0].NamedArguments[0].TypedValue.Value);
                        object p_object = p.GetValue(item, null);
                        string value = string.Empty;
                        if (p.PropertyType == typeof(DateTime))
                        {
                            value = p_object == null ? DateTime.Parse("1900-1-1").ToString("yyyy-MM-dd HH:mm:ss") :
                                Convert.ToDateTime(p_object).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            value = p_object == null ? "" : p_object.ToString();
                        }

                        list.Add(new CacheIndexInfo() { Index = index, Value = value });
                    }
                }

                list = list.OrderBy(a => a.Index).ToList();
                content.AppendLine(string.Join(SEP_STR, (from f in list select f.Value).ToArray()));
            }

            File.WriteAllText(cachePath, content.ToString(), Encoding.UTF8);
        }

        private static void AppendFile(T t)
        {
            StringBuilder content = new StringBuilder();
            List<CacheIndexInfo> list = new List<CacheIndexInfo>();
            var ps = typeof(T).GetProperties();
            foreach (var p in ps)
            {
                var ads = p.GetCustomAttributesData();
                if (ads.Count > 0)
                {
                    int index = Convert.ToInt32(ads[0].NamedArguments[0].TypedValue.Value);
                    object p_object = p.GetValue(t, null);
                    string value = string.Empty;
                    if (p.PropertyType == typeof(DateTime))
                    {
                        value = p_object == null ? DateTime.Parse("1900-1-1").ToString("yyyy-MM-dd HH:mm:ss") :
                            Convert.ToDateTime(p_object).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        value = p_object == null ? "" : p_object.ToString();
                    }

                    list.Add(new CacheIndexInfo() { Index = index, Value = value });
                }
            }

            list = list.OrderBy(a => a.Index).ToList();
            content.AppendLine(string.Join(SEP_STR, (from f in list select f.Value).ToArray()));
            File.AppendAllText(cachePath, content.ToString(), Encoding.UTF8);
        }
    }

    public class CacheOrderAttribute : Attribute
    {
        public int Index { get; set; }
    }

    public class CacheIndexInfo
    {
        public int Index { get; set; }

        public string Value { get; set; }
    }

    public class PropertyIndexInfo
    {
        public int Index { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
