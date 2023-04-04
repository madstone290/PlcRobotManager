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
    public partial class FormSubroutine : Form
    {
        private System.Timers.Timer cycleTimer = new System.Timers.Timer();

        public FormSubroutine()
        {
            InitializeComponent();
            cycleTimer.Interval = 100;
            cycleTimer.Elapsed += (s, e) =>
            {
                foreach (var value in PlcValues)
                    value.UpdateTime();

                RefreshGrid();
            };
            cycleTimer.Start();
        }

        private void RefreshGrid()
        {
            this.InvokeSafe(() =>
            {
                gridControl1.RefreshDataSource();
            });
        }

        public List<PlcSubroutineValue> PlcValues { get; set; } = new List<PlcSubroutineValue>();
        public List<string> Robots { get; set; } = new List<string>();

        public string SelectedRobot { get; set; }
        public IRobotManager RobotManager = Program.RobotManager;

        public Dictionary<string, List<DeviceLabel>> RobotLabels { get; set; } = new Dictionary<string, List<DeviceLabel>>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            gridControl1.DataSource = PlcValues;
            gridControl2.DataSource = Robots;

            gridView2.FocusedRowChanged += GridView2_FocusedRowChanged;

            RobotManager.RobotCycleStarted += RobotManager_RobotCycleStarted;
            RobotManager.RobotCycleEnded += RobotManager_RobotCycleEnded;

            Robots.AddRange(RobotManager.Robots.Select(x => x.Name));

            foreach (var robot in Robots)
            {
                var plc = RobotManager.GetPlcNames(robot).First();
                RobotLabels.Add(robot, RobotManager.GetDeviceLabels(robot, plc));
            }

            gridControl2.RefreshDataSource();
            gridView2.FocusedRowHandle = 0;

        }


        private void RobotManager_RobotCycleStarted(object sender, RobotCycleEventArgs e)
        {
            if (SelectedRobot != e.Name)
                return;

            var plcRountine =  PlcValues.FirstOrDefault(x => e.CycleEventArgs.Name == x.Name);
            if (plcRountine == null)
                return;

            plcRountine.Started = DateTime.Now;
            plcRountine.Ended = null;
        }

        private void RobotManager_RobotCycleEnded(object sender, RobotCycleEventArgs e)
        {
            if (SelectedRobot != e.Name)
                return;

            var plcRountine = PlcValues.FirstOrDefault(x => e.CycleEventArgs.Name == x.Name);
            if (plcRountine == null)
                return;

            plcRountine.Ended = DateTime.Now;
            plcRountine.Last = plcRountine.Current;
        }

        private void GridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var robotName = gridView2.GetRow(e.FocusedRowHandle) as string;
            if (robotName != null)
            {
                SelectedRobot = robotName;
                PlcValues.Clear();

                var subroutines = RobotLabels[robotName].Where(x => x.Subroutine != null)
                    .Select(x => new PlcSubroutineValue()
                    {
                        Name = x.Subroutine.Name
                    })
                    .GroupBy(x=> x.Name)
                    .Select(g=> g.First());
                PlcValues.AddRange(subroutines);
            }
        }

      
    }
}
