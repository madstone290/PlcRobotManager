using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

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
        /// 데이터 수집에 사용할 라벨목록. 주소순으로 정렬되어 있다.
        /// </summary>
        protected readonly List<DeviceLabel> _deviceLabels = new List<DeviceLabel>();

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

        /// <summary>
        /// 서브루틴 팩토리
        /// </summary>
        private readonly SubroutineFactory _subroutineFactory = new SubroutineFactory();

        private readonly List<ISubroutine> _subroutines = new List<ISubroutine>();

        public BaseGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels)
        {
            _plc = plc;
            _deviceLabels.AddRange(deviceLabels.OrderBy(x => x, DeviceLabel.Comparer.Default));
            _wordBlockReader = new WordBlockReader(plc);
            _bitBlockReader = new BitBlockReader(plc);
            _randomReader = new RandomReader(plc);

            List<ISubroutine> subroutines = _subroutineFactory.Create(_deviceLabels.Where(x => x.Subroutine != null));
            foreach(ISubroutine subroutine in subroutines)
            {
                subroutine.CycleStarted += (s, count) =>
                {
                    CycleStarted?.Invoke(s, new CycleEventArgs(subroutine.Name, subroutine.Codes, count, DateTime.UtcNow));
                    _logger.Debug($"CycleStarted Name:{subroutine.Name} Count:{count}");
                };
                subroutine.CycleEnded += (s, count) =>
                {
                    CycleEnded?.Invoke(s, new CycleEventArgs(subroutine.Name, subroutine.Codes, count, DateTime.UtcNow));
                    _logger.Debug($"CycleEnded Name:{subroutine.Name} Count:{count}");
                };
            }
            _subroutines.AddRange(subroutines);
        }

        protected BaseGatherer()
        {
        }

        public event EventHandler<CycleEventArgs> CycleStarted;
        public event EventHandler<CycleEventArgs> CycleEnded;
        public event EventHandler<ValueChangeEventArgs> ValueChanged;

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
            foreach (ISubroutine subroutine in _subroutines)
                subroutine.CheckCycle(_processedData);

            return Result.Success();
        }

        private void ProcessValue()
        {
            IEnumerable<IRange> entireRanges = BlockRanges.Cast<IRange>().Concat(RandomRanges.Cast<IRange>());
            foreach (IRange range in entireRanges)
            {
                foreach (var label in range.OrderedDeviceLabels)
                {
                    List<short> rawValues = label.AddressStringList.Select(address => _rawData[address]).ToList();
                    object newValue = label.ConvertValue(rawValues);
                    if (label.RaiseValueEvent && _processedData.TryGetValue(label.Code, out object prevValue)) // 최초 조회에는 값변경 이벤트 발생시키지 않음
                    {
                        if (!newValue.Equals(prevValue))
                            ValueChanged?.Invoke(this, new ValueChangeEventArgs(label.Code, prevValue, newValue, DateTime.UtcNow));
                    }
                    _processedData[label.Code] = newValue;
                }
            }

        }

    }
}

