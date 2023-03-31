using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public interface IRobotManager
    {
        IDataManager DataManager { get; set; }

        IEnumerable<IRobot> Robots { get; }

        event EventHandler<RobotCycleEventArgs> RobotCycleStarted;

        event EventHandler<RobotCycleEventArgs> RobotCycleEnded;

        void SetUp(IEnumerable<IRobot> robots);

        Task RunAsync();

        Task StopAsync();

        /// <summary>
        /// PLC에 등록된 라벨을 가져온다.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="plcName"></param>
        /// <returns></returns>
        List<DeviceLabel> GetDeviceLabels(string robotName, string plcName);

        /// <summary>
        /// 로봇에 등록된 PLC목록을 가져온다.
        /// </summary>
        /// <param name="robotName"></param>
        /// <returns></returns>
        List<string> GetPlcNames(string robotName);

        /// <summary>
        /// 주어진 로봇의 가공 데이터를 읽어온다
        /// </summary>
        /// <param name="robotName"></param>
        /// <returns></returns>
        Dictionary<string, object> GetProcessedRobotData(string robotName);

        /// <summary>
        /// 주어진 로봇의 원본 데이터를 읽어온다
        /// </summary>
        /// <param name="robotName"></param>
        /// <returns></returns>
        Dictionary<string, short> GetRawRobotData(string robotName);
    }
}
