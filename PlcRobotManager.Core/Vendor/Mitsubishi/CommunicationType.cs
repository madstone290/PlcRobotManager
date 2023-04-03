using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 미쓰비시 PLC통신 유형
    /// </summary>
    public enum CommunicationType
    {
        /// <summary>
        /// Q시리즈 E71 Tcp
        /// </summary>
        QSeriesE71Tcp,

        /// <summary>
        /// 이더넷 어댑터 Tcp
        /// </summary>
        EthernetAdapterTcp,

        /// <summary>
        /// GX 시뮬레이터2
        /// </summary>
        GXSimulator2
    }
}
