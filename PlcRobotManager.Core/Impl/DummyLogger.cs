using System;

namespace PlcRobotManager.Core.Impl
{
    public class DummyLogger : ILogger
    {
        public void Debug(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }

        public void Error(string message, Exception exception = null)
        {
            Console.WriteLine(message);
        }

        public void Fatal(string message, Exception exception = null)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }
}
