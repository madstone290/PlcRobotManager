using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System;
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

        private readonly List<BlockRange> _blockRanges = new List<BlockRange>();

        private readonly List<RandomRange> _randomRanges= new List<RandomRange>();


        public AutoDataGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels)
            : base(plc)
        {
            _deviceLabels.AddRange(deviceLabels.OrderBy(x => x.AddressString));

            // 디바이스기준으로 분류한 다음 개수에 따라 블록읽기와 랜덤읽기로 나눈다.
            var sorter = new Sorter();
            var ranges = sorter.Sort(deviceLabels);

            _blockRanges.AddRange(ranges.Item1);
            _randomRanges.AddRange(ranges.Item2);
        }

        /// <summary>
        /// 블록읽기 최대크기
        /// </summary>
        public int MaxBlockSize { get; set; } = 1000;

        /// <summary>
        /// 최소 라벨개수.블록에 이보다 작은 라벨이 존재하면 랜덤읽기로 데이터를 불러온다.
        /// </summary>
        public int MinLabelCount { get; set; } = 30;

        public override IEnumerable<BlockRange> BlockRanges => _blockRanges;

        public override IEnumerable<RandomRange> RandomRanges => _randomRanges;

        public override string ToString()
        {
            return nameof(AutoDataGatherer);
        }

        /// <summary>
        /// 데이터 수집에 사용할 라벨을 분류한다.
        /// </summary>
        public class Sorter
        {
            /// <summary>
            /// 주어진 라벨을 디바이스단위로 분류한다.
            /// 최소 라벨개수보다 적은 경우 랜덤범위로 분류된다.
            /// 최대 블록크기보다 큰 블록범위는 크기를 만족하는 여러개의 블록범위로 분류된다.
            /// </summary>
            /// <param name="deviceLabels"></param>
            /// <param name="maxBlockSize"></param>
            /// <param name="minLabelCount"></param>
            /// <returns></returns>
            public Tuple<List<BlockRange>, List<RandomRange>> Sort(IEnumerable<DeviceLabel> deviceLabels, int maxBlockSize = 1000, int minLabelCount = 30)
            {
                List<BlockRange> blockRanges = new List<BlockRange>();
                List<RandomRange> randomRanges = new List<RandomRange>();
                List<DeviceLabel> randomLabels = new List<DeviceLabel>();

                var deviceLabelGroups = deviceLabels.GroupBy(x => x.Device); // 디바이스 단위로 라벨 1차 분류
                foreach (var group in deviceLabelGroups)
                {
                    if (group.Count() < minLabelCount)
                    {  
                        // 최소 라벨개수보다 적으면 랜덤범위로 추가
                        randomLabels.AddRange(group);
                        continue;
                    }

                    List<List<DeviceLabel>> blockGroups = DivideByMaxBlockSize(group, maxBlockSize); // 최대 블록크기에 맞춰 2차 분류
                    foreach (var blockGroup in blockGroups)
                    {
                        if (blockGroup.Count() < minLabelCount)
                            randomLabels.AddRange(blockGroup);
                        else
                            blockRanges.Add(new BlockRange(blockGroup));
                    }

                }

                if (randomLabels.Any())
                    randomRanges.Add(new RandomRange(randomLabels));

                return new Tuple<List<BlockRange>, List<RandomRange>>(blockRanges, randomRanges);
            }

            /// <summary>
            /// 최대 블록크기 기준으로 다시 분류한다.
            /// </summary>
            /// <param name="deviceLabels"></param>
            /// <param name="maxBlockSize"></param>
            /// <returns></returns>
            private List<List<DeviceLabel>> DivideByMaxBlockSize(IEnumerable<DeviceLabel> deviceLabels, int maxBlockSize)
            {
                List<List<DeviceLabel>> blockGroups = new List<List<DeviceLabel>>();
                IEnumerable<DeviceLabel> orderedLabels = deviceLabels.OrderBy(x => x.Address);

                DeviceLabel first = orderedLabels.First();
                DeviceLabel last = orderedLabels.Last();

                int leftAddress = -1;
                int rightAddress = -1;
                int remainingLabelCount = orderedLabels.Count();

                while (0 < remainingLabelCount)
                {
                    // 현재 블록크기를 설정한다.
                    leftAddress = orderedLabels.First(x => rightAddress < x.StartWordAddress).StartWordAddress; // 남은 구간의 시작주소
                    rightAddress = leftAddress + maxBlockSize < last.EndWordAddress  // 남은 구간의 끝 주소
                        ? leftAddress + maxBlockSize // 남은 블록크기가 최대 블록크기보다 큰 경우
                        : last.EndWordAddress; // 최대 블록크기보다 작은 경우
                    // 현재 블록에 포함된 모든 라벨을 그룹으로 등록한다.
                    List<DeviceLabel> blockGroup = orderedLabels.Where(x => leftAddress <= x.StartWordAddress && x.StartWordAddress <= rightAddress).ToList();
                    blockGroups.Add(blockGroup);

                    remainingLabelCount -= blockGroup.Count;
                }

                return blockGroups;
            }
        }

    }
}
