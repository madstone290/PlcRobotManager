namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 서브루틴의 사이클변경 탐지 방법
    /// </summary>
    public enum SubroutineDetectionType
    {
        /// <summary>
        /// 사이클타임 값으로 사이클 시작/종료를 개별로 판단한다.
        /// </summary>
        CycleTime = 0,

        /// <summary>
        /// 수량 값으로 사이클 시작/종료를 함께 판단한다.
        /// </summary>
        Quantity = 1,

        /// <summary>
        /// 시작 비트값으로 사이클 시작/종료를 개별로 판단한다.
        /// </summary>
        StartFlag = 2,

        /// <summary>
        /// 시작 비트값으로 사이클 시작/종료를 개별로 판단한다. 비트가 반대로 적용된다.
        /// </summary>
        StartFlagReverse = 3,

        /// <summary>
        /// 시자, 종료 비트값으로 사이클 시작/종료를 개별로 판단한다.
        /// </summary>
        StartEndFlag = 4,

        /// <summary>
        /// 시자, 종료 비트값으로 사이클 시작/종료를 개별로 판단한다. 비트가 반대로 적용된다.
        /// </summary>
        StartEndFlagReverse = 5
    }
}
