namespace TestControl
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.fileBrowse1 = new Tangh.Controls.BaseControl.FileBrowse();
            this.quickListView1 = new Tangh.Controls.QuickListView();
            this.onlineLabel1 = new Tangh.Controls.OnlineLabel();
            this.timeStatusStrip1 = new Tangh.Controls.TimeStatusStrip();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.SuspendLayout();
            // 
            // fileBrowse1
            // 
            this.fileBrowse1.Location = new System.Drawing.Point(12, 127);
            this.fileBrowse1.Name = "fileBrowse1";
            this.fileBrowse1.Size = new System.Drawing.Size(322, 37);
            this.fileBrowse1.TabIndex = 3;
            // 
            // quickListView1
            // 
            this.quickListView1.Location = new System.Drawing.Point(12, 12);
            this.quickListView1.Name = "quickListView1";
            this.quickListView1.Size = new System.Drawing.Size(121, 97);
            this.quickListView1.TabIndex = 2;
            this.quickListView1.UseCompatibleStateImageBehavior = false;
            // 
            // onlineLabel1
            // 
            this.onlineLabel1.AutoSize = true;
            this.onlineLabel1.Location = new System.Drawing.Point(29, 29);
            this.onlineLabel1.Name = "onlineLabel1";
            this.onlineLabel1.Size = new System.Drawing.Size(0, 12);
            this.onlineLabel1.TabIndex = 1;
            // 
            // timeStatusStrip1
            // 
            this.timeStatusStrip1.Location = new System.Drawing.Point(0, 300);
            this.timeStatusStrip1.Name = "timeStatusStrip1";
            this.timeStatusStrip1.Size = new System.Drawing.Size(346, 22);
            this.timeStatusStrip1.TabIndex = 0;
            this.timeStatusStrip1.Text = "timeStatusStrip1";
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.Location = new System.Drawing.Point(12, 227);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.Size = new System.Drawing.Size(120, 21);
            this.domainUpDown1.TabIndex = 4;
            this.domainUpDown1.Text = "domainUpDown1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 322);
            this.Controls.Add(this.domainUpDown1);
            this.Controls.Add(this.fileBrowse1);
            this.Controls.Add(this.quickListView1);
            this.Controls.Add(this.onlineLabel1);
            this.Controls.Add(this.timeStatusStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tangh.Controls.TimeStatusStrip timeStatusStrip1;
        private Tangh.Controls.OnlineLabel onlineLabel1;
        private Tangh.Controls.QuickListView quickListView1;
        private Tangh.Controls.BaseControl.FileBrowse fileBrowse1;
        private System.Windows.Forms.DomainUpDown domainUpDown1;
    }
}

