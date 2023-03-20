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
            IsBitBlock = deviceLabels.First().Device.IsBit();

            Tuple<DeviceLabel, int> startAndLength;
            if (IsBitBlock)
                startAndLength = InitBitBlockRange(deviceLabels);
            else
                startAndLength = InitWordBlockRange(deviceLabels);

            Start = startAndLength.Item1;
            Length = startAndLength.Item2;
        }

        /// <summary>
        /// 주소의 오름차순으로 정렬된 디바이스 라벨
        /// </summary>
        public IEnumerable<DeviceLabel> OrderedDeviceLabels { get; }

        /// <summary>
        /// 시작 디바이스
        /// </summary>
        public DeviceLabel Start { get; }

        /// <summary>
        /// 길이
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 비트블록 여부
        /// </summary>
        public bool IsBitBlock { get; }

        /// <summary>
        /// 워드블록범위로 초기화한다
        /// </summary>
        /// <param name="orderedLabels">주소의 오름차순으로 정렬된 라벨목록</param>
        /// <returns></returns>
        private Tuple<DeviceLabel, int> InitWordBlockRange(IEnumerable<DeviceLabel> orderedLabels)
        {
            DeviceLabel min = orderedLabels.First();
            DeviceLabel max = orderedLabels.Last();
            
            DeviceLabel start = min;
            int length = max.Address - min.Address + 1;

            return Tuple.Create(start, length);
        }

        /// <summary>
        /// 비트블록범위로 초기화한다
        /// </summary>
        /// <param name="orderedLabels">주소의 오름차순으로 정렬된 라벨목록</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        private Tuple<DeviceLabel, int> InitBitBlockRange(IEnumerable<DeviceLabel> orderedLabels)
        {
            DeviceLabel min = orderedLabels.First();
            DeviceLabel max = orderedLabels.Last();

            if (min.Address % 16 != 0) throw new Exception("비트블록의 시작주소는 16의 배수여야 합니다");

            DeviceLabel start = min;
            int length;
            if (max.Address % 16 == 0)
                length = max.Address / 16;
            else
                length = (max.Address / 16) + 1;

            return Tuple.Create(start, length);
        }

        
    }
}
