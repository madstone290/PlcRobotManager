using PlcRobotManager.Core.Vendor.Mitsubishi;

namespace PlcRobotManager.Core.UnitTests.Mockups
{
    public class PlcMockup : IMitsubishiPlc
    {
        public Result Close()
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public Result Open()
        {
            throw new System.NotImplementedException();
        }

        public Result<short[]> ReadBlock2(string startAddress, int length)
        {
            throw new System.NotImplementedException();
        }

        public Result<int[]> ReadBlock4(string startAddress, int length)
        {
            throw new System.NotImplementedException();
        }

        public Result<short[]> ReadRandom(string randomAddress, int length)
        {
            throw new System.NotImplementedException();
        }
    }
}
