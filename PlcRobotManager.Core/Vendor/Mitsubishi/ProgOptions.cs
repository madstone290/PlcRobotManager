using System;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// ActProgType에 사용되는 속성값의 옵션
    /// </summary>
    public class ProgOptions
    {
        public CommunicationType CommunicationType { get; set; } = CommunicationType.GXSimulator2;

        /// <summary>
        /// 시뮬레이터에 연결시 사용할 목적지 번호.
        /// <para> GX2 사용시 0(사용안함), 1(시뮬레이터 A), 2(시뮬레이터 B), 3(시뮬레이터 C), 4(시뮬레이터 D) 설정.</para>
        /// <para>MT2 사용시 2(시뮬레이터 B), 3(시뮬레이터 C), 4(시뮬레이터 D) 설정</para>
        /// </summary>
        public int ActTargetSimulator { get; set; } = 1;

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

        /// <summary>
        /// 호스트 IP주소
        /// </summary>
        public string ActHostAddress { get; set; } = string.Empty;

        /// <summary>
        /// 호스트 포트번호
        /// </summary>
        public int ActDestinationPortNumber { get; set; } = 5002;

        /// <summary>
        /// MELSEC 네트워크 번호
        /// </summary>
        public int ActNetworkNumber { get; set; } = 1;

        /// <summary>
        /// MELSEC 국번
        /// </summary>
        public int ActStationNumber { get; set; } = 1;

        /// <summary>
        /// QE71, Q 시리즈 E71연결시 소스(PC측) 네트워크 번호를 지정합니다.
        /// </summary>
        public int ActSourceNetworkNumber { get; set; } = 0x00;

        /// <summary>
        ///  QE71, Q 시리즈 E71연결시 소스(PC측) 국번을 지정합니다.
        /// </summary>
        public int ActSourceStationNumber { get; set; } = 0x00;

        /// <summary>
        /// 통신 대상국의 CPU 타입. 메뉴얼에서 CPU타입에 맞는 값 검색 후 설정할 것.
        /// </summary>
        public int ActCpuType { get; set; }

        /// <summary>
        /// PC 와 PLC/인버터 간의 통신의 타임 아웃값(ms)
        /// </summary>
        public int ActTimeOut { get; set; }

        /// <summary>
        /// 시리얼 커뮤니케이션 모듈 , QE71 및 Q 시리즈 대응 E71 의 모듈 번호를 지정합니다.
        /// <para>
        /// 멀티 드롭 링크 시는 요구 소스의 시리얼 커뮤니케이션 모듈의 모듈 번호를 지정합니다.
        /// 다만 CPU COM 통신 경유 멀티 드롭 링크의 경우에는 요구 소스국의 모듈 번호가 필요
        /// 하지 않습니다 . ("0"(0x00) 지정 )
        /// </para>
        /// <para>
        /// 멀티 드롭 링크 이외의 경우 , "0"(0x00) 을 지정합니다.
        /// QE71 및 Q 시리즈 대응 E71 의 경우 , 중계 대상 국번을 지정합니다 .
        /// </para>
        /// <para>
        /// (자네트워크 액세스의 경우, "0"(0x00) 고정 )
        /// MELSECNET/10 경유로 다른 네트워크에 액세스하는 경우 , 접속 Ethernet 모듈의 파라
        /// 미터에서 설정한 국번을 지정합니다
        /// </para>
        /// </summary>
        public int ActConnectUnitNumber { get; set; } = 0x00;

        /// <summary>
        /// 멀티 드롭 접속시, 최종 액세스 대상국의 실제 입출력 번호(선두 입출력 ÷16)를 지정합니다.
        ///  교신 대상이 CPU 인 경우, 0x03FF 을 지정합니다
        /// </summary>
        public int ActDestinationIONumber { get; set; } = 0x00;

        public int ActDidPropertyBit { get; set; } = 0x01;

        public int ActDsidPropertyBit { get; set; } = 0x01;

        /// <summary>
        /// 모듈 IO번호를 지정한다.
        /// </summary>
        public int ActIONumber { get; set; } = 0x03FF;

        public int ActMultiDropChannelNumber { get; set; } = 0x00;

        /// <summary>
        /// 패스워드 잠금이 가능한 다음의 모듈에서 설정한 패스워드를 해제하기 위하여 패스워드를 지정합니다.
        /// R C24, Q시리즈 C24, Q시리즈 E71, Q시리즈 CMO
        /// </summary>
        public string ActPassword { get; set; } = null;

        /// <summary>
        /// 네트워크를 경유하여 다른 국에 액세스하는 경우, 경유 네트워크에 MELSECNET/10 이 포함되어 있는지 여부를 지정합니다.
        /// MELSECNET/10 비포함: 0x00, 포함: 0x01.
        /// </summary>
        public int ActThroughNetworkType { get; set; } = 0x00;

        public int ActUnitNumber { get; set; } = 0x00;

        /// <summary>
        /// PLC통신타입에 맞게 유효한 값으로 변경된 새 옵션을 반환한다.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public ProgOptions Validate()
        {
            switch (CommunicationType)
            {
                case CommunicationType.QSeriesE71Tcp:
                    return QSeriesE71Tcp(ActCpuType,
                                         ActHostAddress,
                                         ActNetworkNumber,
                                         ActStationNumber,
                                         ActSourceNetworkNumber,
                                         ActSourceStationNumber,
                                         actConnectUnitNumber: ActConnectUnitNumber,
                                         actDestinationIONumber: ActDestinationIONumber,
                                         actDestinationPortNumber: ActDestinationPortNumber,
                                         actDidPropertyBit: ActDidPropertyBit,
                                         actDsidPropertyBit: ActDsidPropertyBit,
                                         actIONumber: ActIONumber,
                                         actPassword: ActPassword,
                                         actThroughNetworkType: ActThroughNetworkType,
                                         actTimeOut: ActTimeOut,
                                         actUnitNumber: ActUnitNumber);
                case CommunicationType.GXSimulator2:
                    return GXSimulator2(ActTargetSimulator);
                case CommunicationType.EthernetAdapterTcp:
                    return EthernetAdapterTcp(ActCpuType, ActHostAddress, actTimeOut: ActTimeOut);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Q시리즈 E71(이더넷모듈)에 TCP통신을 위한 옵션을 생성한다.
        /// </summary>
        /// <param name="actCpuType">대상국의 Cpu타입</param>
        /// <param name="actHostAddress">대상의 IP주소</param>
        /// <param name="actNetworkNumber">대상국 모듈의 네트워크 번호</param>
        /// <param name="actStationNumber">대상국 모듈 국번</param>
        /// <param name="actSourceNetworkNumber">PC네트워크 번호</param>
        /// <param name="actSourceStationNumber">PC 국번</param>
        /// <param name="actConnectUnitNumber"></param>
        /// <param name="actDestinationIONumber"></param>
        /// <param name="actDestinationPortNumber">포트번호</param>
        /// <param name="actDidPropertyBit"></param>
        /// <param name="actDsidPropertyBit"></param>
        /// <param name="actIONumber"></param>
        /// <param name="actPassword"></param>
        /// <param name="actThroughNetworkType"></param>
        /// <param name="actTimeOut">통신 타임아웃</param>
        /// <param name="actUnitNumber"></param>
        /// <returns></returns>
        public static ProgOptions QSeriesE71Tcp(int actCpuType,
            string actHostAddress,
            int actNetworkNumber,
            int actStationNumber,
            int actSourceNetworkNumber,
            int actSourceStationNumber,
            int actConnectUnitNumber = 0x00,
            int actDestinationIONumber = 0x00,
            int actDestinationPortNumber = 5002,
            int actDidPropertyBit = 0x01,
            int actDsidPropertyBit = 0x01,
            int actIONumber = 0x03FF,
            string actPassword = null,
            int actThroughNetworkType = 0x00,
            int actTimeOut = 0,
            int actUnitNumber = 0)
        {
            return new ProgOptions()
            {
                ActCpuType = actCpuType,
                ActHostAddress = actHostAddress,
                ActNetworkNumber = actNetworkNumber,
                ActStationNumber = actStationNumber,
                ActSourceNetworkNumber = actSourceNetworkNumber,
                ActSourceStationNumber = actSourceStationNumber,
                ActProtocolType = 0x05,
                ActMultiDropChannelNumber = 0x00,
                ActUnitType = 0x1A,
                ActConnectUnitNumber = actConnectUnitNumber,
                ActDestinationIONumber = actDestinationIONumber,
                ActDestinationPortNumber = actDestinationPortNumber,
                ActDidPropertyBit = actDidPropertyBit,
                ActDsidPropertyBit = actDsidPropertyBit,
                ActIONumber = actIONumber,
                ActPassword = actPassword,
                ActThroughNetworkType = actThroughNetworkType,
                ActTimeOut = actTimeOut,
                ActUnitNumber = actUnitNumber,
            };
        }

        /// <summary>
        /// 이더넷 어댑터 통신을 위한 옵션을 생성한다.
        /// <para>
        /// *** 주의 *** 이더넷 어댑터 모듈과 다르다
        /// </para> 
        /// </summary>
        /// <param name="actCpuType">대상국의 Cpu타입</param>
        /// <param name="actHostAddress">대상의 IP주소</param>
        /// <param name="actTimeOut">통신 타임아웃</param>
        /// <returns></returns>
        public static ProgOptions EthernetAdapterTcp(int actCpuType, string actHostAddress,
            int actTimeOut = 0)
        {
            return new ProgOptions()
            {
                ActCpuType = actCpuType,
                ActHostAddress = actHostAddress,
                ActProtocolType = 0x05, // PROTOCOL_TCPIP
                ActUnitType = 0x4A, // UNIT_FXETHER
                ActTimeOut = actTimeOut
            };
        }

        /// <summary>
        /// GX Simulator1 통신을 위한 옵션을 생성한다.
        /// </summary>
        /// <param name="actCpuType">대상국 CPU타입</param>
        /// <param name="actNetworkNumber">자국인 경우 0x00, 다른 국인 경우 대상국의 네트워크 번호</param>
        /// <param name="actStationNumber">자국인 경우  0xFF, 다른 국인 경우 대상국의 국번</param>
        /// <param name="actTimeOut">통신 타임아웃</param>
        /// <returns>옵션</returns>
        public static ProgOptions GX1Simulator(int actCpuType, int actNetworkNumber = 0x00,
            int actStationNumber = 0xFF, int actTimeOut = 0)
        {
            return new ProgOptions()
            {
                ActCpuType = actCpuType,
                ActNetworkNumber = actNetworkNumber,
                ActProtocolType = 0x06,
                ActStationNumber = actStationNumber,
                ActTimeOut = actTimeOut,
                ActUnitType = 0x0B,
            };
        }

        /// <summary>
        /// GX Simulator2 통신을 위한 옵션을 생성한다.
        /// </summary>
        /// <param name="actTargetSimulator">시뮬레이터 번호</param>
        /// <returns></returns>
        public static ProgOptions GXSimulator2(int actTargetSimulator)
        {
            return new ProgOptions()
            {
                ActTargetSimulator = actTargetSimulator,
                ActUnitType = 0x30,
            };
        }

        /// <summary>
        /// MT Simulator2 통신을 위한 옵션을 생성한다.
        /// </summary>
        /// <param name="actTargetSimulator">시뮬레이터 번호</param>
        /// <param name="actCpuType">대상국의 Cpu 타입</param>
        /// <returns></returns>
        public static ProgOptions MTSimulator2(int actTargetSimulator, int actCpuType)
        {
            return new ProgOptions()
            {
                ActTargetSimulator = actTargetSimulator,
                ActCpuType = actCpuType,
                ActUnitType = 0x30,
            };
        }

        /// <summary>
        /// 메뉴얼 4.6 MELSECNET/H 통신
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static ProgOptions MelsecNetH()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 메뉴얼 4.7  CC-Link IE 컨트롤러 네트워크 통신
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static ProgOptions CCLinkIEControllerNetwork()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 메뉴얼 4.8  CC-Link IE 필드 네트워크 통신
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static ProgOptions CCLinkIEFieldNetwork()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 메뉴얼 4.9 CC Link 통신
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static ProgOptions CCLink()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 메뉴얼 4.10 CC Link G4 통신
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static ProgOptions CCLinkG4()
        {
            throw new NotSupportedException();
        }



    }
}
