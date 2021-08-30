using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SupperlittWin
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtendMethod
    {
        /// <summary>
        /// 绑定拖动
        /// </summary>
        /// <param name="tb"></param>
        public static void BindDrap(this TextBox tb)
        {
            tb.AllowDrop = true;
            tb.DragEnter += (object sender, DragEventArgs e) =>
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

                (sender as TextBox).Text = path;
            };
        }

        /// <summary>
        /// 绑定载入和保存
        /// </summary>
        public static void BindLoadAndSave(this TextBox tb, Form frm)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string temp_path = Path.Combine(dir, tb.Name + ".txt");
            tb.TextChanged += (a, b) =>
            {
                File.WriteAllText(temp_path, tb.Text, new UTF8Encoding(false));
            };

            frm.Load += (a, b) =>
            {
                if (File.Exists(temp_path))
                {
                    string str = File.ReadAllText(temp_path, new UTF8Encoding(false));
                    tb.Text = str;
                }
            };
        }
    }
}
