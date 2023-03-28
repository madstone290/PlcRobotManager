using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public interface IRobot
    {
        /// <summary>
        /// 이름
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 작업간 아이들 시간(밀리초). 기본값 0. 조회속도를 조절하기 위해 사용한다.
        /// </summary>
        int AdditionalIdleTime { get; set; }

        /// <summary>
        /// 매 데이터 조회후 데이터 로그를 남길지 설정한다.
        /// </summary>
        bool DataLoggingEnabled { get; set; }

        /// <summary>
        /// 몇 사이클마다 데이터를 로깅할지 설정한다. 1이상의 값만 유효하다.
        /// </summary>
        int DataLoggingCycle { get; set; }

        /// <summary>
        /// 데이터 저장 이벤트
        /// </summary>
        event EventHandler<object> Save;

        Task RunAsync();

        Task StopAsync();

        /// <summary>
        /// 가공된 데이터를 읽어온다.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetProcessedData();

        /// <summary>
        /// 원본 데이터를 읽어온다.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, short> GetRawData();
        List<string> GetPlcNames();
        List<DeviceLabel> GetDeviceLabels();
    }
}
