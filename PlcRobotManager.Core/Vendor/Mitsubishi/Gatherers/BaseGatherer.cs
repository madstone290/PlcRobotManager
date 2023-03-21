using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
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
        protected readonly List<BlockRange> _blockRanges = new List<BlockRange>();

        /// <summary>
        /// 랜덤읽기 범위
        /// </summary>
        protected readonly List<RandomRange> _randomRanges = new List<RandomRange>();

        public BaseGatherer(IMitsubishiPlc plc)
        {
            _plc = plc;
            _wordBlockReader = new WordBlockReader(plc);
            _bitBlockReader = new BitBlockReader(plc);
            _randomReader = new RandomReader(plc);
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
                var result = _randomReader.ReadRandom(randomRange);
                if (!result.IsSuccessful)
                    return Result<Dictionary<string, short>>.Fail(result.Message);

                foreach (var pair in result.Data)
                    data[pair.Key] = pair.Value;
            }
            #endregion

            return Result<Dictionary<string, short>>.Success(data);
        }
    }
}
