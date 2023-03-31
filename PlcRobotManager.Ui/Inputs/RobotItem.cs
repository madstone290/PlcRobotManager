using PlcRobotManager.Core.Infos;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Ui.Inputs
{
    public class RobotJsonItem
    {
        /// <summary>
        /// 로봇명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 수집기 타입
        /// </summary>
        public DataGathererType GathererType { get; set; }

        public int AdditionalIdleTime { get; set; } = 1 * 1000;

        public bool DataLoggingEnabled { get; set; } = true;

        public int DataLoggingCycle { get; set; } = 1;

        /// <summary>
        /// 로봇에서 사용할 PLC정보
        /// </summary>
        public List<PlcJsonItem> PlcList { get; set; }

    }
}
