using System;
using System.Collections.Generic;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines
{
    /// <summary>
    /// PLC사이클타임값을 기반으로 서브루틴의 사이클을 감지한다.
    /// 조회시간(수백ms)보다 작은 사이클타임에 대해서는 정상적으로 동작하지 않는다.
    /// 시작/종료/대기 3가지 상태를 가진다.
    /// <para>
    /// [시작]
    /// 1) 프로그램 가동 후 최초로 사이클타임이 증가하는 경우
    /// 2) 사이클타임이 이전 사이클타임보다 줄어드는 경우
    /// </para>
    /// <para>
    /// [종료]
    /// 1) 작업상태가 On이고 사이클타임이 이전 사이클타임과 동일한 경우
    /// 2) 사이클타임이 이전 사이클타임보다 줄어들었는데 작업상태가 on인 경우 (작업종료를 탐지하지 못한채로 다음 사이클 시작)
    /// </para>
    /// <para>
    /// [대기]
    /// 1) 작업상태가 Off이고 사이클타임이 이전 사이클타임과 동일한 경우
    /// </para>
    /// </summary>
    public class CycleTimeSubroutine : ISubroutine
    {
        /// <summary>
        /// 서브루틴명
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 서브루틴 사이클 카운트
        /// </summary>
        private int _cycleCount = 0;

        /// <summary>
        /// 현재 사이클타임
        /// </summary>
        private double _cycleTime = 0;

        /// <summary>
        /// PLC 사이클타임값 확인을 위한 키
        /// </summary>
        private readonly string _cycleTimeCode;

        /// <summary>
        /// 루틴 작업진행 상태
        /// </summary>
        private bool _isRoutineWorking;

        private bool _isFirstCylce = true;

        public CycleTimeSubroutine(string name, string cycleTimeCode, int initialCount = 1)
        {
            _name = name;
            _cycleTimeCode = cycleTimeCode;
            _cycleCount = initialCount;
        }

        #region ISubroutine
        public string Name => _name;
        public SubroutineDetectionType DetectionType => SubroutineDetectionType.CycleTime;
        public int CycleCount => _cycleCount;
        public List<string> Codes => new List<string>() { _cycleTimeCode };

        public event EventHandler<int> CycleStarted;
        public event EventHandler<int> CycleEnded;

        public void CheckCycle(IReadOnlyDictionary<string, object> data)
        {
            if (!data.TryGetValue(_cycleTimeCode, out object value))
                return;

            if (!double.TryParse(Convert.ToString(value), out double cycleTime))
                return;

            if (_isFirstCylce) // 첫 1회에는 값만 기록하고 사이클을 판단하지 않는다.
            {
                _isFirstCylce = false; 
                _cycleTime = cycleTime;
                return;
            }


            if (_cycleTime < cycleTime) // 사이클 진행
            {
                // 시작
                if(_cycleTime == 0 && !_isRoutineWorking) // 시작
                {
                    _isRoutineWorking = true;
                    CycleStarted?.Invoke(this, _cycleCount);
                }
            }
            else if (cycleTime < _cycleTime) // (종료 and 시작) or 시작
            {
                // 종료
                if (_isRoutineWorking) 
                {
                    _isRoutineWorking = false;
                    CycleEnded?.Invoke(this, _cycleCount);
                    _cycleCount++;
                }
                // 시작
                _isRoutineWorking = true;
                CycleStarted?.Invoke(this, _cycleCount);
            }
            else if (cycleTime == _cycleTime) // 종료 or 대기
            {
                if (_isRoutineWorking) // 종료
                {
                    _isRoutineWorking = false;
                    CycleEnded?.Invoke(this, _cycleCount);
                    _cycleCount++;
                }
                // 대기
            }
            
            _cycleTime = cycleTime;


        }
        #endregion
    }
}
