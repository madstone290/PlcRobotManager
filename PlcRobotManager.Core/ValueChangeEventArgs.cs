using System;

namespace PlcRobotManager.Core
{
    public class ValueChangeEventArgs : EventArgs
    {
        public ValueChangeEventArgs(string code, object prev, object current, DateTime changedTime)
        {
            Code = code;
            Prev = prev;
            Current = current;
            ChangedTime = changedTime;
        }

        /// <summary>
        /// 라벨 코드
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 이전 값. null일수도 있다.
        /// </summary>
        public object Prev { get; }

        /// <summary>
        /// 변경된 값
        /// </summary>
        public object Current { get; }

        /// <summary>
        /// 변경 시간
        /// </summary>
        public DateTime ChangedTime { get; }
    }
}
