namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    public interface IMitsubishiPlc
    {
        /// <summary>
        /// PLC 이름
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 초기화 작업을 수행한다
        /// </summary>
        void Initialize();

        /// <summary>
        /// 연결 열기
        /// </summary>
        /// <returns></returns>
        Result Open();

        /// <summary>
        /// 2바이트 블록읽기
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Result<short[]> ReadBlock2(string startAddress, int length);

        /// <summary>
        /// 4바이트 블록읽기
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Result<int[]> ReadBlock4(string startAddress, int length);

        /// <summary>
        /// 랜덤 읽기
        /// </summary>
        /// <param name="randomAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Result<short[]> ReadRandom(string randomAddress, int length);

        /// <summary>
        /// 연결 종료
        /// </summary>
        /// <returns></returns>
        Result Close();
        
    }
}
