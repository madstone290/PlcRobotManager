using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public class RobotCycleEventArgs
    {
        public RobotCycleEventArgs(string name, CycleEventArgs cycleEventArgs)
        {
            Name = name;
            CycleEventArgs = cycleEventArgs;
        }


        /// <summary>
        /// 로봇이름
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 사이클이벤트
        /// </summary>
        public CycleEventArgs CycleEventArgs { get; }
    }
}
