using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers
{
    public abstract class BaseGatherer : IPlcDataGatherer
    {
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

        public BaseGatherer(IMitsubishiPlc plc)
        {
            _plc = plc;
            _wordBlockReader = new WordBlockReader(plc);
            _bitBlockReader = new BitBlockReader(plc);
            _randomReader = new RandomReader(plc);
        }

        public Result Gather()
        {
            _rawData.Clear();

            #region 블록읽기
            foreach (var blockRange in BlockRanges)
            {
                Result<Dictionary<string, short>> readResult;
                if (blockRange.IsBitBlock)
                    readResult = _bitBlockReader.ReadBlock(blockRange);
                else
                    readResult = _wordBlockReader.ReadBlock(blockRange);

                if (!readResult.IsSuccessful)
                    return Result.Fail(readResult.Message);

                foreach (var pair in readResult.Data)
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

            //값 변환
            ProcessValue();

            return Result.Success();
        }

        private void ProcessValue()
        {
            _processedData.Clear();
            foreach (var range in BlockRanges)
            {
                foreach(var label in range.OrderedDeviceLabels)
                {
                    List<short> values = new List<short>();
                    foreach (var address in label.AddressStringList)
                    {
                        values.Add(_rawData[address]);
                    }
                    _processedData[label.Code] = label.ConvertValue(values);
                }
            }

            foreach(var range in RandomRanges) 
            {
                foreach (var label in range.OrderedDeviceLabels)
                {
                    List<short> values = new List<short>();
                    foreach (var address in label.AddressStringList)
                    {
                        values.Add(_rawData[address]);
                    }
                    _processedData[label.Code] = label.ConvertValue(values);
                }
            }
        }

    }
}

