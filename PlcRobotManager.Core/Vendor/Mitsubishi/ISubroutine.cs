using System;
using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// Plc 프로그램의 서브루틴. 사이클을 가진다.
    /// </summary>
    public interface ISubroutine
    {
        /// <summary>
        /// 서브루틴 이름
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 서브루틴 사이클판단에 사용할 라벨코드
        /// </summary>
        List<string> Codes { get; }

        /// <summary>
        /// 사이클 탐지 유형
        /// </summary>
        SubroutineDetectionType DetectionType { get; }

        /// <summary>
        /// 현재 사이클 카운트
        /// </summary>
        int CycleCount { get; }

        /// <summary>
        /// 사이클이 변경되었는지 확인한다.
        /// </summary>
        /// <param name="data"></param>
        void CheckCycle(IReadOnlyDictionary<string, object> data);

        /// <summary>
        /// 사이클이 시작되었다.
        /// </summary>
        event EventHandler<int> CycleStarted;

        /// <summary>
        /// 사이클이 종료되었다.
        /// </summary>
        event EventHandler<int> CycleEnded;


    }
}
