using PlcRobotManager.Core.Impl;
using System;

namespace PlcRobotManager.Core
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception exception = null);
        void Fatal(string message, Exception exception = null);
    }

    public static class LoggerFactory
    {
        public static ILogger GetLogger<TContext>()
        {
            return SerilogWrapLogger.ForContext<TContext>();
        }

        public static ILogger GetLogger()
        {
            return SerilogWrapLogger.Default();
        }
    }

}
