using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers
{
    public abstract class BaseGatherer : IPlcDataGatherer
    {
        private readonly ILogger _logger = LoggerFactory.GetLogger<BaseGatherer>();

        /// <summary>
        /// 읽기를 진행할 PLC
        /// </summary>
        protected readonly IMitsubishiPlc _plc;

        /// <summary>
        /// 원본 데이터. 주소/값 쌍
        /// </summary>
        protected readonly Dictionary<string, short> _rawData = new Dictionary<string, short>();

        /// <summary>
        /// 가공된 데이터. 라벨코드/값 쌍
        /// </summary>
        protected readonly Dictionary<string, object> _processedData = new Dictionary<string, object>();

        /// <summary>
        /// 비트블록 리더
        /// </summary>
        protected readonly BitBlockReader _bitBlockReader;

        /// <summary>
        /// 워드블록 리더
        /// </summary>
        protected readonly WordBlockReader _wordBlockReader;

        /// <summary>
        /// 랜덤 리더
        /// </summary>
        protected readonly RandomReader _randomReader;

        private readonly Subroutines.CycleTimeSubroutine routine1 = new Subroutines.CycleTimeSubroutine("test", "CycleTime1", 1);

        public BaseGatherer(IMitsubishiPlc plc)
        {
            _plc = plc;
            _wordBlockReader = new WordBlockReader(plc);
            _bitBlockReader = new BitBlockReader(plc);
            _randomReader = new RandomReader(plc);

            routine1.CycleStarted += (s, count) =>
            {
                _logger.Debug($"CycleStarted {count}");
            };
            routine1.CycleEnded += (s, count) =>
            {
                _logger.Debug($"CycleEnded {count}");
            };

        }

        /// <summary>
        /// 블록읽기 목록
        /// </summary>
        public abstract IEnumerable<BlockRange> BlockRanges { get; }

        /// <summary>
        /// 랜덤읽기 범위
        /// </summary>
        public abstract IEnumerable<RandomRange> RandomRanges { get; }

        public IReadOnlyDictionary<string, short> RawData => _rawData;

        public IReadOnlyDictionary<string, object> ProcessedData => _processedData;

        public Result Gather()
        {
            _rawData.Clear();

            #region 블록읽기
            foreach (var blockRange in BlockRanges)
            {
                Result<Dictionary<string, short>> result;
                if (blockRange.IsBitBlock)
                    result = _bitBlockReader.ReadBlock(blockRange);
                else
                    result = _wordBlockReader.ReadBlock(blockRange);

                if (!result.IsSuccessful)
                    return Result.Fail(result.Message);

                foreach (var pair in result.Data)
                    _rawData[pair.Key] = pair.Value;

            }
            #endregion

            #region 랜덤읽기
            foreach (var randomRange in RandomRanges)
            {
                var result = _randomReader.ReadRandom(randomRange);
                if (!result.IsSuccessful)
                    return Result.Fail(result.Message);

                foreach (var pair in result.Data)
                    _rawData[pair.Key] = pair.Value;
            }
            #endregion

            // 데이터 후처리
            ProcessValue();

            // 서브루틴 상태 갱신
            routine1.CheckCycle(_processedData);

            return Result.Success();
        }

        private void ProcessValue()
        {
            _processedData.Clear();
            IEnumerable<IRange> entireRanges = BlockRanges.Cast<IRange>().Concat(RandomRanges.Cast<IRange>());
            foreach (IRange range in entireRanges)
            {
                foreach (var label in range.OrderedDeviceLabels)
                {
                    List<short> rawValues = label.AddressStringList.Select(address => _rawData[address]).ToList();
                    _processedData[label.Code] = label.ConvertValue(rawValues);
                }
            }

        }

    }
}

