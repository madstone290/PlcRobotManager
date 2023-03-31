using System;

namespace PlcRobotManager.Core
{
    public class CycleEventArgs : EventArgs
    {
        public CycleEventArgs(string name, string code, int count, DateTime changedTime)
        {
            Name = name;
            Code = code;
            Count = count;
            ChangedTime = changedTime;
        }

        /// <summary>
        /// 서브루틴이름
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 라벨 코드
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// 사이클 카운트
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 변경 시간
        /// </summary>
        public DateTime ChangedTime { get; }
    }
}
