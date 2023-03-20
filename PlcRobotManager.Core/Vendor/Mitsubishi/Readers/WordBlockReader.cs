using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Readers
{
    /// <summary>
    /// 워드블록 읽기를 수행한다
    /// </summary>
    public class WordBlockReader
    {
        private readonly IMitsubishiPlc _plc;

        public WordBlockReader(IMitsubishiPlc plc)
        {
            _plc = plc;
        }

        public Result<Dictionary<string, short>> ReadBlock(BlockRange blockRange)
        {
            if (blockRange == null) throw new ArgumentNullException(nameof(blockRange));
            if (blockRange.IsBitBlock) throw new ArgumentException("워드블록이 아닙니다");

            var result = _plc.ReadBlock2(blockRange.Start.AddressString, blockRange.Length);

            if (!result.IsSuccessful)
                return Result<Dictionary<string, short>>.Fail(result.Message);

            Dictionary<string, short> data = new Dictionary<string, short>();
            int startAddress = blockRange.OrderedDeviceLabels.First().Address;

            foreach (var label in blockRange.OrderedDeviceLabels)
            {
                int shortIndex = label.Address - startAddress; // 수신한 short배열에서의 인덱스
                data[label.AddressString] = result.Data[shortIndex];
            }

            return Result<Dictionary<string, short>>.Success(data);
        }
    }
}
