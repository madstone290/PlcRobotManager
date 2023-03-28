using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Infos
{
    public class PlcInfo
    {
        public static PlcInfo Test { get; } = new PlcInfo()
        {
            Name = "Test PLC",
            StationNumber = 1
        };

        public string Name { get; set; }

        public int ActTargetSimulator { get; set; } = 1;

        public int StationNumber { get; set; } = 1;

        public List<DeviceLabelInfo> DeviceLabelInfos { get; set; } = new List<DeviceLabelInfo>();
    }
}
