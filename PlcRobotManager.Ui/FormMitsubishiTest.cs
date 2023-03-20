using PlcRobotManager.Core.Vendor.Mitsubishi;
using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PlcRobotManager.Ui
{
    public partial class FormMitsubishiTest : Form
    {
        public class PlcResultItem
        {
            public string Address { get; set; }
            public int Value { get; set; }
        }

        private List<PlcResultItem> _plcResults = new List<PlcResultItem>();

        private readonly Stopwatch sw = new Stopwatch();

        private readonly MitsubishiPlc _plc = new MitsubishiPlc("Plc1", new ProgOptions()
        {
            ActTargetSimulator = 1,
        });

        public FormMitsubishiTest()
        {
            InitializeComponent();

            Address = "D0";
            Length = 10;

            _plc.Initialize();
            gridControl1.DataSource = _plcResults;

            randomReadBtn.Click += RandomReadBtn_Click;
        }


        public string Message
        {
            get => msgEdit.Text;
            set => msgEdit.Text = value;
        }

        public string Status
        {
            get => statusEdit.Text;
            set => statusEdit.Text = value;
        }

        public string Time
        {
            get => timeEdit.Text;
            set => timeEdit.Text = value;
        }

        public string Address
        {
            get => addressEdit.Text;
            set => addressEdit.Text = value;
        }

        public int Length
        {
            get
            {
                int.TryParse(lengthEdit.Text, out int v);
                return v;
            }
            set => lengthEdit.Text = value.ToString();
        }


        private void ReadBtn_ClickAsync(object sender, EventArgs e)
        {

            BlockReader reader = new BlockReader(_plc);

            string deviceName = Regex.Match(Address, "[a-zA-Z]{1,}").Value;
            int number = Convert.ToInt32(Regex.Match(Address, "[0-9]{1,}").Value);
            Device device = Device.FromName(deviceName);
            var labels = Enumerable.Range(0, Length).Select(i => new DeviceLabel(device, number + i));

            sw.Restart();
            var result = reader.ReadBlock(new BlockRange(labels));
            Time = sw.ElapsedMilliseconds.ToString();

            if (result.IsSuccessful)
            {
                var plcItems = result.Data.Select(pair => new PlcResultItem()
                {
                    Address = pair.Key,
                    Value = pair.Value
                });

                _plcResults.Clear();
                _plcResults.AddRange(plcItems);
                gridControl1.RefreshDataSource();

                Status = "읽기 성공";
            }
            else
            {
                Status = "읽기 실패";
            }
            Message = result.Message;

        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            var result = _plc.Open();
            if (result.IsSuccessful)
            {
                Status = "연결 성공";
            }
            else
            {
                Status = "연결 실패";
            }
            Message = result.Message;

        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            _plc.Close();
            Status = "연결 종료";
        }


        private void RandomReadBtn_Click(object sender, EventArgs e)
        {
            int eachCount = 100;
            int startAddress = 0;
            var dlabels = Enumerable.Range(0, eachCount).Select(i => new DeviceLabel(Device.D, startAddress + (i * 5)));
            var xlabels = Enumerable.Range(0, eachCount).Select(i => new DeviceLabel(Device.X, startAddress + (i * 5)));
            var mlabels = Enumerable.Range(0, eachCount).Select(i => new DeviceLabel(Device.M, startAddress + (i * 5)));
            var range = new RandomRange(dlabels.Concat(xlabels).Concat(mlabels));
            var randomReader = new RandomReader(_plc);

            sw.Restart();
            var result = randomReader.ReadRandom(range);
            Time = sw.ElapsedMilliseconds.ToString();

            if (result.IsSuccessful)
            {
                var plcItems = result.Data.Select(pair => new PlcResultItem()
                {
                    Address = pair.Key,
                    Value = pair.Value
                });

                _plcResults.Clear();
                _plcResults.AddRange(plcItems);
                gridControl1.RefreshDataSource();

                Status = "읽기 성공";
            }
            else
            {
                Status = "읽기 실패";
            }
            Message = result.Message;

        }


    }
}
