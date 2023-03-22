using FluentAssertions;
using PlcRobotManager.Core.Vendor.Mitsubishi;
using PlcRobotManager.Core.Vendor.Mitsubishi.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlcRobotManager.Core.UnitTests
{
    public class BlockRangeTests
    {
        /// <summary>
        /// 비트블록범위는 워드블록의 1/16 크기이다.
        /// </summary>
        /// <param name="address">시작 비트 주소</param>
        /// <param name="count">비트 개수</param>
        [Theory]
        [InlineData(0, 30)]
        [InlineData(0, 56)]
        [InlineData(3, 30)]
        [InlineData(15, 45)]
        public void BitBlock_Size_DividedBy_16(int address, int count)
        {
            IEnumerable<DeviceLabel> labels = Enumerable.Range(0, count).Select(i => new DeviceLabel(Device.X, address + i));
            int expectedLength = (address + count - 1) / 16 + 1;

            BlockRange blockRange = new BlockRange(labels);


            blockRange.Length.Should().Be(expectedLength);
        }
    }
}
