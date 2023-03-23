using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 디바이스 라벨(주소). 디바이스 및 주소정보를 가진다.
    /// </summary>
    public class DeviceLabel
    {
        public DeviceLabel(string code, Device device, int address, int length = 1, int? bitPosition = null, GatheringGroup group = null)
        {
            Code = code;
            Device = device;
            Address = address;
            Length = length < 1 ? 1 : length;

            if (device.IsBit())
            {
                StartWordAddress = address / 16;
                BitPosition = address % 16;
            }
            else
            {
                StartWordAddress = address;
                BitPosition = bitPosition;
            }

            StartWordAddressString = device.GetAddressString(StartWordAddress);
            AddressString = device.GetAddressString(Address, BitPosition);

            Group = group;
        }

        /// <summary>
        /// 라벨 식별을 위한 코드. 중복이 허용되지 않는다.
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// 디바이스
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// 실제 시작 주소.
        /// 비트주소인 경우 워드주소로 수신한 값을 비트단위로 읽는다.
        /// </summary>
        public int Address { get; }

        /// <summary>
        /// 실제 주소 문자열 표현
        /// </summary>
        public string AddressString { get; }

        /// <summary>
        /// 라벨에 포함된 모든 주소의 문자열
        /// </summary>
        public List<string> AddressStringList => Enumerable.Range(0, Length)
            .Select(offset => Device.GetAddressString(Address + offset, BitPosition))
            .ToList();

        /// <summary>
        /// 값 조회를 위한 시작 워드주소. 블록읽기에 사용한다.
        /// </summary>
        public int StartWordAddress { get; }

        /// <summary>
        /// 끝 워드 주소
        /// </summary>
        public int EndWordAddress => StartWordAddress + Length - 1;

        /// <summary>
        /// 워드주소의 문자열 표현
        /// </summary>
        public string StartWordAddressString { get; }

        /// <summary>
        /// 라벨의 비트위치. 워드값을 비트로 사용할 때 적용한다.
        /// </summary>
        public int? BitPosition { get; }

        /// <summary>
        /// 워드 길이
        /// </summary>
        public int Length { get; } = 1;

        /// <summary>
        /// 수집 그룹. 매뉴얼 방식으로 데이터를 수집할 때 적용된다.
        /// </summary>
        public GatheringGroup Group { get; }

        /// <summary>
        /// 라벨의 값이 비트값인가?
        /// </summary>
        public bool IsBitValue => Device.IsBit() || BitPosition.HasValue;

        /// <summary>
        /// 라벨의 데이터 타입에 맞게 값을 변환한다.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public object ConvertValue(IEnumerable<short> values)
        {
            // Demo 
            if (Length == 1)
                return values.First();
            else
                return values.First() * short.MaxValue + values.Skip(1).First();
        }

        public override string ToString()
        {
            return $"{nameof(DeviceLabel)} {AddressString}";
        }
    }
}
