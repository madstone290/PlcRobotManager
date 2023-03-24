using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Ranges
{
    /// <summary>
    /// 랜덤읽기 범위
    /// </summary>
    public class RandomRange : IRange
    {
        public RandomRange(IEnumerable<DeviceLabel> deviceLabels)
        {
            if (deviceLabels == null) throw new ArgumentNullException(nameof(deviceLabels));
            if (!deviceLabels.Any()) throw new ArgumentException("디바이스 라벨이 비었습니다");

            OrderedDeviceLabels = deviceLabels.OrderBy(x => x.AddressString);
            Length = OrderedDeviceLabels.Sum(x => x.AddressStringList.Count); // 조회할 모든 주소의 개수
            DeviceList = string.Join("\n", OrderedDeviceLabels.SelectMany(x=> x.AddressStringList));
        }

        /// <summary>
        /// 주소의 오름차순으로 정렬된 디바이스 라벨
        /// </summary>
        public IEnumerable<DeviceLabel> OrderedDeviceLabels { get; }

        /// <summary>
        /// 디바이스주소 목록
        /// </summary>
        public string DeviceList { get; }

        /// <summary>
        /// 주소의 개수
        /// </summary>
        public int Length { get; }

        RangeType IRange.Type => RangeType.Random;
        IEnumerable<DeviceLabel> IRange.OrderedDeviceLabels => OrderedDeviceLabels;

        public override string ToString()
        {
            return $"{nameof(RandomRange)}";
        }
    }
}
