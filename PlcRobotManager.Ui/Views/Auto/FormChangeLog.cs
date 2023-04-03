using PlcRobotManager.Core.Vendor.Mitsubishi;
using PlcRobotManager.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlcRobotManager.Core.Impl;

namespace PlcRobotManager.Ui.Views.Auto
{
    public partial class FormChangeLog: Form
    {
        public FormChangeLog()
        {
            InitializeComponent();
        }

        private void RefreshGrid()
        {
            this.InvokeSafe(() =>
            {
                gridControl1.RefreshDataSource();
            });
        }

        public Dictionary<string, List<ValueChangeLog>> RobotValueChangeLogs = new Dictionary<string, List<ValueChangeLog>>();
        public List<ValueChangeLog> SelectedRobotValueLogs { get; set; } = new List<ValueChangeLog>();
        public List<string> Robots { get; set; } = new List<string>();

        public string SelectedRobot { get; set; }
        public IRobotManager RobotManager = Program.RobotManager;

        public Dictionary<string, List<DeviceLabel>> RobotLabels { get; set; } = new Dictionary<string, List<DeviceLabel>>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            gridControl1.DataSource = SelectedRobotValueLogs;
            gridControl2.DataSource = Robots;

            gridView2.FocusedRowChanged += GridView2_FocusedRowChanged;

            Robots.AddRange(RobotManager.Robots.Select(x => x.Name));

            foreach (var robot in Robots)
                RobotValueChangeLogs.Add(robot, new List<ValueChangeLog>());

            RobotManager.RobotValueChanged += (s, ve) =>
            {
                var newLog = new ValueChangeLog()
                {
                    Changed = DateTime.Now,
                    Code = ve.ValueChangeEventArgs.Code,
                    Prev = ve.ValueChangeEventArgs.Prev,
                    Current = ve.ValueChangeEventArgs.Current,
                };
                RobotValueChangeLogs[ve.Name].Add(newLog);

                if(SelectedRobot == ve.Name)
                {
                    SelectedRobotValueLogs.Add(newLog);
                    RefreshGrid();
                }
            };

            foreach (var robot in Robots)
            {
                var plc = RobotManager.GetPlcNames(robot).First();
                RobotLabels.Add(robot, RobotManager.GetDeviceLabels(robot, plc));
            }

            gridControl2.RefreshDataSource();
            gridView2.FocusedRowHandle = 0;

        }

        private void GridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var robotName = gridView2.GetRow(e.FocusedRowHandle) as string;
            if (robotName != null)
            {
                SelectedRobot = robotName;
                SelectedRobotValueLogs.Clear();

                SelectedRobotValueLogs.AddRange(RobotValueChangeLogs[robotName]);
            }
        }

      
    }
}
