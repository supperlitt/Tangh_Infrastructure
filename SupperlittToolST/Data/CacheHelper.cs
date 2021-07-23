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
    /// 初始化缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheHelper<T> where T : class
    {
        /*
        private List<T> dataList = new List<T>();
        private string savePath = string.Empty;
        private string KeyId = string.Empty;
        private JavaScriptSerializer js = new JavaScriptSerializer();

        /// <summary>
        /// 初始化缓存存储位置和主键多个组合主键逗号分隔
        /// </summary>
        /// <param name="baseDir">初始化缓存存储位置：文件夹</param>
        /// <param name="KeyId">主键多个组合主键逗号分隔</param>
        public CacheHelper(string baseDir, string KeyId = null)
        {
            this.KeyId = KeyId;
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            savePath = Path.Combine(baseDir, typeof(T).Name + ".cache.txt");
            if (File.Exists(savePath))
            {
                string[] lines = File.ReadAllLines(savePath, new UTF8Encoding(false));
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

        public void Add(T model)
        {
            lock (dataList)
            {
                dataList.Add(model);
                SaveFile();
            }
        }

        public void AddList(List<T> list)
        {
            lock (dataList)
            {
                dataList.AddRange(list);
                SaveFile();
            }
        }

        public void AddOrUpdate(T model, string[] args)
        {
            lock (dataList)
            {
                T equalModel = CheckKeyEqual(dataList, model);
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

        public T QueryInfo(T model, Dictionary<string, string> condition)
        {
            lock (dataList)
            {
                List<FileDbInfo> infoList = FindArgsDbList(model, args);
                T equalModel = CheckKeyEqual(model, infoList, args);

                return equalModel;
            }
        }

        public List<T> QueryList(int page, int pageSize, T model, params string[] args)
        {
            lock (dataList)
            {
                List<FileDbInfo> infoList = FindArgsDbList(model, args);
                List<T> equalList = CheckArgsEqualList(model, infoList, args);

                return equalList.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            }
        }

        private void Update(T oldModel, T newModel, string[] args)
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

        private T CheckKeyEqual(List<T> dataAll, T model, string[] args = null)
        {
            if (string.IsNullOrEmpty(KeyId))
            {
                return model;
            }

            string[] fieldList = KeyId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string modelKeyValue = ReadString(model, fieldList.ToArray());
            foreach (var info in dataAll)
            {
                string keyValue = ReadString(info, fieldList.ToArray());
                if (modelKeyValue == keyValue)
                {
                    // 匹配上了
                    return info;
                }
            }

            return null;
        }

        private List<T> CheckArgsEqualList(T model, List<FileDbInfo> infoList, string[] args = null)
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

        private List<FileDbInfo> FindArgsDbList(T model, params string[] args)
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

        private List<string> ReadKeyName()
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

        private List<string> ReadArgsName()
        {
            List<string> keyList = new List<string>();
            foreach (var pInfo in typeof(T).GetProperties())
            {
                keyList.Add(pInfo.Name);
            }

            return keyList;
        }

        private string ReadString(T model, string key_name)
        {
            PropertyInfo propertyInfo = model.GetType().GetProperty(key_name);
            object obj = propertyInfo.GetValue(model, null);
            if (propertyInfo.PropertyType == typeof(int))
            {
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

        private string ReadString(T model, string[] fieldArray)
        {
            List<string> list = new List<string>();
            foreach (var field_name in fieldArray)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(field_name);
                object obj = propertyInfo.GetValue(model, null);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    list.Add(obj.ToString());
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    list.Add(obj.ToString());
                }
                else
                {
                    list.Add(obj.ToString());
                }
            }

            return string.Join("%%%", list.ToArray());
        }

        private void SaveFile()
        {
            StringBuilder content = new StringBuilder();
            foreach (var item in dataList)
            {
                content.AppendLine(js.Serialize(item));
            }

            File.WriteAllText(savePath, content.ToString(), new UTF8Encoding(false));
        }

        */
    }
}
