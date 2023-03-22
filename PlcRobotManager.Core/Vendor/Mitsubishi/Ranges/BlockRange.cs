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


            if (IsBitBlock)
            {
                var tuple = InitBitBlockRange(deviceLabels);
                Start = tuple.Item1;
                Length = tuple.Item2;
            }
            else
            {
                var tuple = InitWordBlockRange(deviceLabels);
                Start = tuple.Item1;
                Length = tuple.Item2;
            }

        }

        /// <summary>
        /// 주소의 오름차순으로 정렬된 디바이스 라벨
        /// </summary>
        public IEnumerable<DeviceLabel> OrderedDeviceLabels { get; }

        /// <summary>
        /// 시작 디바이스. 실제 필요한 라벨과 다를 수 있다.
        /// 비트라벨인 경우 2바이트 단위로 수행되는 블록읽기의 특성으로 인해 시작비트라벨을 생성하는 경우가 있다.
        /// </summary>
        public DeviceLabel Start { get; }

        /// <summary>
        /// 워드단위 길이.
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

            int startWordAddress = min.Address / 16;
            int endWordAddress = max.Address / 16;
            int length = endWordAddress - startWordAddress + 1;
            DeviceLabel start = new DeviceLabel(min.Device, startWordAddress, min.Group);

            return Tuple.Create(start, length);
        }


    }
}
