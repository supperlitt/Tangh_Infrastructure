using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.timeStatusStrip1.Start();
            this.onlineLabel1.Start();
            this.quickListView1.InitColumns(new Dictionary<string, int>() { { "索引", 50 }, { "名称", 60 } });

            this.quickListView1.SetData(new List<string[]>() { new string[] { "0", "tes1" }, new string[] { "1", "test2" } });
        }
    }
}
