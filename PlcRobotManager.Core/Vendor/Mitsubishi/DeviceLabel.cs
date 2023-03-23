using PlcRobotManager.Core.Vendor.Mitsubishi.Readers;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 디바이스 라벨(주소). 디바이스 및 주소정보를 가진다.
    /// </summary>
    public class DeviceLabel
    {
        public DeviceLabel(Device device, int address, int? bitPosition = null, GatheringGroup group = null)
        {
            Device = device;
            Address = address;

            if (device.IsBit())
            {
                WordAddress = address / 16;
                BitPosition = address % 16;
            }
            else
            {
                WordAddress = address;
                BitPosition = bitPosition;
            }

            WordAddressString = device.GetAddressString(WordAddress);
            AddressString = device.GetAddressString(Address, BitPosition);

            Group = group;
        }

        /// <summary>
        /// 디바이스
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// 실제 주소.
        /// 비트주소인 경우 워드주소로 수신한 값을 비트단위로 읽는다.
        /// </summary>
        public int Address { get; }

        /// <summary>
        /// 실제 주소 문자열 표현
        /// </summary>
        public string AddressString { get; }

        /// <summary>
        /// 값 조회를 위한 워드주소. 블록읽기에 사용한다.
        /// </summary>
        public int WordAddress { get; }

        /// <summary>
        /// 워드주소의 문자열 표현
        /// </summary>
        public string WordAddressString { get; }

        /// <summary>
        /// 라벨의 비트위치. 워드값을 비트로 사용할 때 적용한다.
        /// </summary>
        public int? BitPosition { get; }

        /// <summary>
        /// 수집 그룹. 매뉴얼 방식으로 데이터를 수집할 때 적용된다.
        /// </summary>
        public GatheringGroup Group { get; }

        /// <summary>
        /// 라벨의 값이 비트값인가?
        /// </summary>
        public bool IsBitValue => Device.IsBit() || BitPosition.HasValue;

        public override string ToString()
        {
            return $"{nameof(DeviceLabel)} {AddressString}";
        }
    }
}
