using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core
{
    public interface IValueEvent
    {
        /// <summary>
        /// 값이 변경되었다.
        /// </summary>
        event EventHandler<ValueChangeEventArgs> ValueChanged;
    }
}
