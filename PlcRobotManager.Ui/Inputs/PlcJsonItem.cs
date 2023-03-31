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

        public int ActTargetSimulator { get; set; } = 1;

        public int StationNumber { get; set; } = 1;

        public string LabelFilePath { get; set; }
    }
}
