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
        private static JavaScriptSerializer js = new JavaScriptSerializer();
        private List<T> dataList = new List<T>();
        private string savePath = string.Empty;
        private string KeyId = string.Empty;
        private bool is_save_cahce = true;

        /// <summary>
        /// 初始化缓存存储位置和主键多个组合主键逗号分隔
        /// </summary>
        /// <param name="baseDir">初始化缓存存储位置：文件夹</param>
        /// <param name="KeyId">主键多个组合主键逗号分隔</param>
        /// <param name="is_save_cahce">是否保存缓存，重启继续有效</param>
        public CacheHelper(string baseDir, string KeyId = null, bool is_save_cahce = true)
        {
            this.KeyId = KeyId;
            this.is_save_cahce = is_save_cahce;
            if (is_save_cahce)
            {
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
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="model"></param>
        /// <param name="check_exist">是否检查以及存在，存在则更新</param>
        public void Add(T model, bool check_exist = true)
        {
            lock (dataList)
            {
                if (check_exist)
                {
                    var t = CheckKeyEqual(dataList, model);
                    if (t == null)
                    {
                        dataList.Add(model);
                    }
                    else
                    {
                        t = model;
                    }
                }
                else
                {
                    dataList.Add(model);
                }

                SaveFile();
            }
        }

        /// <summary>
        /// 添加多个成员
        /// </summary>
        /// <param name="list"></param>
        /// <param name="check_exist">是否检查以及存在，存在则更新</param>
        public void AddList(List<T> list, bool check_exist = true)
        {
            lock (dataList)
            {
                if (check_exist)
                {
                    foreach (var item in list)
                    {
                        var t = CheckKeyEqual(dataList, item);
                        if (t == null)
                        {
                            dataList.Add(item);
                        }
                        else
                        {
                            t = item;
                        }
                    }
                }
                else
                {
                    dataList.AddRange(list);
                }

                SaveFile();
            }
        }

        /// <summary>
        /// 查询单个对象，查询条件
        /// </summary>
        /// <param name="query_str">
        /// a:int >= b
        /// a:string = b
        /// a:int <= b
        /// a:datetime > b
        /// a:datetime != b
        /// a:datetime < b
        /// a:datetime >=b & c:datetime<=a
        /// </param>
        /// <returns></returns>
        public T QueryInfo(string query_str)
        {
            List<QueryItem> queryList = new List<QueryItem>();
            string[] array = query_str.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            string[] calcArray = new string[] { ">=", "<=", "!=", "=", ">", "<" };
            foreach (var str in array)
            {
                QueryItem item = null;
                foreach (var calc in calcArray)
                {
                    if (str.Contains(calc))
                    {
                        item = new QueryItem(calc);
                        break;
                    }
                }

                if (item != null)
                {
                    string[] data_array = str.Split(new string[] { item.compare }, StringSplitOptions.None);
                    if (data_array.Length == 2)
                    {
                        var nameArray = data_array[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameArray.Length == 2)
                        {
                            item.field = nameArray[0];
                            item.type = nameArray[1];
                        }

                        item.value = data_array[1];
                    }
                }

                if (item != null)
                {
                    queryList.Add(item);
                }
            }

            lock (dataList)
            {
                foreach (var obj in dataList)
                {
                    bool is_match = true;
                    foreach (var query in queryList)
                    {
                        object obj_value = obj.GetType().GetProperty(query.field).GetValue(obj);
                        if (query.type == "int")
                        {
                            if (query.compare == ">=")
                            {
                            }
                            else if (query.compare == "<=")
                            {

                            }
                            else if (query.compare == "!=")
                            {

                            }
                            else if (query.compare == "=")
                            {
                                if (query.value.ToInt32() != Convert.ToInt32(obj_value))
                                {
                                    is_match = false;
                                    break;
                                }
                            }
                            else if (query.compare == ">")
                            {

                            }
                            else if (query.compare == "<")
                            {

                            }
                        }
                        else if (query.type == "long")
                        {
                            if (query.value.ToInt64() != Convert.ToInt64(obj_value))
                            {
                                is_match = false;
                                break;
                            }
                        }
                        else if (query.value == "string")
                        {
                            if (query.value != obj_value.ToString())
                            {
                                is_match = false;
                                break;
                            }
                        }
                        else if (query.type == "datetime")
                        {
                            if (query.value.ToDateTime() != Convert.ToDateTime(obj_value))
                            {
                                is_match = false;
                                break;
                            }
                        }
                    }

                    if (is_match)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        public List<T> QueryList(string query_str, int page, int pageSize)
        {
            lock (dataList)
            {
                List<FileDbInfo> infoList = FindArgsDbList(model, args);
                List<T> equalList = CheckArgsEqualList(model, infoList, args);

                return equalList.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            }
        }

        public void Update(T newModel)
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
            if (is_save_cahce)
            {
                StringBuilder content = new StringBuilder();
                foreach (var item in dataList)
                {
                    content.AppendLine(js.Serialize(item));
                }

                File.WriteAllText(savePath, content.ToString(), new UTF8Encoding(false));
            }
        }

        */
    }

    public class QueryItem
    {
        public string field { get; set; }

        public string type { get; set; }

        public string value { get; set; }

        public string compare { get; set; }

        public QueryItem()
        {
        }

        public QueryItem(string compare)
        {
            this.compare = compare;
        }
    }
}
