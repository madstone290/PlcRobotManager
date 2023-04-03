using PlcRobotManager.Core.Vendor.Mitsubishi;
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
        };

        public string Name { get; set; }

        public ProgOptions ProgOptions { get; set; }

        public List<DeviceLabelInfo> DeviceLabelInfos { get; set; } = new List<DeviceLabelInfo>();
    }
}
