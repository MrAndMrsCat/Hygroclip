global using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Filters;
using Serilog;
using Shirehorse.Core.Extensions;
using static Shirehorse.Core.FiniteStateMachines.IStateMachine;

namespace PicoController;
public static class Logger
{
    public record LogMessage
    {
        public LogEventLevel Level { get; set; }
        public string Message { get; set; } = "";
    }

    public static event EventHandler<LogMessage>? NewMessage;

    public static LogEventLevel LogLevel { get; set; } = LogEventLevel.Debug;

    public static void Write(LogEventLevel level, string message)
    {
        if (!_serilogInitialized) InitializeSeriLog();

        try
        {
            Log.Write(level, message);

            NewMessage?.Invoke(null, new LogMessage()
            {
                Level = level,
                Message = message
            });
        }
        catch
        {
            // shouldn't reach here
        }
    }

    public static void Debug(string message) => Write(LogEventLevel.Debug, message);
    public static void Info(string message) => Write(LogEventLevel.Information, message);
    public static void Warning(string message) => Write(LogEventLevel.Warning, message);
    public static void Error(string message) => Write(LogEventLevel.Error, message);
    public static void Debug(Exception exception) => Write(LogEventLevel.Debug, exception.ToHierarchicalString());

    public static void InitializeSeriLog()
    {
        try
        {
            Log.CloseAndFlush();

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.ControlledBy(new(LogEventLevel.Debug))
               .WriteTo.Console(restrictedToMinimumLevel: LogLevel, theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Literate)
               .CreateLogger();

            _serilogInitialized = true;
        }
        catch
        {
            // do nothing, this really shouldn't happen, but we don't want to halt the application if it does
        }

    }

    private static bool _serilogInitialized;
}
