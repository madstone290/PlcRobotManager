using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 디바이스 라벨(주소). 디바이스 및 주소정보를 가진다.
    /// </summary>
    public class DeviceLabel
    {
        public DeviceLabel(string code, Device device, int address, DataType dataType = DataType.Number, 
            int length = 1, int? bitPosition = null, bool raiseValueEvent = false, GatheringGroup group = null, SubroutineInfo subroutine = null)
        {
            Code = code;
            Device = device;
            Address = address;
            DataType = dataType;
            Length = length < 1 ? 1 : length;
            RaiseValueEvent = raiseValueEvent;

            if (device.IsBit())
            {
                StartWordAddress = address / 16;
                BitPosition = address % 16;
            }
            else
            {
                StartWordAddress = address;
                BitPosition = bitPosition;
            }

            StartWordAddressString = device.GetAddressString(StartWordAddress);
            AddressString = device.GetAddressString(Address, BitPosition);

            Group = group;
            Subroutine = subroutine;
        }

        /// <summary>
        /// 라벨 식별을 위한 코드. 중복이 허용되지 않는다.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 라벨 데이터 타입
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// 디바이스
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// 실제 시작 주소.
        /// 비트주소인 경우 워드주소로 수신한 값을 비트단위로 읽는다.
        /// </summary>
        public int Address { get; }

        /// <summary>
        /// 실제 주소 문자열 표현
        /// </summary>
        public string AddressString { get; }

        /// <summary>
        /// 라벨에 포함된 모든 주소의 문자열
        /// </summary>
        public List<string> AddressStringList => Enumerable.Range(0, Length)
            .Select(offset => Device.GetAddressString(Address + offset, BitPosition))
            .ToList();

        /// <summary>
        /// 값 조회를 위한 시작 워드주소. 블록읽기에 사용한다.
        /// </summary>
        public int StartWordAddress { get; }

        /// <summary>
        /// 끝 워드 주소
        /// </summary>
        public int EndWordAddress => StartWordAddress + Length - 1;

        /// <summary>
        /// 워드주소의 문자열 표현
        /// </summary>
        public string StartWordAddressString { get; }

        /// <summary>
        /// 라벨의 비트위치. 워드값을 비트로 사용할 때 적용한다.
        /// </summary>
        public int? BitPosition { get; }

        /// <summary>
        /// 워드 길이
        /// </summary>
        public int Length { get; } = 1;

        /// <summary>
        /// 라벨의 값이 비트값인가?
        /// </summary>
        public bool IsBitValue => Device.IsBit() || BitPosition.HasValue;

        /// <summary>
        /// 숫자 변환에 적용할 소수점 위치
        /// </summary>
        public int DecimalPoint { get; }

        /// <summary>
        /// 값 변경시 이벤트 발생여부
        /// </summary>
        public bool RaiseValueEvent { get; set; }

        /// <summary>
        /// 수집 그룹. 매뉴얼 방식으로 데이터를 수집할 때 적용된다.
        /// </summary>
        public GatheringGroup Group { get; }

        /// <summary>
        /// 서브루틴정보
        /// </summary>
        public SubroutineInfo Subroutine { get; }

        /// <summary>
        /// 라벨의 데이터 타입에 맞게 값을 변환한다.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public object ConvertValue(IEnumerable<short> values)
        {
            switch (DataType)
            {
                case DataType.Bit:
                    return ConvertToBit(values);
                case DataType.String:
                    return ConvertToString(values);
                case DataType.Number:
                default:
                    return ConvertToNumber(values);
            }
        }

        private bool ConvertToBit(IEnumerable<short> values)
        {
            return values.First() == 1;
        }

        private double ConvertToNumber(IEnumerable<short> values)
        {
            double number;
            if (Length == 1)
                number = values.First();
            else
                number = values.First() * short.MaxValue + values.Skip(1).First();

            double decimalPontParam = Math.Pow(10, DecimalPoint);
            int intPart = (int)(number / decimalPontParam);
            int decimalPart = (int)(number % decimalPontParam);
            number = intPart + (decimalPart / decimalPontParam);
            return number;
        }

        private string ConvertToString(IEnumerable<short> values)
        {
            string text = string.Empty;
            foreach (short value in values)
            {
                text += Encoding.ASCII.GetString(BitConverter.GetBytes(value));
            }
            return text;
        }

        public override string ToString()
        {
            return $"{nameof(DeviceLabel)} {AddressString}";
        }

        public class SubroutineInfo
        {
            public SubroutineInfo(string name, SubroutineDetectionType detectionType, bool isStart = false, bool isEnd = false)
            {
                Name = name;
                DetectionType = detectionType;
                IsStart = isStart;
                IsEnd = isEnd;
            }

            /// <summary>
            /// 루틴명
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 루틴 사이클탐지 유형
            /// </summary>
            public SubroutineDetectionType DetectionType { get; }

            /// <summary>
            /// 루틴의 시작을 알리는 값인가?
            /// </summary>
            public bool IsStart { get; }

            /// <summary>
            /// 루틴의 종료를 알리는 값인가?
            /// </summary>
            public bool IsEnd { get; }
        }

        /// <summary>
        /// 라벨 비교. 디바이스, 주소, 비트여부, 코드순으로 정렬한다.
        /// </summary>
        public class Comparer : IComparer<DeviceLabel>
        {
            public static Comparer Default { get; } = new Comparer();

            public int Compare(DeviceLabel x, DeviceLabel y)
            {
                if (x.Device != y.Device)
                    return x.Device.Name.CompareTo(y.Device.Name);

                if (x.Address != y.Address)
                    return x.Address.CompareTo(y.Address);

                if (x.BitPosition.HasValue && y.BitPosition.HasValue)
                    return x.BitPosition.Value.CompareTo(y.BitPosition.Value);
                else if (x.BitPosition.HasValue)
                    return 1;
                else if (y.BitPosition.HasValue)
                    return -1;
                else
                    return x.Code.CompareTo(y.Code);
            }
        }

    }
}
