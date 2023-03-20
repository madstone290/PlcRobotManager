using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// PLC 데이터를 수집한다
    /// </summary>
    public interface IPlcDataGatherer
    {
        /// <summary>
        /// PLC 데이터를 수집한다.
        /// </summary>
        /// <returns></returns>
        Result<Dictionary<string, short>> Gather();
    }
}
