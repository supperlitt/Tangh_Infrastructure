using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace WinTest
{
    public class MonitorManager
    {
        private static List<Type> monitorType = new List<Type>();

        private static string separator = "----";

        public static void AddType(Type type)
        {
            monitorType.Add(type);
        }

        public static void SetSepartor(string _separator)
        {
            separator = _separator;
        }

        private static List<string> typeList = new List<string>();

        public static TreeNode ReadTree(string title)
        {
            typeList = new List<string>();
            foreach (var type in monitorType)
            {
                typeList.Add(type.FullName);
            }

            TreeNode root = new TreeNode(title);
            foreach (var type in monitorType)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
                foreach (FieldInfo f in fields)
                {
                    var data = f.GetValue(null);
                    string typeName = data.GetType().FullName;
                    TreeNode node = null;
                    if (data is IEnumerable)
                    {
                        int result = 0;
                        IEnumerator enumerator = (data as IEnumerable).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            result++;
                        }

                        node = new TreeNode(f.Name + separator + f.Name + ".Count:" + result);
                    }
                    else
                    {
                        node = new TreeNode(f.Name);
                    }

                    if (typeName.StartsWith("System.Collections.Generic.List`1[[System.String,"))
                    {
                        foreach (var m in data as List<string>)
                        {
                            TreeNode item = new TreeNode(m);
                            node.Nodes.Add(item);
                        }
                    }
                    else if (typeName.StartsWith("System.Collections.Generic.List`1[[System.Int32,"))
                    {
                        foreach (var m in data as List<int>)
                        {
                            TreeNode item = new TreeNode(m.ToString());
                            node.Nodes.Add(item);
                        }
                    }
                    else if (typeName.StartsWith("System.Collections.Generic.List`1[[") && typeList.Exists(p => typeName.StartsWith("System.Collections.Generic.List`1[[" + p)))
                    {
                        foreach (var m in (data as IEnumerable))
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            PropertyInfo[] ps = m.GetType().GetProperties();
                            foreach (var p in ps)
                            {
                                object obj = p.GetValue(m, null);
                                dic.Add(p.Name, obj);
                            }

                            StringBuilder content = new StringBuilder();
                            foreach (var c in dic)
                            {
                                content.AppendFormat("{0}:{1}{2}", c.Key, c.Value, separator);
                            }

                            if (content.Length > 0)
                            {
                                content.Remove(content.Length - separator.Length, separator.Length);
                            }

                            TreeNode itemNode = new TreeNode(content.ToString());
                            node.Nodes.Add(itemNode);
                        }
                    }
                    else if (typeList.Exists(p => typeName.StartsWith(p)))
                    {
                        var typeFullName = typeList.Find(p => typeName.StartsWith(p));
                    }

                    root.Nodes.Add(node);
                }
            }

            return root;
        }

        private static TreeNode GetChildTreeNode(object data, string name, ref object childObj)
        {
            string typeName = data.GetType().FullName;
            TreeNode node = null;
            if (data is IEnumerable)
            {
                int result = 0;
                IEnumerator enumerator = (data as IEnumerable).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    result++;
                }

                node = new TreeNode(name + separator + name + ".Count:" + result);
            }
            else
            {
                node = new TreeNode(name);
            }

            if (typeName.StartsWith("System.Collections.Generic.List`1[[System.String,"))
            {
                foreach (var m in data as List<string>)
                {
                    TreeNode item = new TreeNode(m);
                    node.Nodes.Add(item);
                }

                childObj = null;
            }
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[System.Int32,"))
            {
                foreach (var m in data as List<int>)
                {
                    TreeNode item = new TreeNode(m.ToString());
                    node.Nodes.Add(item);
                }

                childObj = null;
            }
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[") && typeList.Exists(p => typeName.StartsWith("System.Collections.Generic.List`1[[" + p)))
            {
                foreach (var m in (data as IEnumerable))
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    PropertyInfo[] ps = m.GetType().GetProperties();
                    foreach (var p in ps)
                    {
                        object obj = p.GetValue(m, null);
                        dic.Add(p.Name, obj);
                    }

                    StringBuilder content = new StringBuilder();
                    foreach (var c in dic)
                    {
                        content.AppendFormat("{0}:{1}{2}", c.Key, c.Value, separator);
                    }

                    if (content.Length > 0)
                    {
                        content.Remove(content.Length - separator.Length, separator.Length);
                    }

                    TreeNode itemNode = new TreeNode(content.ToString());
                    node.Nodes.Add(itemNode);
                }
            }
            else if (typeList.Exists(p => typeName.StartsWith(p)))
            {
                var typeFullName = typeList.Find(p => typeName.StartsWith(p));
            }

            return node;
        }
    }

    public class ObjectInfo
    {
        public string Name { get; set; }

        public string FullName { get; set; }
    }

    public class ValueInfo
    {
        public string Name { get; set; }

        public string FullName { get; set; }
    }
}
