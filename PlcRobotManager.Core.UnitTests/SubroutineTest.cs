using FluentAssertions;
using PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlcRobotManager.Core.UnitTests
{
    public class SubroutineTest
    {

        [Fact]
        public void Detect_Subrounte_Start()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int cycleCount = 0;
            subroutine.CycleStarted += (s, e) =>
            {
                cycleCount = e;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act
            subroutine.CheckCycle(data);
            data[key] = 5;
            subroutine.CheckCycle(data);

            // Assert
            cycleCount.Should().Be(initialCount);

        }

        [Fact]
        public void Detect_Subrounte_Working()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int cycleCount = 0;
            bool isWorking = false;
            subroutine.CycleStarted += (s, e) =>
            {
                cycleCount = e;
                isWorking = true;
            };
            subroutine.CycleEnded += (s, e) =>
            {
                isWorking = false;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act
            subroutine.CheckCycle(data);
            data[key] = 5;
            subroutine.CheckCycle(data);

            // Assert
            isWorking.Should().Be(true);
        }

        [Fact]
        public void Detect_Subrounte_End()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int startCycleCount = 0;
            int endCycleCount = 0;
            bool isWorking = false;
            subroutine.CycleStarted += (s, e) =>
            {
                startCycleCount = e;
                isWorking = true;
            };
            subroutine.CycleEnded += (s, e) =>
            {
                endCycleCount = e;
                isWorking = false;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act
            subroutine.CheckCycle(data);
            data[key] = 5;
            subroutine.CheckCycle(data);
            data[key] = 5;
            subroutine.CheckCycle(data);

            // Assert
            isWorking.Should().Be(false);
            endCycleCount.Should().Be(initialCount);
        }

        [Fact]
        public void Detect_Subrounte_EndStart()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int startCycleCount = 0;
            int endCycleCount = 0;
            bool isWorking = false;
            subroutine.CycleStarted += (s, e) =>
            {
                startCycleCount = e;
                isWorking = true;
            };
            subroutine.CycleEnded += (s, e) =>
            {
                endCycleCount = e;
                isWorking = false;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act
            subroutine.CheckCycle(data);
            data[key] = 5;
            subroutine.CheckCycle(data);
            data[key] = 0.3;
            subroutine.CheckCycle(data);

            // Assert
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount);
            startCycleCount.Should().Be(initialCount + 1);
        }


        [Fact]
        public void Detect_Subrounte_Start_From_Zero()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int startCycleCount = 0;
            int endCycleCount = 0;
            bool isWorking = false;
            subroutine.CycleStarted += (s, e) =>
            {
                startCycleCount = e;
                isWorking = true;
            };
            subroutine.CycleEnded += (s, e) =>
            {
                endCycleCount = e;
                isWorking = false;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act
            subroutine.CheckCycle(data);
            data[key] = 0;
            subroutine.CheckCycle(data);
            data[key] = 5;
            subroutine.CheckCycle(data); // 1s
            data[key] = 0.3;
            subroutine.CheckCycle(data); // 1e 2s
            data[key] = 5.3;
            subroutine.CheckCycle(data); 
            data[key] = 5.3;
            subroutine.CheckCycle(data); // 2e
            data[key] = 0;
            subroutine.CheckCycle(data); // 3s
            data[key] = 0.3;
            subroutine.CheckCycle(data);
            data[key] = 5.1;
            subroutine.CheckCycle(data);
            data[key] = 5.1;
            subroutine.CheckCycle(data); // 3e 
            data[key] = 0.1;
            subroutine.CheckCycle(data); // 4s


            // Assert
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 2);
            startCycleCount.Should().Be(initialCount + 3);
        }

        [Fact]
        public void Detect_Subrounte_Start_From_Non_Zero()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int startCycleCount = 0;
            int endCycleCount = 0;
            bool isWorking = false;
            subroutine.CycleStarted += (s, e) =>
            {
                startCycleCount = e;
                isWorking = true;
            };
            subroutine.CycleEnded += (s, e) =>
            {
                endCycleCount = e;
                isWorking = false;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act
            subroutine.CheckCycle(data);
            data[key] = 3;
            subroutine.CheckCycle(data); // 1s
            data[key] = 0.3;
            subroutine.CheckCycle(data); // 1e 2s
            data[key] = 5.3;
            subroutine.CheckCycle(data);
            data[key] = 5.3;
            subroutine.CheckCycle(data); // 2e
            data[key] = 0;
            subroutine.CheckCycle(data); // 3s
            data[key] = 0.3;
            subroutine.CheckCycle(data);
            data[key] = 5.1;
            subroutine.CheckCycle(data);
            data[key] = 5.1;
            subroutine.CheckCycle(data); // 3e 
            data[key] = 0.1;
            subroutine.CheckCycle(data); // 4s


            // Assert
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 2);
            startCycleCount.Should().Be(initialCount + 3);
        }


        [Fact]
        public void Detect_Subrounte_Each_Step()
        {
            // Arrange
            int initialCount = 3;
            string key = "cycle_time";
            CycleTimeSubroutine subroutine = new CycleTimeSubroutine("test", key, initialCount);
            int startCycleCount = initialCount;
            int endCycleCount = initialCount;
            bool isWorking = false;
            subroutine.CycleStarted += (s, e) =>
            {
                startCycleCount = e;
                isWorking = true;
            };
            subroutine.CycleEnded += (s, e) =>
            {
                endCycleCount = e;
                isWorking = false;
            };
            var data = new Dictionary<string, object>()
            {
                { key, 0 }
            };

            // Act & Assert
            subroutine.CheckCycle(data);

            data[key] = 3;
            subroutine.CheckCycle(data); // 1s
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 0);
            startCycleCount.Should().Be(initialCount + 0);

            data[key] = 0.3;
            subroutine.CheckCycle(data); // 1e 2s
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 0);
            startCycleCount.Should().Be(initialCount + 1);

            data[key] = 5.3;
            subroutine.CheckCycle(data);
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 0);
            startCycleCount.Should().Be(initialCount + 1);

            data[key] = 5.3;
            subroutine.CheckCycle(data); // 2e
            isWorking.Should().Be(false);
            endCycleCount.Should().Be(initialCount + 1);
            startCycleCount.Should().Be(initialCount + 1);


            data[key] = 5.3;
            subroutine.CheckCycle(data); // 2 idle
            isWorking.Should().Be(false);
            endCycleCount.Should().Be(initialCount + 1);
            startCycleCount.Should().Be(initialCount + 1);

            data[key] = 0;
            subroutine.CheckCycle(data); // 3s
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 1);
            startCycleCount.Should().Be(initialCount + 2);

            data[key] = 0.3;
            subroutine.CheckCycle(data);
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 1);
            startCycleCount.Should().Be(initialCount + 2);

            data[key] = 5.1;
            subroutine.CheckCycle(data);
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 1);
            startCycleCount.Should().Be(initialCount + 2);

            data[key] = 5.1;
            subroutine.CheckCycle(data); // 3e 
            isWorking.Should().Be(false);
            endCycleCount.Should().Be(initialCount + 2);
            startCycleCount.Should().Be(initialCount + 2);

            data[key] = 0.1;
            subroutine.CheckCycle(data); // 4s
            isWorking.Should().Be(true);
            endCycleCount.Should().Be(initialCount + 2);
            startCycleCount.Should().Be(initialCount + 3);
    
        }
    }
}
