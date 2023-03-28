using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Extensions
{
    /// <summary>
    /// 숫자 범위를 생성한다.
    /// </summary>
    public static class StepRange
    {
        /// <summary>
        /// 정수 범위 생성
        /// </summary>
        /// <param name="start">시작</param>
        /// <param name="count">개수</param>
        /// <param name="step">간격</param>
        /// <returns></returns>
        public static IEnumerable<int> For(int start, int count, int step = 1)
        {
            int value = start - step;
            for (int i = 0; i < count; i++)
            {
                value += step;
                yield return value;
            }
        }
    }
}
