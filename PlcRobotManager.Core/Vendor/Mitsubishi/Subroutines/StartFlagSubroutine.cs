using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines
{
    /// <summary>
    /// 작업상태시작을 알리는 비트값으로 사이클 상태를 판단한다.
    /// 비트가 1로 변하는 경우 사이클시작, 0으로 변하는 경우 사이클종료로 판단한다.
    /// 사이클종료에서 시작까지의 간격이 좁은 경우(수백ms) 통신지연으로(수백ms) 인해 정상적인 사이클탐지에 실패할 가능성이 높다.
    /// </summary>
    public class StartFlagSubroutine : ISubroutine
    {
        /// <summary>
        /// 사이클 상태 확인을 위한 키
        /// </summary>
        private readonly string _flagKey;

        /// <summary>
        /// 서브루틴명
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 서브루틴 사이클 카운트
        /// </summary>
        private int _cycleCount = 0;

        /// <summary>
        /// 시작 플래그 값
        /// </summary>
        private bool _startFlag = false;

        /// <summary>
        /// 최초 사이클인지 확인한다. 프로그램을 처음 구동할 때에는 현재 사이클 상태를 정상적으로 판단할 수 없다.
        /// </summary>
        private bool _isFirstCylce = true;

        public StartFlagSubroutine(string name, string flagKey, int initialCount = 1)
        {

            _name = name;
            _flagKey = flagKey;
            _cycleCount = initialCount;
        }

        public string Name => _name;
        public SubroutineDetectionType DetectionType => SubroutineDetectionType.StartFlag;
        public int CycleCount => _cycleCount;
        public event EventHandler<int> CycleStarted;
        public event EventHandler<int> CycleEnded;

        public void CheckCycle(IReadOnlyDictionary<string, object> data)
        {
            if (!data.TryGetValue(_flagKey, out object value))
                return;

            bool startFlag = Convert.ToBoolean(value);

            if (_isFirstCylce) // 첫 1회에는 값만 기록하고 사이클을 판단하지 않는다.
            {
                _isFirstCylce = false;
                _startFlag = startFlag;
                return;
            }

            if (_startFlag == startFlag)
                return;

            if (startFlag)
            {
                CycleStarted?.Invoke(this, _cycleCount);
            }
            else
            {
                CycleEnded?.Invoke(this, _cycleCount);
                _cycleCount++;
            }

            _startFlag = startFlag;
        }
    }
}
