using PlcRobotManager.Core.Impl;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public static class Demo
    {
        public static async Task<IRobotManager> Run(int robotCount = 1)
        {
            IDataManager dataManager = new DummyDataManager();
            IRobotManager robotManager = new RobotManager();
            robotManager.DataManager = dataManager;

            IMitsubishiPlc mitsubishiPlc = new MitsubishiPlc("Plc1", new ProgOptions()
            {
                ActTargetSimulator = 1
            });

            var dLabels = Enumerable.Range(0, 300).Select(i => new DeviceLabel(Device.D, i * 2));
            var mLabels = Enumerable.Range(0, 300).Select(i => new DeviceLabel(Device.M, i * 2));
            var xLabels = Enumerable.Range(0, 50).Select(i => new DeviceLabel(Device.X, i * 2));
            var yLabels = Enumerable.Range(0, 50).Select(i => new DeviceLabel(Device.Y, i * 2));
            var allLabels = dLabels.Concat(mLabels).Concat(xLabels).Concat(yLabels).ToList();

            var robots = Enumerable.Range(1, robotCount).Select(x => new MitsubishiRobot(x.ToString(), mitsubishiPlc, DataGathererType.Auto, allLabels)
            {
                AdditionalIdleTime = 1000,
                DataLoggingEnabled = true,
                DataLoggingCycle = 1
            }).ToList();
            robotManager.SetUp(robots);
            await robotManager.RunAsync();

            return robotManager;
        }

        public static async Task Stop(IRobotManager robotManager)
        {

            await robotManager.StopAsync();

            Console.WriteLine("Iam exiting");

            foreach (var robot in robotManager.Robots.Cast<MitsubishiRobot>())
            {
                Console.WriteLine($"{robot.Name} : {robot.TotalReadCount}");
            }
        }
    }
}
