using PlcRobotManager.Core;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using PlcRobotManager.Ui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlcRobotManager.Ui.Views.Auto
{
    public partial class FormAutoProcessed : Form
    {
        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        public FormAutoProcessed()
        {
            InitializeComponent();
            RobotManager = Program.RobotManager;
        }

        public List<PlcObjValue> PlcValues { get; set; } = new List<PlcObjValue>();
        public List<string> Robots { get; set; } = new List<string>();

        public string SelectedRobot { get; set; }
        public IRobotManager RobotManager { get; internal set; }

        public Dictionary<string, List<DeviceLabel>> RobotLabels { get; set; } = new Dictionary<string, List<DeviceLabel>>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            gridControl1.DataSource = PlcValues;
            gridControl2.DataSource = Robots;

            gridView2.FocusedRowChanged += GridView2_FocusedRowChanged;

            
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            if (RobotManager != null)
            {
                Robots.AddRange(RobotManager.Robots.Select(x => x.Name));

                foreach(var robot in Robots)
                {
                    var plc = RobotManager.GetPlcNames(robot).First();
                    RobotLabels.Add(robot, RobotManager.GetDeviceLabels(robot, plc));
                }

                gridControl2.RefreshDataSource();
                gridView2.FocusedRowHandle = 0;
            }

        }

        private void GridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var robotName = gridView2.GetRow(e.FocusedRowHandle) as string;
            if(robotName != null)
            {
                timer.Stop();
                SelectedRobot = robotName;
                RefreshRobotData();
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshRobotData();
        }

        private void RefreshRobotData()
        {
            var rawData = RobotManager.GetProcessedRobotData(SelectedRobot);

            if (!RobotLabels.ContainsKey(SelectedRobot))
                return;

            var plcValues = RobotLabels[SelectedRobot]
                .Select(label=> new PlcObjValue(label, rawData[label.Code]))
                .OrderBy(obj => obj.Label, DeviceLabel.Comparer.Default)
                .ToList();

            PlcValues.Clear();
            PlcValues.AddRange(plcValues);

            this.InvokeSafe(() => gridControl1.RefreshDataSource());
        }

    }
}
