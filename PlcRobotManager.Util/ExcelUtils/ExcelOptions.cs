using System.Collections.Generic;

namespace PlcRobotManager.Util.ExcelUtils
{
    /// <summary>
    /// 엑셀 서비스에 사용할 옵션
    /// </summary>
    public class ExcelOptions
    {
        public ExcelOptions(IEnumerable<Header> headers)
        {
            Headers = headers;
        }

        public IEnumerable<Header> Headers { get; set; }

        /// <summary>
        /// 컬럼 헤더. 캡션을 제공한다.
        /// </summary>
        public class Header
        {
            public Header()
            {
            }

            public Header(string name, string caption)
            {
                Name = name;
                Caption = caption;
            }


            /// <summary>
            /// 컬럼 헤더명
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 컬럼 헤더캡션
            /// </summary>
            public string Caption { get; set; }
        }
    }
}
