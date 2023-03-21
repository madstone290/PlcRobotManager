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
    public class ManualGatherer : IPlcDataGatherer
    {
        /// <summary>
        /// 읽기를 진행할 PLC
        /// </summary>
        private readonly IMitsubishiPlc _plc;

        /// <summary>
        /// 비트블록 리더
        /// </summary>
        private readonly BitBlockReader _bitBlockReader;

        /// <summary>
        /// 워드블록 리더
        /// </summary>
        private readonly WordBlockReader _wordBlockReader;

        /// <summary>
        /// 수집 그룹
        /// </summary>
        private readonly Dictionary<GatheringGroup, List<DeviceLabel>> _gatheringGroups = new Dictionary<GatheringGroup, List<DeviceLabel>>();

        /// <summary>
        /// 블록읽기 목록
        /// </summary>
        private readonly List<BlockRange> _blockRanges = new List<BlockRange>();

        /// <summary>
        /// 랜덤읽기 범위
        /// </summary>
        private readonly List<RandomRange> _randomRanges = new List<RandomRange>();

        public ManualGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels)
        {
            _plc = plc;
            _wordBlockReader = new WordBlockReader(plc);
            _bitBlockReader = new BitBlockReader(plc);

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

        public Result<Dictionary<string, short>> Gather()
        {
            Dictionary<string, short> data = new Dictionary<string, short>();

            #region 블록읽기
            foreach (var blockRange in _blockRanges)
            {
                Result<Dictionary<string, short>> readResult;
                if (blockRange.IsBitBlock)
                    readResult = _bitBlockReader.ReadBlock(blockRange);
                else
                    readResult = _wordBlockReader.ReadBlock(blockRange);

                if (!readResult.IsSuccessful)
                    return Result<Dictionary<string, short>>.Fail(readResult.Message);

                foreach (var pair in readResult.Data)
                    data[pair.Key] = pair.Value;

            }
            #endregion

            #region 랜덤읽기
            foreach (var randomRange in _randomRanges)
            {
                Result<short[]> readResult = _plc.ReadRandom(randomRange.DeviceList, randomRange.Length);
                if (!readResult.IsSuccessful)
                    return Result<Dictionary<string, short>>.Fail(readResult.Message);

                int shortIndex = 0; // 수신한 short배열에서의 인덱스
                foreach (DeviceLabel label in randomRange.OrderedDeviceLabels)
                    data[label.AddressString] = readResult.Data[shortIndex++];
            }
            #endregion

            return Result<Dictionary<string, short>>.Success(data);

        }
    }
}
