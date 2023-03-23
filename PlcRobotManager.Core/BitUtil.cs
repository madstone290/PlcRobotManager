using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public class BitUtil
    {
        /// <summary>
        /// 정수의 비트값을 확인한다
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsOn(int value, int position)
        {
            return ((value >> position) & 1 )== 1;
        }
    }
}
