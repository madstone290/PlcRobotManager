using System;

namespace PlcRobotManager.Core
{
    /// <summary>
    /// 사이클 이벤트 인터페이스.
    /// </summary>
    public interface ICycleEvent
    {
        /// <summary>
        /// 서브루틴 사이클이 시작되었다.
        /// </summary>
        event EventHandler<CycleEventArgs> CycleStarted;

        /// <summary>
        /// 서브루틴 사이클이 완료되었다
        /// </summary>
        event EventHandler<CycleEventArgs> CycleEnded;
    }
}
