using PlcRobotManager.Core.Extensions;
using PlcRobotManager.Core.Impl;
using PlcRobotManager.Core.Infos;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public static class RobotManagerHelper
    {
        public static async Task<IRobotManager> RunPredefined()
        {
            int codeNumber = 1;
            Func<string> getUniqueId = new Func<string>(() => (codeNumber++).ToString().PadLeft(5, '0'));

            List<RobotInfo> robotInfos = new List<RobotInfo>()
            {
                new RobotInfo()
                {
                    Name = "Robot #1",
                    DataLoggingCycle = 10,
                    DataLoggingEnabled = true,
                    AdditionalIdleTime = 5 * 1000,
                    GathererType = DataGathererType.Manual,
                    PlcInfos = new List<PlcInfo>()
                    {
                        new PlcInfo()
                        {
                            Name = "Plc #1",
                            ActTargetSimulator = 1,
                            DeviceLabelInfos = new List<DeviceLabelInfo>()
                            {
                                new DeviceLabelInfo() // 공정1
                                {
                                    Code = "Process #1 Cycle",
                                    DeviceName = "D",
                                    Address = 0,
                                    DataType = DataType.Number,
                                    Length = 1,
                                    BitPosition = null,

                                    GatheringGroupName = "GroupD",
                                    GatheringGroupRangeType = RangeType.Block,

                                    SubroutineName = "Process #1",
                                    SubroutineDetectionType = SubroutineDetectionType.CycleTime,
                                },
                                new DeviceLabelInfo() // 공정2
                                {
                                    Code = "Process #2 Cycle",
                                    DeviceName = "D",
                                    Address = 1,
                                    DataType = DataType.Number,
                                    Length = 1,
                                    GatheringGroupName = "GroupD",
                                    GatheringGroupRangeType = RangeType.Block,
                                    SubroutineName = "Process #2",
                                    SubroutineDetectionType = SubroutineDetectionType.CycleTime,
                                }
                            }
                            .Concat(StepRange.For(0, 50).Select(i => // 길이 1짜리 D주소 50개
                                new DeviceLabelInfo()
                                {
                                    Code = getUniqueId(),
                                    DataType = DataType.Number,
                                    DeviceName= "D",
                                    Address = i,
                                    Length = 1,
                                    GatheringGroupName = "GroupD",
                                    GatheringGroupRangeType = RangeType.Block,
                                }))
                            .Concat(StepRange.For(50, 25, 2).Select(i => // 길이 2짜리 D주소 25개
                                new DeviceLabelInfo()
                                {
                                    Code = getUniqueId(),
                                    DataType = DataType.Number,
                                    DeviceName= "D",
                                    Address = i,
                                    Length = 2,
                                    GatheringGroupName = "GroupD",
                                    GatheringGroupRangeType = RangeType.Block,
                                }))
                            .Concat(StepRange.For(100, 10).SelectMany(i => // 비트 D주소. 100~109. 각 주소당 16개 비트.
                            {
                                return StepRange.For(0, 16).Select(bit =>
                                    new DeviceLabelInfo()
                                    {
                                        Code = getUniqueId(),
                                        DataType = DataType.Bit,
                                        DeviceName = "D",
                                        Address = i,
                                        BitPosition = bit % 16,
                                        GatheringGroupName = "GroupD",
                                        GatheringGroupRangeType = RangeType.Block,
                                    });
                            }))
                            .Concat(StepRange.For(110, 2, 10).Select(i => // D110부터 길이10 문자열 2개
                                new DeviceLabelInfo()
                                {
                                    Code = getUniqueId(),
                                    DataType = DataType.String,
                                    DeviceName = "D",
                                    Address = i,
                                    Length = 10,
                                    GatheringGroupName = "GroupR",
                                    GatheringGroupRangeType = RangeType.Random,
                                }))
                            .Concat(StepRange.For(0, 100).Select(i => 
                                new DeviceLabelInfo()
                                {
                                    Code = getUniqueId(),
                                    DataType = DataType.Bit,
                                    DeviceName = "X",
                                    Address = i,
                                    GatheringGroupName = "GroupX",
                                    GatheringGroupRangeType = RangeType.Block,
                                }))
                            .Concat(StepRange.For(3, 100).Select(i =>
                                new DeviceLabelInfo()
                                {
                                    Code = getUniqueId(),
                                    DataType = DataType.Bit,
                                    DeviceName = "M",
                                    Address = i,
                                    GatheringGroupName = "GroupM",
                                    GatheringGroupRangeType = RangeType.Block,
                                }))
                            .ToList()
                        }
                    }
                }
            };
            return await Run(robotInfos);
        }

        public static async Task<IRobotManager> Run(IEnumerable<RobotInfo> robotInfos, IDataManager dataManager = null)
        {
            IRobotManager robotManager = new RobotManager
            {
                DataManager = dataManager ?? new DummyDataManager()
            };

            Dictionary<string, IMitsubishiPlc> plcCache = new Dictionary<string, IMitsubishiPlc>();

            List<IRobot> robots = robotInfos.Select(robotInfo =>
            {
                PlcInfo plcInfo = robotInfo.PlcInfos.First();
                if (!plcCache.TryGetValue(plcInfo.Name, out IMitsubishiPlc plc))
                {
                    plc = new MitsubishiPlc(plcInfo.Name, new ProgOptions()
                    {
                        ActTargetSimulator = plcInfo.ActTargetSimulator,
                    });
                    plcCache.Add(plcInfo.Name, plc);
                }

                List<GatheringGroup> groups = plcInfo.DeviceLabelInfos
                    .GroupBy(labelInfo => labelInfo.GatheringGroupName)
                    .Select(group => new GatheringGroup(group.Key, group.First().GatheringGroupRangeType))
                    .ToList();

                List<DeviceLabel> deviceLabels = plcInfo.DeviceLabelInfos.Select(label =>
                    new DeviceLabel(label.Code,
                        Device.FromName(label.DeviceName),
                        label.Address,
                        dataType: label.DataType,
                        length: label.Length,
                        bitPosition: label.BitPosition,
                        group: groups.FirstOrDefault(group => group.Name == label.GatheringGroupName),
                        subroutine: string.IsNullOrWhiteSpace(label.SubroutineName)
                            ? null
                            : new DeviceLabel.SubroutineInfo(label.SubroutineName,
                                                         label.SubroutineDetectionType,
                                                         label.SubroutineIsStart,
                                                         label.SubroutineIsEnd)
                    )).ToList();

                return new MitsubishiRobot(robotInfo.Name, plc, robotInfo.GathererType, deviceLabels)
                {
                    AdditionalIdleTime = robotInfo.AdditionalIdleTime,
                    DataLoggingEnabled = robotInfo.DataLoggingEnabled,
                    DataLoggingCycle = robotInfo.DataLoggingCycle
                };
            }).Cast<IRobot>().ToList();

            robotManager.SetUp(robots);
            await robotManager.RunAsync();

            return robotManager;
        }

        public static async Task Stop(IRobotManager robotManager)
        {
            await robotManager.StopAsync();

            Console.WriteLine("Robot manager is exiting");

            foreach (var robot in robotManager.Robots.Cast<MitsubishiRobot>())
            {
                Console.WriteLine($"Robot: {robot.Name} has count: {robot.TotalReadCount}");
            }
        }
    }
}
