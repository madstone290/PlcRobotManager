using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers
{
    /// <summary>
    /// 범위안에 드는 디바이스는 블록읽기로 수집한다.
    /// 블록에 포함된 디바이스가 기준보다 적은 경우 랜덤읽기로 수집한다.
    /// </summary>
    public class AutoDataGatherer : IPlcDataGatherer
    {
        /// <summary>
        /// 읽기를 진행할 PLC
        /// </summary>
        private readonly IMitsubishiPlc _plc;

        /// <summary>
        /// 비트블록 리더
        /// </summary>
        private readonly BitBlockReader _bitBlockReader;

        /// <summary>
        /// 워드블록 리더
        /// </summary>
        private readonly WordBlockReader _wordBlockReader;

        /// <summary>
        /// 디바이스 목록
        /// </summary>
        private readonly List<DeviceLabel> _deviceLabels = new List<DeviceLabel>();

        /// <summary>
        /// 블록읽기 목록
        /// </summary>
        private readonly List<BlockRange> _blockRanges = new List<BlockRange>();

        /// <summary>
        /// 랜덤읽기 범위
        /// </summary>
        private readonly List<RandomRange> _randomRanges = new List<RandomRange>();

        /// <summary>
        /// 블록읽기 크기
        /// </summary>
        public int BlockSize { get; set; } = 1000;

        /// <summary>
        /// 최소 블록크기. 이 크기보다 작으면 랜덤읽기로 데이터를 불러온다.
        /// </summary>
        public int MinimumBlockSize { get; set; } = 30;


        public AutoDataGatherer(IMitsubishiPlc plc, IEnumerable<DeviceLabel> deviceLabels)
        {
            _plc = plc;
            _deviceLabels.AddRange(deviceLabels.OrderBy(x => x.AddressString));

            _wordBlockReader = new WordBlockReader(plc);
            _bitBlockReader = new BitBlockReader(plc);

            Sort();
        }

        private void Sort()
        {
            /*
             * 디바이스(X, Y, D, M...) 기준으로 분류한 다음 개수에 따라 블록읽기와 랜덤읽기로 나눈다.
             * */
            List<List<DeviceLabel>> deviceLabelsByDeviceName = new List<List<DeviceLabel>>(); // 디바이스기준으로 라벨을 분류한다.
            List<DeviceLabel> basicBin = new List<DeviceLabel>();
            deviceLabelsByDeviceName.Add(basicBin);

            foreach (var label in _deviceLabels)
            {
                if (!basicBin.Any())
                {
                    basicBin.Add(label);
                }
                else if(basicBin.Last().Device == label.Device)
                {
                    basicBin.Add(label);
                }
                else
                {
                    basicBin = new List<DeviceLabel>() { label };
                    deviceLabelsByDeviceName.Add(basicBin);
                }
            }

            var blockBins = deviceLabelsByDeviceName.Where(x => MinimumBlockSize <= x.Count());
            
            // TODO 주소간격이 멀리 떨어진 경우 새블록or랜덤읽기 적용
            _blockRanges.AddRange(blockBins.Select(bin => new BlockRange(bin)));

            var randomBins = deviceLabelsByDeviceName.Except(blockBins);
            if(randomBins.Any())
            {
                _randomRanges.Add(new RandomRange(randomBins.SelectMany(label => label)));
            }
            
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

                foreach(var pair in readResult.Data)
                    data[pair.Key] = pair.Value;

            }
            #endregion

            #region 랜덤읽기
            foreach(var randomRange in _randomRanges)
            {
                Result<short[]> readResult = _plc.ReadRandom(randomRange.DeviceList, randomRange.Length);
                if (!readResult.IsSuccessful)
                    return Result<Dictionary<string, short>>.Fail(readResult.Message);

                int shortIndex = 0; // 수신한 short배열에서의 인덱스
                foreach (DeviceLabel label in randomRange.OrderedDeviceLabels)
                    data[label.AddressString] = readResult.Data[shortIndex++];
            }
            #endregion

            return Result<Dictionary<string, short>>.Success(data);
        }
         
    }
}
