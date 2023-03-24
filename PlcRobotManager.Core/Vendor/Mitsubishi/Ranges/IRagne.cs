using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Ranges
{
    public interface IRange
    {
        /// <summary>
        /// 범위타입
        /// </summary>
        RangeType Type { get; }

        /// <summary>
        /// 주소의 오름차순으로 정렬된 디바이스 라벨
        /// </summary>
        IEnumerable<DeviceLabel> OrderedDeviceLabels { get; }
    }
}
