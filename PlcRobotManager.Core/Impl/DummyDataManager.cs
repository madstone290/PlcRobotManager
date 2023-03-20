using System;
using System.Collections.Generic;
using System.Linq;

namespace PlcRobotManager.Core.Impl
{
    public class DummyDataManager : IDataManager
    {
        public void Save(object data)
        {
            if (data == null)
                return;

            if (data is IEnumerable<KeyValuePair<string, short>> pairs)
            {
                Console.WriteLine($"Saved data with number of: {pairs.Count()}");
            }
            else
            {
                Console.WriteLine($"Unsupported data type: {data.GetType().Name}");
            }

            
        }
    }
}
