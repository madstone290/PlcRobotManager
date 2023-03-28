using PlcRobotManager.Core.Vendor.Mitsubishi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcRobotManager.Core.Vendor.Mitsubishi.DeviceLabel;

namespace PlcRobotManager.Ui.Views.Auto
{
    public class PlcObjValue
    {
        private readonly DeviceLabel _label;
        private readonly object _value;

        public PlcObjValue(DeviceLabel label, object value)
        {
            _label = label;
            _value = value;
            Code = label.Code;
            DataType = label.DataType;
            Device = label.Device.Name;
            Address = label.Address;
            AddressString = label.AddressString;
            StartWordAddress = label.StartWordAddress;
            StartWordAddressString = label.StartWordAddressString;
            BitPosition = label.BitPosition;
            Length = label.Length;
            IsBitValue = label.Device.IsBit() || label.BitPosition != null;
            DecimalPoint = label.DecimalPoint;
            SubroutineName = label.Subroutine?.Name;
            SubroutineType = label.Subroutine == null ? string.Empty : label.Subroutine.DetectionType.ToString();
        }

        public DeviceLabel Label => _label;

        public object Value => _value;

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
        public string Device { get; }

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
        /// 수집 그룹. 매뉴얼 방식으로 데이터를 수집할 때 적용된다.
        /// </summary>
        public GatheringGroup Group { get; }

        /// <summary>
        /// 라벨의 값이 비트값인가?
        /// </summary>
        public bool IsBitValue { get; }

        /// <summary>
        /// 숫자 변환에 적용할 소수점 위치
        /// </summary>
        public int DecimalPoint { get; }
        
        public string SubroutineName { get; set; }

        public string SubroutineType { get; set; }

    }
}
