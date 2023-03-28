﻿using PlcRobotManager.Core.Impl;
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
                ActTargetSimulator = 1,
            });



            GatheringGroup group1 = new GatheringGroup("블록1", RangeType.Random);
            GatheringGroup group2 = new GatheringGroup("블록2", RangeType.Block);
            GatheringGroup group3 = new GatheringGroup("블록3", RangeType.Random);
            GatheringGroup group4 = new GatheringGroup("랜덤1", RangeType.Random);



            int codeNumber = 1;
            Func<string> getUniqueId = new Func<string>(() => (codeNumber++).ToString().PadLeft(5, '0'));
            var routines = new DeviceLabel[]
            {
                new DeviceLabel("공정1사이클타임", Device.D, 0, dataType: DataType.Number, length: 1, group: group1,
                        subroutine: new DeviceLabel.SubroutineInfo("테스트공정1", SubroutineDetectionType.CycleTime)),
                new DeviceLabel("공정2사이클타임", Device.D, 1, dataType: DataType.Number, length: 1, group: group1,
                        subroutine: new DeviceLabel.SubroutineInfo("테스트공정2", SubroutineDetectionType.CycleTime)),
            };
            var groupLabels1 = Enumerable.Range(0, 100).Select(i => new DeviceLabel(getUniqueId(), Device.D, i * 2, dataType: DataType.String, length: 2, group: group1));
            var groupLabels2 = Enumerable.Range(5, 100).Select(i => new DeviceLabel(getUniqueId(), Device.X, i, dataType: DataType.Bit, group: group2));
            var groupLabels3 = Enumerable.Range(5, 100).Select(i => new DeviceLabel(getUniqueId(), Device.D, i, bitPosition: (i % 16), group: group1));
            var groupLabels4 = Enumerable.Range(0, 100).Select(i =>
            {
                if (i % 3 == 0)
                    return new DeviceLabel(getUniqueId(), Device.M, i * 3, group: group4);
                else if (i % 3 == 1)
                    return new DeviceLabel(getUniqueId(), Device.L, i * 3, group: group4);
                else
                    return new DeviceLabel(getUniqueId(), Device.Y, i * 3, group: group4);
            });
            var allLabels = routines.Concat(groupLabels1).Concat(groupLabels2).Concat(groupLabels3).Concat(groupLabels4).ToList();

            var robots = Enumerable.Range(1, robotCount).Select(x => new MitsubishiRobot(x.ToString(), mitsubishiPlc, DataGathererType.Manual, allLabels)
            {
                AdditionalIdleTime = 5 * 1000,
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
