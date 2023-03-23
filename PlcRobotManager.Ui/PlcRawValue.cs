using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Ui
{
    public class PlcRawValue
    {
        private readonly BitArray _bitArray;
        private readonly string _address;
        private readonly short _value;

        public PlcRawValue(string address, short value)
        {
            _address = address;
            _value = value;
            _bitArray = new BitArray(BitConverter.GetBytes(value));
        }

        public string Address => _address;
        public short Value => _value;
        public bool Bit15 => _bitArray[15];
        public bool Bit14 => _bitArray[14];
        public bool Bit13 => _bitArray[13];
        public bool Bit12 => _bitArray[12];
        public bool Bit11 => _bitArray[11];
        public bool Bit10 => _bitArray[10];
        public bool Bit9 => _bitArray[9];
        public bool Bit8 => _bitArray[8];
        public bool Bit7 => _bitArray[7];
        public bool Bit6 => _bitArray[6];
        public bool Bit5 => _bitArray[5];
        public bool Bit4 => _bitArray[4];
        public bool Bit3 => _bitArray[3];
        public bool Bit2 => _bitArray[2];
        public bool Bit1 => _bitArray[1];
        public bool Bit0 => _bitArray[0];
    }
}
