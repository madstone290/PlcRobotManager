using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Ui.Views.Auto
{
    public class PlcSubroutineValue
    {
        /// <summary>
        /// 루틴명
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 사이클타임
        /// </summary>
        public TimeSpan Current { get; set; }

        public TimeSpan Last { get; set; }
    }
}
