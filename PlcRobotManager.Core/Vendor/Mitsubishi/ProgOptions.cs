namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// ActProgType에 사용되는 속성값의 옵션
    /// </summary>
    public class ProgOptions
    {
        /// <summary>
        /// 시뮬레이터에 연결시 사용할 목적지 번호.
        /// <para> GX2 사용시 0(사용안함), 1(시뮬레이터 A), 2(시뮬레이터 B), 3(시뮬레이터 C), 4(시뮬레이터 D) 설정.</para>
        /// <para>MT2 사용시 2(시뮬레이터 B), 3(시뮬레이터 C), 4(시뮬레이터 D) 설정</para>
        /// </summary>
        public int ActTargetSimulator { get; set; }


        /// <summary>
        /// 물리적 포트에 연결되어 있는 모듈의 타입. ** 타입이 맞지 않는 경우 열기동작이 실패한다 **
        /// <para>매뉴얼을 참조하여 올바른 값을 설정할 것</para>
        /// <para>시뮬레이터 연결 --- GX1: 0x0B, GX2: 0x30, MT2: 0x30</para>
        /// </summary>
        public int ActUnitType { get; set; } = 0x30;

        /// <summary>
        /// 통신 프로토콜. 일반적으로 0x05(TCP/IP)를 사용한다.
        /// </summary>
        public int ActProtocolType { get; set; } = 0x05;

    }
}
