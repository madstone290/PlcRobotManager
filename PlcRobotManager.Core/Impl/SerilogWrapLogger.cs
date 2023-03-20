using Serilog;
using System;

namespace PlcRobotManager.Core.Impl
{
    public class SerilogWrapLogger : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogWrapLogger(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        static SerilogWrapLogger()
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
            Log.Information("No one listens to me!");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] [{ClassName}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/log-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ClassName}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }

        public static SerilogWrapLogger ForContext<TContext>()
        {
            return new   SerilogWrapLogger(Log.ForContext("ClassName", typeof(TContext).Name));
        }

        public static SerilogWrapLogger Default()
        {
            return new SerilogWrapLogger(Log.Logger);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message, Exception exception = null)
        {
            _logger.Error(exception, message);
        }

        public void Fatal(string message, Exception exception = null)
        {
            _logger.Fatal(exception, message);
        }

        public void Info(string message)
        {
            _logger.Information(message);
        }

        public void Warn(string message)
        {
            _logger.Warning(message);
        }
    }
}
