using PlcRobotManager.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines
{
    /// <summary>
    /// 수량값을 추적하여 사이클 시작/종료를 판단한다.
    /// 사이클 정지상태를 파악할 수 없다.
    /// </summary>
    public class QuantitySubroutine : ISubroutine
    {
        /// <summary>
        /// 사이클 상태 확인을 위한 키
        /// </summary>
        private readonly string _quantityKey;

        /// <summary>
        /// 서브루틴명
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 수량
        /// </summary>
        private int _quantity;

        /// <summary>
        /// 서브루틴 사이클 카운트
        /// </summary>
        private int _cycleCount = 0;

        /// <summary>
        /// 최초 사이클인지 확인한다. 프로그램을 처음 구동할 때에는 현재 사이클 상태를 정상적으로 판단할 수 없다.
        /// </summary>
        private bool _isFirstCylce = true;

        public string Name => _name;
        public SubroutineDetectionType DetectionType => SubroutineDetectionType.Quantity;
        public int CycleCount => _cycleCount;
        public event EventHandler<int> CycleStarted;
        public event EventHandler<int> CycleEnded;

        public void CheckCycle(IReadOnlyDictionary<string, object> data)
        {
            if (!data.TryGetValue(_quantityKey, out object value))
                return;
            if (!int.TryParse(Convert.ToString(value), out int quantity))
                return;

            if (_isFirstCylce) // 첫 1회에는 값만 기록하고 사이클을 판단하지 않는다.
            {
                _isFirstCylce = false;
                _quantity= quantity;
                return;
            }

            if(_quantity < quantity) // 시작 && 종료
            {
                CycleEnded?.Invoke(this, _cycleCount);
                _cycleCount++;
                CycleStarted?.Invoke(this, _cycleCount);
            }
            _quantity = quantity;
        }
    }
}
