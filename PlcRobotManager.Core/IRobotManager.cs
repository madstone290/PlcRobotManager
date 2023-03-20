using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public interface IRobotManager
    {
        IDataManager DataManager { get; set; }

        IEnumerable<IRobot> Robots { get; }

        void SetUp(IEnumerable<IRobot> robots);

        Task RunAsync();

        Task StopAsync();

        /// <summary>
        /// 주어진 로봇의 현재 데이터를 읽어온다
        /// </summary>
        /// <param name="robotName"></param>
        /// <returns></returns>
        Dictionary<string, short> GetRobotData(string robotName);

    }
}
