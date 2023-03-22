using FluentAssertions;
using PlcRobotManager.Core.UnitTests.Mockups;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers;
using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace PlcRobotManager.Core.UnitTests
{
    public class AutoDataGathererSortTests
    {

        /// <summary>
        /// 1개의 디바이스를 1개의 블록으로 분류한다.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="startAddress"></param>
        /// <param name="endAddress"></param>
        /// <param name="maxBlockSize"></param>
        /// <param name="minLabelCount"></param>
        [Theory]
        [InlineData("D", 0, 100, 1000, 50)]
        [InlineData("D", 100, 300, 1000, 50)]
        [InlineData("D", 0, 50, 1000, 50)]
        [InlineData("X", 0, 64, 1000, 50)]
        [InlineData("Y", 0, 64, 1000, 50)]
        public void Sort_SingleDevice_1Block(string device, int startAddress, int count, int maxBlockSize, int minLabelCount)
        {
            var sorter = new AutoDataGatherer.Sorter();
            int address = startAddress;
            var labels = Enumerable.Range(0, count)
                .Select(x => new DeviceLabel(Device.FromName(device), address++));


            Tuple<List<BlockRange>, List<RandomRange>> ranges = sorter.Sort(labels, maxBlockSize, minLabelCount);

            ranges.Item1.Should().HaveCount(1);
            ranges.Item1.First().Start.Address.Should().Be(startAddress);
            if (ranges.Item1.First().IsBitBlock)
                ranges.Item1.First().Length.Should().Be((count - 1) / 16 + 1);
            else
                ranges.Item1.First().Length.Should().Be(count);

            ranges.Item2.Should().BeEmpty();
        }

        /// <summary>
        /// 여러개의 블록으로 분류한다.
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="startAddresses"></param>
        /// <param name="count"></param>
        /// <param name="maxBlockSize"></param>
        /// <param name="minLabelCount"></param>
        /// <param name="expBlockCount"></param>
        [Theory]
        [InlineData(new object[] { new string[] { "D", "M" }, new int[] { 0, 0 }, new int[] { 1500, 2500 }, 1000, 50, new int[] { 2, 3 } })]
        public void Sort_Multiple_Blocks(string[] devices, int[] startAddresses, int[] count, int maxBlockSize, int minLabelCount, int[] expBlockCount)
        {
            var sorter = new AutoDataGatherer.Sorter();
            var labels = new List<DeviceLabel>();

            for (int i = 0; i < devices.Length; i++)
            {
                Device device = Device.FromName(devices[i]);
                int address = startAddresses[i];
                for (int j = 0; j < count[i]; j++)
                {
                    labels.Add(new DeviceLabel(device, address++));
                }

            }

            Tuple<List<BlockRange>, List<RandomRange>> ranges = sorter.Sort(labels, maxBlockSize, minLabelCount);


            for (int i = 0; i < devices.Length; i++)
            {
                var deviceRanges = ranges.Item1.Where(x => x.Start.Device == Device.FromName(devices[i]));
                deviceRanges.Count().Should().Be(expBlockCount[i]);

                int remaining = count[i];
                foreach (var range in deviceRanges)
                {
                    int expected = Math.Min(remaining, maxBlockSize);
                    if (range.IsBitBlock)
                        range.Length.Should().Be((expected - 1) / 16 + 1);
                    else
                        range.Length.Should().Be(expected);
                    remaining -= expected;
                }


            }

            ranges.Item2.Should().BeEmpty();

        }
    }
}
