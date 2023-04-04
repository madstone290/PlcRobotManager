using System;
using System.Collections.Generic;

namespace PlcRobotManager.Core
{
    public class CycleEventArgs : EventArgs
    {
        public CycleEventArgs(string name, List<string> codes, int count, DateTime changedTime)
        {
            Name = name;
            Codes = codes;
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
        /// 라벨코드(사용된 코드가 여러개인 경우)
        /// </summary>
        public List<string> Codes { get; }
        
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
