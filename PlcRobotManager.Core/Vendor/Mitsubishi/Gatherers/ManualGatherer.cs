using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers
{
    /// <summary>
    /// 수동 수집기. 사용자가 제공한 그룹단위로 데이터를 수집한다.
    /// </summary>
    public class ManualGatherer : BaseGatherer
    {
        /// <summary>
        /// 수집 그룹
        /// </summary>
        private readonly Dictionary<GatheringGroup, List<DeviceLabel>> _gatheringGroups = new Dictionary<GatheringGroup, List<DeviceLabel>>();

        private readonly List<BlockRange> _blockRanges = new List<BlockRange>();

        private readonly List<RandomRange> _randomRanges = new List<RandomRange>();

        public ManualGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels) : base(plc)
        {
            var labelGroups = deviceLabels.GroupBy(x => x.Group);
            foreach (var labelGroup in labelGroups)
            {
                _gatheringGroups.Add(labelGroup.Key, labelGroup.ToList());

                GatheringGroup gatheringGroup = labelGroup.Key;
                if (gatheringGroup.RangeType == RangeType.Block)
                    _blockRanges.Add(new BlockRange(labelGroup));
                else
                    _randomRanges.Add(new RandomRange(labelGroup));
            }
        }


        public override IEnumerable<BlockRange> BlockRanges => _blockRanges;

        public override IEnumerable<RandomRange> RandomRanges => _randomRanges;

    }
}
