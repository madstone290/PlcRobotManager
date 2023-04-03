using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public class RobotValueEventArgs
    {
        public RobotValueEventArgs(string name, ValueChangeEventArgs valueChangeEventArgs)
        {
            Name = name;
            ValueChangeEventArgs = valueChangeEventArgs;
        }


        /// <summary>
        /// 로봇이름
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 값 변경 이벤트
        /// </summary>
        public ValueChangeEventArgs ValueChangeEventArgs { get; }
    }
}
