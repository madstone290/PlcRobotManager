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

        public DateTime? Started { get; set; }

        public string StartedStr => Started?.ToString("HH:mm:ss");

        public DateTime? Ended { get; set; }

        public string EndedStr => Ended?.ToString("HH:mm:ss");

        /// <summary>
        /// 사이클타임
        /// </summary>
        public TimeSpan Current { get; set; }

        public string CurrentStr => $@"{Current:mm\:ss\.fff}";

        public TimeSpan Last { get; set; }

        public string LastStr => $@"{Last:mm\:ss\.fff}";

        public void UpdateTime()
        {
            if (!Started.HasValue)
                return;
            if (Ended.HasValue && Started.Value < Ended.Value)
                return;

            Current = DateTime.Now - Started.Value;

        
        }
    }
}
