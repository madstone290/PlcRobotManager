using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace PlcRobotManager.Core.Vendor.Mitsubishi.Subroutines
{
    /// <summary>
    /// 서브루틴을 생성한다
    /// </summary>
    public class SubroutineFactory
    {
        public ISubroutine Create(SubroutineDetectionType detectionType, string name, string[] dictKeys, int initCount = 1)
        {
            switch (detectionType)
            {
                case SubroutineDetectionType.CycleTime:
                    return new CycleTimeSubroutine(name, dictKeys[0], initCount);
                case SubroutineDetectionType.Quantity:
                    return new QuantitySubroutine(name, dictKeys[0], initCount);
                case SubroutineDetectionType.StartFlag:
                    return new StartFlagSubroutine(name, dictKeys[0], initCount);
                case SubroutineDetectionType.StartEndFlag:
                    return new StartEndFlagSubroutine(name, dictKeys[0], dictKeys[1], initCount);
                default:
                    throw new ArgumentOutOfRangeException(nameof(detectionType));
            }
        }

        public ISubroutine Create(SubroutineDetectionType detectionType, string name, string dictKey, int initCount = 1)
        {
            return Create(detectionType, name, new string[] { dictKey }, initCount);
        }

        public List<ISubroutine> Create(IEnumerable<DeviceLabel> labels)
        {
            List<ISubroutine> subroutines = new List<ISubroutine>();
            var groups = labels.GroupBy(x => new { x.Subroutine.Name, x.Subroutine.DetectionType });
            foreach(var  group in groups)
            {
                SubroutineDetectionType detectionType = group.Key.DetectionType;
                string subroutineName = group.Key.Name;

                ISubroutine subroutine;
                if (group.Count() == 1)
                {
                    DeviceLabel label = group.First();
                    subroutine = Create(detectionType, subroutineName, label.Code);
                }
                else
                {
                    subroutine = Create(detectionType, subroutineName, 
                        new string[] { group.First(x=> x.Subroutine.IsStart).Code, group.First(x => x.Subroutine.IsEnd).Code });
                }
                subroutines.Add(subroutine);
            }
            return subroutines;
        }

    }
}
