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

            Tuple<int, int> tuple;
            if (IsBitBlock)
            {
                tuple = InitBitBlockRange(deviceLabels);
            }
            else
            {
                tuple = InitWordBlockRange(deviceLabels);
            }
            StartAddress = tuple.Item1;
            Length = tuple.Item2;
            StartAddressString = Device.GetAddressString(StartAddress);

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
        public int StartAddress { get; }

        /// <summary>
        /// 시작주소 문자열
        /// </summary>
        public string StartAddressString { get; }

        /// <summary>
        /// 길이(워드)
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
        private Tuple<int, int> InitWordBlockRange(IEnumerable<DeviceLabel> orderedLabels)
        {
            DeviceLabel min = orderedLabels.First();
            DeviceLabel max = orderedLabels.Last();

            int length = max.Address - min.Address + 1;

            return Tuple.Create(min.Address, length);
        }

        /// <summary>
        /// 비트블록범위로 초기화한다
        /// </summary>
        /// <param name="orderedLabels">주소의 오름차순으로 정렬된 라벨목록</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        private Tuple<int, int> InitBitBlockRange(IEnumerable<DeviceLabel> orderedLabels)
        {
            DeviceLabel min = orderedLabels.First();
            DeviceLabel max = orderedLabels.Last();

            int startWordAddress = min.Address / 16;
            int endWordAddress = max.Address / 16;
            int length = endWordAddress - startWordAddress + 1;

            return Tuple.Create(startWordAddress, length);
        }


    }
}
