using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Ranges
{
    /// <summary>
    /// 블록읽기에 사용할 블록 범위
    /// </summary>
    public class BlockRange
    {
        public BlockRange(IEnumerable<DeviceLabel> deviceLabels)
        {
            if (deviceLabels == null) throw new ArgumentNullException(nameof(deviceLabels));
            if (!deviceLabels.Any()) throw new ArgumentException("디바이스 라벨이 비었습니다");
            if (1 < deviceLabels.GroupBy(x => x.Device).Count()) throw new ArgumentException("2개 이상의 디바이스가 존재합니다");

            OrderedDeviceLabels = deviceLabels.OrderBy(x => x.Address);
            Device = deviceLabels.First().Device;
            IsBitBlock = Device.IsBit();

            DeviceLabel min = OrderedDeviceLabels.First();
            DeviceLabel max = OrderedDeviceLabels.Last();
            StartWordAddress = min.WordAddress;
            Length = max.WordAddress - min.WordAddress + 1;
            StartWordAddressString = min.WordAddressString;
        }

        /// <summary>
        /// 주소의 오름차순으로 정렬된 디바이스 라벨
        /// </summary>
        public IEnumerable<DeviceLabel> OrderedDeviceLabels { get; }

        /// <summary>
        /// 디바이스
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// 시작주소(워드)
        /// </summary>
        public int StartWordAddress { get; }

        /// <summary>
        /// 시작주소 문자열(워드)
        /// </summary>
        public string StartWordAddressString { get; }

        /// <summary>
        /// 길이(워드)
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 비트블록 여부
        /// </summary>
        public bool IsBitBlock { get; }

    }
}
