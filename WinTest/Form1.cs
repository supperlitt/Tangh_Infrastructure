using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MonitorManager.AddType(typeof(TestData1));
            MonitorManager.AddType(typeof(TestDataModel));
            MonitorManager.AddType(typeof(TestBigModel));

            var tree = MonitorManager.ReadTree("root");

            this.treeView1.Nodes.Add(tree);
        }
    }
}
