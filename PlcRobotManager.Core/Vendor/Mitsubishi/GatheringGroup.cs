namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 수집 그룹. 수동 수집기에서는 그룹단위로 데이터 수집이 이루어진다.
    /// </summary>
    public class GatheringGroup
    {
        public GatheringGroup(string name, RangeType rangeType)
        {
            Name = name;
            RangeType = rangeType;
        }

        /// <summary>
        /// 그룹명
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 데이터 수집에 적용할 범위 타입
        /// </summary>
        public RangeType RangeType { get; }
    }
}
