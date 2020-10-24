using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Xml;
using System.Security.AccessControl;

namespace Update
{
    public partial class UpdateForm : Form
    {
        private string fileUrl = string.Empty;

        private string fileName = string.Empty;

        public UpdateForm()
        {
            InitializeComponent();
        }

        public UpdateForm(string filestring, string fileName)
        {
            InitializeComponent();
            this.fileUrl = filestring;
            this.fileName = fileName;
        }

        /// <summary>        
        /// c#,.net 下载文件        
        /// </summary>        
        /// <param name="URL">下载文件地址</param>       
        /// <param name="filename">下载后的存放地址</param>
        /// <param name="prog">用于显示的进度条</param>
        /// <param name="maxValue">文件总个数</param>
        public void DownloadFile(string URL, string filename, System.Windows.Forms.ProgressBar prog, int maxValue)
        {
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);

                    // 计算百分比
                    float percent = (float)totalDownloadedByte / (float)totalBytes;
                    this.Invoke(new Action<ProgressBar>(p => p.Value = (int)(maxValue * percent)), prog);

                    osize = st.Read(by, 0, (int)by.Length);

                    //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                    System.Windows.Forms.Application.DoEvents();
                }

                so.Close();
                st.Close();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            if (!string.IsNullOrEmpty(fileUrl))
            {
                this.progressBar1.Maximum = 1000;
                Thread thread = new Thread(new ParameterizedThreadStart(ThreadDownLoad));
                thread.IsBackground = true;
                thread.Start(fileUrl);
            }
            else
            {
                this.progressBar1.Value = this.progressBar1.Maximum;
                this.label1.Text = "更新完成.";
            }
        }

        private void ThreadDownLoad(object obj)
        {
            string url = obj as string;
            string saveFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".exe");

            // 下载文件
            DownloadFile(url, saveFileName, this.progressBar1, 1000);
            Thread.Sleep(500);

            string version = url.Substring(url.LastIndexOf("/") + 1, url.Length - 9 - url.LastIndexOf("/"));
            // 删除原来的文件

            FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.fileName));
            System.Security.AccessControl.FileSecurity fileSecurity = fi.GetAccessControl();
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            fi.SetAccessControl(fileSecurity);  

            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.fileName));
            Thread.Sleep(1500);

            // 修改下载文件的名称
            FileInfo f = new FileInfo(saveFileName);
            fileSecurity = f.GetAccessControl();
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            f.SetAccessControl(fileSecurity);  
            f.MoveTo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.fileName));

            Thread.Sleep(1000);

            // 启动原来的文件
            //UpdateConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.fileName) + ".config", "version", version);

            if (!string.IsNullOrEmpty(fileName))
            {
                // path
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                Process.Start(new ProcessStartInfo()
                {
                    FileName = path
                });
            }

            Application.ExitThread();
            Application.Exit();
        }
    }
}
