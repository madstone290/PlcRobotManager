using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Ui.Views.Auto
{
    public class ValueChangeLog
    {
        /// <summary>
        /// 라벨코드
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 변경시간
        /// </summary>
        public DateTime Changed { get; set; }

        public string ChangedStr => Changed.ToString("HH:mm:ss");

        /// <summary>
        /// 이전 값
        /// </summary>
        public object Prev { get; set; }

        /// <summary>
        /// 변경된 값
        /// </summary>
        public object Current { get; set; }

    }
}
