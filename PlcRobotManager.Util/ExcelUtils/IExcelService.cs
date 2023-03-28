using System.Collections.Generic;

namespace PlcRobotManager.Util.ExcelUtils
{
    /// <summary>
    /// 엑셀 서비스
    /// </summary>
    public interface IExcelService
    {
        /// <summary>
        /// 엑셀파일을 읽은 후 리스트를 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">엑셀 경로</param>
        /// <param name="options">옵션</param>
        /// <returns></returns>
        List<T> Read<T>(string filePath, ExcelOptions options = null);
    }
}
