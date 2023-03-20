namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 디바이스 라벨(주소). 디바이스 및 주소정보를 가진다.
    /// </summary>
    public class DeviceLabel
    {
        public DeviceLabel(Device device, int address)
        {
            Device = device;
            Address = address;
            AddressString = GetAddressString();
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

        private string GetAddressString()
        {
            if(Device.NumberType == NumberType.Decimal)
            {
                return Device.Name + Address.ToString().PadLeft(5, '0');
            }
            else
            {
                return Device.Name + Address.ToString("X4").PadLeft(5, '0');
            }
        }

    }
}
