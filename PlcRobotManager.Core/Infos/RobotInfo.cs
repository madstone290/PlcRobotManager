using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Infos
{
    public class RobotInfo
    {
        /// <summary>
        /// 로봇명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 수집기 타입
        /// </summary>
        public DataGathererType GathererType { get; set; }

        public List<PlcInfo> PlcInfos { get; set; } = new List<PlcInfo>();
        
        public int AdditionalIdleTime { get; set; } = 1 * 1000;
        
        public bool DataLoggingEnabled { get; set; } = true;

        public int DataLoggingCycle { get; set; } = 1;


    }
}
