using System;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines
{
    /// <summary>
    /// 서브루틴을 생성한다
    /// </summary>
    public class SubroutineFactory
    {
        public ISubroutine Create(SubroutineDetectionType detectionType, string name, string dictKey, int initCount = 1, bool isStart = false, bool isEnd = false)
        {
            switch(detectionType)
            {
                case SubroutineDetectionType.CycleTime:
                    return new CycleTimeSubroutine(name, dictKey, initCount);
                default:
                    throw new ArgumentOutOfRangeException(nameof(detectionType));
            }
        }
    }
}
