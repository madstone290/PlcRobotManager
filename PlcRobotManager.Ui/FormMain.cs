using PlcRobotManager.Ui.Views.Auto;
using PlcRobotManager.Ui.Views.Test;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlcRobotManager.Ui
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            documentManager1.MdiParent = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            documentManager1.View.AddDocument(new FormMitsubishiTest(), "수동");
        }

        private void 수동ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.View.AddDocument(new FormMitsubishiTest(), "수동");
        }

        private void 오토ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.View.AddDocument(new FormAutoProcessed(), "자동_가공");
        }

        private void 자동원본ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.View.AddDocument(new FormAutoRaw(), "자동_원본");
        }

        private void 사이클타임ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.View.AddDocument(new FormSubroutine(), "사이클타임");
        }

        private void 값변경로그ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.View.AddDocument(new FormChangeLog(), "값변경로그");
        }
    }
}
