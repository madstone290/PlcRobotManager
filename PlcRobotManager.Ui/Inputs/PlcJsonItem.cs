using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Ui.Inputs
{
    public class PlcJsonItem
    {
        public string Name { get; set; }

        public string LabelFilePath { get; set; }

        public ProgOptions ProgOptions { get; set; }
    }
}
