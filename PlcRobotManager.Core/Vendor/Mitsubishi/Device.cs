using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 미쯔비시 PLC 디바이스.
    /// 주로 사용하는 디바이스만 제공한다.
    /// </summary>
    public class Device
    {
        public Device(string name, DeviceType type, NumberType numberType, string description)
        {
            Name = name;
            Type = type;
            NumberType = numberType;
            Description = description;
        }

        public string Name { get; }
        public DeviceType Type { get; }
        public NumberType NumberType { get; }
        public string Description { get; }

        public bool IsBit() => Type == DeviceType.Bit;

        public static Device FromName(string name)
        {
            return _devices[name.ToUpper()];
        }

        private static readonly Dictionary<string, Device> _devices = new Dictionary<string, Device>()
        {
            { "FX",  new Device("FX", DeviceType.Bit, NumberType.Decimal, "Function input") },
            { "FY",  new Device("FY", DeviceType.Bit, NumberType.Decimal, "Function output") },
            { "FD",  new Device("FD", DeviceType.Word, NumberType.Decimal, "Function register") },
            { "SM",  new Device("SM", DeviceType.Bit, NumberType.Decimal, "Special relay") },
            { "SD",  new Device("SD", DeviceType.Word, NumberType.Decimal, "Special register") },
            { "X",   new Device("X", DeviceType.Bit, NumberType.Hexadecimal, "Input relay")  },
            { "Y",   new Device("Y", DeviceType.Bit, NumberType.Hexadecimal, "Output relay")  },
            { "M",   new Device("M", DeviceType.Bit, NumberType.Decimal, "Internal relay")  },
            { "L",   new Device("L", DeviceType.Bit, NumberType.Decimal, "Latch relay")  },
            { "F",   new Device("F", DeviceType.Bit, NumberType.Decimal, "Annunciator") },
            { "V",   new Device("V", DeviceType.Bit, NumberType.Decimal, "Edge relay")  },
            { "B",   new Device("B", DeviceType.Bit, NumberType.Hexadecimal, "Link relay")  },
            { "D",   new Device("D", DeviceType.Word, NumberType.Decimal, "Data register")  },
            { "W",   new Device("W", DeviceType.Word, NumberType.Hexadecimal, "Link register") }
        };

        public static Device FX => _devices["FX"];
        public static Device FY => _devices["FY"];
        public static Device FD => _devices["FD"];
        public static Device SM => _devices["SM"];
        public static Device SD => _devices["SD"];
        public static Device X => _devices["X"];
        public static Device Y => _devices["Y"];
        public static Device M => _devices["M"];
        public static Device L => _devices["L"];
        public static Device F => _devices["F"];
        public static Device V => _devices["V"];
        public static Device B => _devices["B"];
        public static Device D => _devices["D"];
        public static Device W => _devices["W"];
    }
}
