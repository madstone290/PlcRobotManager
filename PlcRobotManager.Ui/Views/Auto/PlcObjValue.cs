using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Ui.Views.Auto
{
    public class PlcObjValue
    {
        private readonly string _address;
        private readonly object _value;

        public PlcObjValue(string address, object value)
        {
            _address = address;
            _value = value;
        }

        public string Address => _address;
        public object Value => _value;
    }
}
