using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers
{
    /// <summary>
    /// 범위안에 드는 디바이스는 블록읽기로 수집한다.
    /// 블록에 포함된 디바이스가 기준보다 적은 경우 랜덤읽기로 수집한다.
    /// </summary>
    public class AutoDataGatherer : BaseGatherer
    {
        /// <summary>
        /// 디바이스 목록
        /// </summary>
        private readonly List<DeviceLabel> _deviceLabels = new List<DeviceLabel>();

        /// <summary>
        /// 블록읽기 크기
        /// </summary>
        public int BlockSize { get; set; } = 1000;

        /// <summary>
        /// 최소 블록크기. 이 크기보다 작으면 랜덤읽기로 데이터를 불러온다.
        /// </summary>
        public int MinimumBlockSize { get; set; } = 30;


        public AutoDataGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels)
            : base(plc)
        {
            _deviceLabels.AddRange(deviceLabels.OrderBy(x => x.AddressString));

             // 디바이스기준으로 분류한 다음 개수에 따라 블록읽기와 랜덤읽기로 나눈다.
            List<List<DeviceLabel>> binList = new List<List<DeviceLabel>>(); // 디바이스기준으로 라벨을 분류한다.
            List<DeviceLabel> basicBin = new List<DeviceLabel>();
            binList.Add(basicBin);

            foreach (var label in _deviceLabels)
            {
                if (!basicBin.Any())
                {
                    basicBin.Add(label);
                }
                else if (basicBin.Last().Device == label.Device)
                {
                    basicBin.Add(label);
                }
                else
                {
                    basicBin = new List<DeviceLabel>() { label };
                    binList.Add(basicBin);
                }
            }

            var blockBins = binList.Where(x => MinimumBlockSize <= x.Count());

            // TODO 주소간격이 멀리 떨어진 경우 새블록or랜덤읽기 적용
            _blockRanges.AddRange(blockBins.Select(bin => new BlockRange(bin)));

            // 나머지는 모두 하나의 랜덤범위로 추가
            var randomBins = binList.Except(blockBins);
            if (randomBins.Any())
            {
                _randomRanges.Add(new RandomRange(randomBins.SelectMany(label => label)));
            }
        }

    }
}
