using PlcRobotManager.Core.Vendor.Mitsubishi;

namespace PlcRobotManager.Core.Infos
{
    public class DeviceLabelInfo
    {
        public string Code { get; set; }
        public string DeviceName { get; set; }
        public int Address { get; set; }
        public DataType DataType { get; set; }
        public int Length { get; set; } = 1;
        public int? BitPosition { get; set; }
        public bool RaiseValueEvent { get; set; }

        #region GatheringGroup
        /// <summary>
        /// 그룹명
        /// </summary>
        public string GatheringGroupName { get; set; }

        /// <summary>
        /// 데이터 수집에 적용할 범위 타입
        /// </summary>
        public RangeType GatheringGroupRangeType { get; set; }
        #endregion

        #region Subroutine

        /// <summary>
        /// 루틴명
        /// </summary>
        public string SubroutineName { get; set; }

        /// <summary>
        /// 루틴 사이클탐지 유형
        /// </summary>
        public SubroutineDetectionType SubroutineDetectionType { get; set; }

        /// <summary>
        /// 루틴의 시작을 알리는 값인가?
        /// </summary>
        public bool SubroutineIsStart { get; set; }

        /// <summary>
        /// 루틴의 종료를 알리는 값인가?
        /// </summary>
        public bool SubroutineIsEnd { get; set; }

        #endregion

    }
}
