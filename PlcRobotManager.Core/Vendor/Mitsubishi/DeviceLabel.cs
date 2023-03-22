namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 디바이스 라벨(주소). 디바이스 및 주소정보를 가진다.
    /// </summary>
    public class DeviceLabel
    {
        public DeviceLabel(Device device, int address, GatheringGroup group = null)
        {
            Device = device;
            Address = address;
            AddressString = device.GetAddressString(address);
            Group = group;
        }

        /// <summary>
        /// 디바이스
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// 주소
        /// </summary>
        public int Address { get; }

        public string AddressString { get; }

        /// <summary>
        /// 수집 그룹. 매뉴얼 방식으로 데이터를 수집할 때 적용된다.
        /// </summary>
        public GatheringGroup Group { get; }

    }
}
