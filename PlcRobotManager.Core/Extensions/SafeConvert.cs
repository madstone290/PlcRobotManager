using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Extensions
{
    public class SafeConvert
    {
        public static bool ToBoolean(object value)
        {
            bool result = false;
            try
            {
                result = Convert.ToBoolean(value);
            }
            catch { }
            return result;
        }
    }
}
