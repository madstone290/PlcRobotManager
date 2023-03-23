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

            var result = _plc.ReadBlock2(blockRange.StartWordAddressString, blockRange.Length);

            if (!result.IsSuccessful)
                return Result<Dictionary<string, short>>.Fail(result.Message);

            Dictionary<string, short> data = new Dictionary<string, short>();
            foreach (var label in blockRange.OrderedDeviceLabels)
            {
                int shortIndex = label.WordAddress - blockRange.StartWordAddress; // short배열 인덱스
                if (!label.IsBitValue) // word이면 그대로 저장
                    data[label.AddressString] = result.Data[shortIndex];
                else // bit이면 비트값 읽고 저장
                    data[label.AddressString] = BitUtil.IsOn(result.Data[shortIndex], label.BitPosition.Value) ? (short)1 : (short)0;
            }

            return Result<Dictionary<string, short>>.Success(data);
        }

        public override string ToString()
        {
            return $"{nameof(WordBlockReader)}";
        }
    }
}
