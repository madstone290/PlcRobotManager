using PlcRobotManager.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines
{
    /// <summary>
    /// 시작비트 및 종료비트를 이용해 사이클 상태를 판단한다.
    /// </summary>
    public class StartEndFlagSubroutine : ISubroutine
    {
        /// <summary>
        /// 시작비트 키
        /// </summary>
        private readonly string _startFlagKey;

        /// <summary>
        /// 종료비트 키
        /// </summary>
        private readonly string _endFlagKey;

        /// <summary>
        /// 서브루틴명
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 서브루틴 사이클 카운트
        /// </summary>
        private int _cycleCount = 0;

        /// <summary>
        /// 시작플래그 값. false -> true이면 사이클시작
        /// </summary>
        private bool _startFlag = false;

        /// <summary>
        /// 종료플래그 값. true -> false이면 사이클종료
        /// </summary>
        private bool _endFlag = false;

        /// <summary>
        /// 최초 사이클인지 확인한다. 프로그램을 처음 구동할 때에는 현재 사이클 상태를 정상적으로 판단할 수 없다.
        /// </summary>
        private bool _isFirstCylce = true;

        public StartEndFlagSubroutine(string startFlagKey, string endFlagKey, string name, int initialCount = 1)
        {
            _startFlagKey = startFlagKey;
            _endFlagKey = endFlagKey;
            _name = name;
            _cycleCount = initialCount;
        }

        public string Name => _name;
        public SubroutineDetectionType DetectionType => SubroutineDetectionType.StartEndFlag;
        public int CycleCount => _cycleCount;
        public event EventHandler<int> CycleStarted;
        public event EventHandler<int> CycleEnded;

        public void CheckCycle(IReadOnlyDictionary<string, object> data)
        {
            if (!data.TryGetValue(_startFlagKey, out object startValue))
                return;
            if (!data.TryGetValue(_endFlagKey, out object endValue))
                return;

            bool startFlag = SafeConvert.ToBoolean(startValue);
            bool endFlag = SafeConvert.ToBoolean(endValue);
            if (_isFirstCylce) // 첫 1회에는 값만 기록하고 사이클을 판단하지 않는다.
            {
                _isFirstCylce = false;
                _startFlag = startFlag;
                _endFlag = endFlag;
                return;
            }

            if (!_startFlag && startFlag) // 시작
            {
                CycleStarted?.Invoke(this, _cycleCount);
            }
            else if (!_endFlag && endFlag) // 종료
            {
                CycleEnded?.Invoke(this, _cycleCount);
                _cycleCount++;
            }
            _startFlag = startFlag;
            _endFlag = endFlag;
        }
    }
}
