using PlcRobotManager.Core.Impl;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Linq;
using System.Reflection.Emit;
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

            //var dLabels = Enumerable.Range(0, 300).Select(i => new DeviceLabel(Device.D, i * 2));
            //var mLabels = Enumerable.Range(0, 300).Select(i => new DeviceLabel(Device.M, i * 2));
            //var xLabels = Enumerable.Range(0, 50).Select(i => new DeviceLabel(Device.X, i * 2));
            //var yLabels = Enumerable.Range(0, 50).Select(i => new DeviceLabel(Device.Y, i * 2));
            //var allLabels = dLabels.Concat(mLabels).Concat(xLabels).Concat(yLabels).ToList();

            GatheringGroup group1 = new GatheringGroup("블록1", RangeType.Block);
            GatheringGroup group2 = new GatheringGroup("블록2", RangeType.Block);
            GatheringGroup group3 = new GatheringGroup("블록3", RangeType.Block);
            GatheringGroup group4 = new GatheringGroup("랜덤1", RangeType.Random);
            var groupLabels1 = Enumerable.Range(0, 200).Select(i => new DeviceLabel(Device.D, i * 2, group1));
            var groupLabels2 = Enumerable.Range(0, 200).Select(i => new DeviceLabel(Device.X, i * 2, group2));
            var groupLabels3 = Enumerable.Range(0, 200).Select(i => new DeviceLabel(Device.Y, i * 2, group3));
            var groupLabels4 = Enumerable.Range(0, 200).Select(i =>
            {
                if (i % 3 == 0)
                    return new DeviceLabel(Device.M, i * 3, group4);
                else if(i % 3 == 1)
                    return new DeviceLabel(Device.L, i * 3, group4);
                else
                    return new DeviceLabel(Device.Y, i * 3, group4);
            });
            var allLabels = groupLabels1.Concat(groupLabels2).Concat(groupLabels3).Concat(groupLabels4).ToList();

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
