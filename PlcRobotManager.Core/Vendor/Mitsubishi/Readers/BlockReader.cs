using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Readers
{
    /// <summary>
    /// 비트블록/워드블록에 대한 블록읽기를 실행한다.
    /// </summary>
    public class BlockReader
    {
        private readonly BitBlockReader _bitBlockReader;
        private readonly WordBlockReader _wordBlockReader;

        public BlockReader(IMitsubishiPlc plc)
        {
            _bitBlockReader = new BitBlockReader(plc);
            _wordBlockReader = new WordBlockReader(plc);
        }

        public Result<Dictionary<string, short>> ReadBlock(BlockRange blockRange)
        {
            Result<Dictionary<string, short>> result;
            if (blockRange.IsBitBlock)
                result = _bitBlockReader.ReadBlock(blockRange);
            else
                result = _wordBlockReader.ReadBlock(blockRange);

            if (!result.IsSuccessful)
                return Result<Dictionary<string, short>>.Fail(result.Message);

            return Result<Dictionary<string, short>>.Success(result.Data);
        }

     
    }
}
