using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Readers
{
    public class RandomReader
    {
        private readonly IMitsubishiPlc _plc;

        public RandomReader(IMitsubishiPlc plc)
        {
            _plc = plc;
        }

        public Result<Dictionary<string, short>> ReadRandom(RandomRange randomRange)
        {
            Dictionary<string, short> data = new Dictionary<string, short>();

            Result<short[]> readResult = _plc.ReadRandom(randomRange.DeviceList, randomRange.Length);
            if (!readResult.IsSuccessful)
                return Result<Dictionary<string, short>>.Fail(readResult.Message);

            int shortIndex = 0; // 수신한 short배열에서의 인덱스
            foreach (DeviceLabel label in randomRange.OrderedDeviceLabels)
                data[label.AddressString] = readResult.Data[shortIndex++];

            return Result<Dictionary<string, short>>.Success(data);
        }
    }
}
