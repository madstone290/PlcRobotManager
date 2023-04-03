namespace PlcRobotManager.Ui
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
            this.tabbedView1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.미쓰비시ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.자동원본ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.오토ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.사이클타임ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.수동ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.값변경로그ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // documentManager1
            // 
            this.documentManager1.ContainerControl = this;
            this.documentManager1.View = this.tabbedView1;
            this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] {
            this.tabbedView1});
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.미쓰비시ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1384, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 미쓰비시ToolStripMenuItem
            // 
            this.미쓰비시ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.자동원본ToolStripMenuItem,
            this.오토ToolStripMenuItem,
            this.사이클타임ToolStripMenuItem,
            this.값변경로그ToolStripMenuItem,
            this.수동ToolStripMenuItem});
            this.미쓰비시ToolStripMenuItem.Name = "미쓰비시ToolStripMenuItem";
            this.미쓰비시ToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.미쓰비시ToolStripMenuItem.Text = "미쓰비시";
            // 
            // 자동원본ToolStripMenuItem
            // 
            this.자동원본ToolStripMenuItem.Name = "자동원본ToolStripMenuItem";
            this.자동원본ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.자동원본ToolStripMenuItem.Text = "자동_원본";
            this.자동원본ToolStripMenuItem.Click += new System.EventHandler(this.자동원본ToolStripMenuItem_Click);
            // 
            // 오토ToolStripMenuItem
            // 
            this.오토ToolStripMenuItem.Name = "오토ToolStripMenuItem";
            this.오토ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.오토ToolStripMenuItem.Text = "자동_가공";
            this.오토ToolStripMenuItem.Click += new System.EventHandler(this.오토ToolStripMenuItem_Click);
            // 
            // 사이클타임ToolStripMenuItem
            // 
            this.사이클타임ToolStripMenuItem.Name = "사이클타임ToolStripMenuItem";
            this.사이클타임ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.사이클타임ToolStripMenuItem.Text = "사이클타임";
            this.사이클타임ToolStripMenuItem.Click += new System.EventHandler(this.사이클타임ToolStripMenuItem_Click);
            // 
            // 수동ToolStripMenuItem
            // 
            this.수동ToolStripMenuItem.Name = "수동ToolStripMenuItem";
            this.수동ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.수동ToolStripMenuItem.Text = "수동";
            this.수동ToolStripMenuItem.Click += new System.EventHandler(this.수동ToolStripMenuItem_Click);
            // 
            // 값변경로그ToolStripMenuItem
            // 
            this.값변경로그ToolStripMenuItem.Name = "값변경로그ToolStripMenuItem";
            this.값변경로그ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.값변경로그ToolStripMenuItem.Text = "값변경로그";
            this.값변경로그ToolStripMenuItem.Click += new System.EventHandler(this.값변경로그ToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 661);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "FormMain";
            ((System.ComponentModel.ISupportInitialize)(this.documentManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Docking2010.DocumentManager documentManager1;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 미쓰비시ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 오토ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 수동ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 자동원본ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 사이클타임ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 값변경로그ToolStripMenuItem;
    }
}