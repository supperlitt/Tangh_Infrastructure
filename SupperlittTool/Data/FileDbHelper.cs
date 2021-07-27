using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace SupperlittTool
{
    /// <summary>
    /// 文件系统数据库
    /// 主键Attribute [Key]，支持主键比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FileDbHelper<T> where T : class
    {
        private static List<T> dataList = new List<T>();
        private static string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", typeof(T).Name + ".txt");
        private static JavaScriptSerializer js = new JavaScriptSerializer();

        static FileDbHelper()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", typeof(T).Name + ".txt");
            if (File.Exists(savePath))
            {
                string[] lines = File.ReadAllLines(savePath, Encoding.UTF8);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            dataList.Add(js.Deserialize<T>(line));
                        }
                        catch { }
                    }
                }
            }
        }

        public static void Add(T model)
        {
            lock (dataList)
            {
                dataList.Add(model);
                SaveFile();
            }
        }

        public static void AddList(List<T> list)
        {
            lock (dataList)
            {
                dataList.AddRange(list);
                SaveFile();
            }
        }

        public static void AddOrUpdate(T model, params string[] args)
        {
            lock (dataList)
            {
                List<FileDbInfo> infoList = FindKeyDbList(model, args);
                T equalModel = CheckKeyEqual(model, infoList, args);
                if (equalModel == null)
                {
                    Add(model);
                }
                else
                {
                    Update(equalModel, model, args);
                }

                SaveFile();
            }
        }

        public static T QueryInfo(T model, params string[] args)
        {
            lock (dataList)
            {
                List<FileDbInfo> infoList = FindArgsDbList(model, args);
                T equalModel = CheckKeyEqual(model, infoList, args);

                return equalModel;
            }
        }

        public static List<T> QueryList(int page, int pageSize, T model, params string[] args)
        {
            lock (dataList)
            {
                List<FileDbInfo> infoList = FindArgsDbList(model, args);
                List<T> equalList = CheckArgsEqualList(model, infoList, args);

                return equalList.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            }
        }

        private static void Update(T oldModel, T newModel, string[] args)
        {
            List<string> keyList = ReadKeyName();
            foreach (PropertyInfo pInfo in newModel.GetType().GetProperties())
            {
                if (args.Length > 0)
                {
                    if (!args.ToList().Contains(pInfo.Name))
                    {
                        Object new_value = pInfo.GetValue(newModel, null);
                        pInfo.SetValue(oldModel, new_value, null);
                    }
                }
                else
                {
                    if (!keyList.Contains(pInfo.Name))
                    {
                        Object new_value = pInfo.GetValue(newModel, null);
                        pInfo.SetValue(oldModel, new_value, null);
                    }
                }
            }
        }

        private static T CheckKeyEqual(T model, List<FileDbInfo> infoList, string[] args = null)
        {
            foreach (var item in dataList)
            {
                bool is_equal = true;
                foreach (var info in infoList)
                {
                    if (args == null || (args != null && args.ToList().Contains(info.key)))
                    {
                        if (ReadString(model, info.key) != ReadString(item, info.key))
                        {
                            is_equal = false;
                            break;
                        }
                    }
                }

                if (is_equal)
                {
                    return item;
                }
            }

            return null;
        }

        private static List<T> CheckArgsEqualList(T model, List<FileDbInfo> infoList, string[] args = null)
        {
            List<T> result = new List<T>();
            foreach (var item in dataList)
            {
                bool is_equal = true;
                foreach (var info in infoList)
                {
                    if (args == null || (args != null && args.ToList().Contains(info.key)))
                    {
                        if (!string.IsNullOrEmpty(info.str_value) && ReadString(model, info.key) != ReadString(item, info.key))
                        {
                            is_equal = false;
                            break;
                        }
                    }
                }

                if (is_equal)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private static List<FileDbInfo> FindKeyDbList(T model, params string[] args)
        {
            List<string> keyList = ReadKeyName();
            var infoList = new List<FileDbInfo>();
            foreach (var key in keyList)
            {
                if (args == null || (args != null && args.ToList().Contains(key)))
                {
                    infoList.Add(new FileDbInfo(key, ReadString(model, key)));
                }
            }

            return infoList;
        }

        private static List<FileDbInfo> FindArgsDbList(T model, params string[] args)
        {
            List<string> keyList = ReadArgsName();
            var infoList = new List<FileDbInfo>();
            foreach (var key in keyList)
            {
                if (args == null || (args != null && args.ToList().Contains(key)))
                {
                    infoList.Add(new FileDbInfo(key, ReadString(model, key)));
                }
            }

            return infoList;
        }

        private static List<string> ReadKeyName()
        {
            List<string> keyList = new List<string>();
            foreach (var pInfo in typeof(T).GetProperties())
            {
                foreach (var attr in pInfo.GetCustomAttributes(false))
                {
                    if (attr.GetType() == typeof(KeyAttribute))
                    {
                        keyList.Add(pInfo.Name);
                    }
                }
            }

            if (keyList.Count == 0)
            {
                throw new Exception("未定义主键,通过设置注解来添加主键[Key],目前只支持int和string");
            }

            return keyList;
        }

        private static List<string> ReadArgsName()
        {
            List<string> keyList = new List<string>();
            foreach (var pInfo in typeof(T).GetProperties())
            {
                keyList.Add(pInfo.Name);
            }

            return keyList;
        }

        private static string ReadString(T model, string key_name)
        {
            PropertyInfo propertyInfo = model.GetType().GetProperty(key_name);
            object obj = propertyInfo.GetValue(model, null);
            if (propertyInfo.PropertyType == typeof(int))
            {
                // 如果-1表示不判断，这里特殊处理一下-1
                if (obj.ToString() == "-1")
                {
                    return string.Empty;
                }

                return obj.ToString();
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                return obj.ToString();
            }
            else
            {
                return obj.ToString();
            }
        }

        private static void SaveFile()
        {
            StringBuilder content = new StringBuilder();
            foreach (var item in dataList)
            {
                content.AppendLine(js.Serialize(item));
            }

            File.WriteAllText(savePath, content.ToString(), new UTF8Encoding(false));
        }
    }

    public class FileDbInfo
    {
        public FileDbInfo(string key, string str_value)
        {
            this.key = key;
            this.str_value = str_value;
        }

        public string key { get; set; }

        public string str_value { get; set; }
    }

    public class KeyAttribute : Attribute { }
}
