using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Readers
{
    /// <summary>
    /// 비트블록 읽기를 수행한다
    /// </summary>
    public class BitBlockReader
    {
        private readonly IMitsubishiPlc _plc;

        public BitBlockReader(IMitsubishiPlc plc)
        {
            _plc = plc;
        }

        public Result<Dictionary<string, short>> ReadBlock(BlockRange blockRange)
        {
            if(blockRange == null) throw new ArgumentNullException(nameof(blockRange));
            if (!blockRange.IsBitBlock) throw new ArgumentException("비트블록이 아닙니다");

            var result = _plc.ReadBlock2(blockRange.StartAddressString, blockRange.Length);

            if (!result.IsSuccessful)
                return Result<Dictionary<string, short>>.Fail(result.Message);

            Dictionary<string, short> data = new Dictionary<string, short>();
            int startAddress = blockRange.StartAddress;

            foreach(var label in blockRange.OrderedDeviceLabels)
            {
                int bitIndex = label.Address - startAddress; // 비트 주소의 인덱스
                int shortIndex = bitIndex / 16; // 수신한 short배열에서의 인덱스
                short shortValue = result.Data[shortIndex]; 

                int bitPostion = bitIndex % 16; // short값에서 bit의 위치
                bool isTrue = (shortValue & (1 << bitPostion)) != 0;
                data[label.AddressString] = isTrue ? (short)1 : (short)0;
            }

            return Result<Dictionary<string, short>>.Success(data);
        }
    }
}
