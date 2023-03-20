using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers
{
    /// <summary>
    /// 데이터를 모두 랜덤읽기로 수집한다.
    /// </summary>
    public class RandomDataGatherer : IPlcDataGatherer
    {
        /// <summary>
        /// 읽기를 진행할 PLC
        /// </summary>
        private readonly IMitsubishiPlc _plc;

        private readonly RandomReader _randomReader;

        /// <summary>
        /// 디바이스 목록
        /// </summary>
        private readonly List<DeviceLabel> _deviceLabels = new List<DeviceLabel>();

        /// <summary>
        /// 랜덤읽기 범위
        /// </summary>
        private readonly List<RandomRange> _randomRanges = new List<RandomRange>();

        /// <summary>
        /// 한번에 읽어올 디바이스 개수
        /// </summary>
        private int _readLength = 100;

        /// <summary>
        /// 한번에 읽어올 디바이스 개수
        /// </summary>
        public int ReadLength
        {
            get => _readLength;
            set
            {
                if (value < 1)
                    _readLength = 1;
                else
                    _readLength = value;
            }
        }

        public RandomDataGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels)
        {
            _plc = plc;
            _deviceLabels.AddRange(deviceLabels.OrderBy(x => x.AddressString));

            _randomReader = new RandomReader(plc);

            int taken = 0;
            while (taken < _deviceLabels.Count)
            {
                int toTake = Math.Min(_deviceLabels.Count - taken, ReadLength);
                _randomRanges.Add(new RandomRange(_deviceLabels.Skip(taken).Take(toTake)));

                taken += toTake;
            }
        }

        public Result<Dictionary<string, short>> Gather()
        {
            Dictionary<string, short> data = new Dictionary<string, short>();

            foreach (var randomRange in _randomRanges)
            {
                var result = _randomReader.ReadRandom(randomRange);
                if (!result.IsSuccessful)
                    return Result<Dictionary<string, short>>.Fail(result.Message);

                foreach(var pair in result.Data)
                    data[pair.Key] = pair.Value;
            }

            return Result<Dictionary<string, short>>.Success(data);
        }

    }
}
