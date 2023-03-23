using System;
using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// PLC 데이터를 수집한다
    /// </summary>
    public interface IPlcDataGatherer
    {
        /// <summary>
        /// 주소/값 사전
        /// </summary>
        IReadOnlyDictionary<string, short> RawData { get; }

        /// <summary>
        /// 라벨코드/값 사전
        /// </summary>
        IReadOnlyDictionary<string, object> ProcessedData { get; }

        /// <summary>
        /// PLC 데이터를 수집한다. 
        /// </summary>
        /// <returns>주소단위의 값 반환.</returns>
        Result Gather();

    }
}
