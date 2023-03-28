using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Impl
{
    public class RobotManager : IRobotManager
    {
        private readonly Dictionary<string, IRobot> _robots = new Dictionary<string, IRobot>();

        public IDataManager DataManager { get; set; }

        public IEnumerable<IRobot> Robots => _robots.Values;

        public void SetUp(IEnumerable<IRobot> robots)
        {
            _robots.Clear();
            foreach (var robot in robots)
            {
                _robots.Add(robot.Name, robot);
                robot.Save += (sender, data) =>
                {
                    DataManager.Save(data);
                };
            }
        }


        public async Task RunAsync()
        {
            List<Task> tasks = new List<Task>();
            foreach (var robot in Robots)
            {
                var task = robot.RunAsync();
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
#if DEBUG
            Console.WriteLine("All robots have run");
#endif
        }



        public async Task StopAsync()
        {
            List<Task> tasks = new List<Task>();
            foreach (var robot in Robots)
            {
                var task = robot.StopAsync();
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
#if DEBUG
            Console.WriteLine("All robots have stopped");
#endif
        }

        public Dictionary<string, object> GetProcessedRobotData(string robotName)
        {
            if (robotName == null || !_robots.TryGetValue(robotName, out var robot))
                return new Dictionary<string, object>();

            return robot.GetProcessedData();
        }

        public Dictionary<string, short> GetRawRobotData(string robotName)
        {
            if (robotName == null || !_robots.TryGetValue(robotName, out var robot))
                return new Dictionary<string, short>();

            return robot.GetRawData();
        }

        public List<DeviceLabel> GetDeviceLabels(string robotName, string plcName)
        {
            if (robotName == null || !_robots.TryGetValue(robotName, out var robot))
                return new List<DeviceLabel>();

            return robot.GetDeviceLabels(plcName);
        }

        public List<string> GetPlcNames(string robotName)
        {
            if (robotName == null || !_robots.TryGetValue(robotName, out var robot))
                return new List<string>();

            return robot.GetPlcNames();
        }
    }
}
