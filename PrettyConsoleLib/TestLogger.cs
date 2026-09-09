using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PrettyConsoleLib
{
    public class TestLogger : ILogger
    {
        private readonly List<LogEntry> _entries = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter == null) return;
            var message = formatter(state, exception);
            lock (_entries)
            {
                _entries.Add(new LogEntry { Level = logLevel, Message = message, Exception = exception });
            }
        }

        public bool ContainsLevel(LogLevel level)
        {
            lock (_entries)
            {
                return _entries.Any(e => e.Level == level);
            }
        }

        public bool ContainsErrorMessage(string substring)
        {
            if (string.IsNullOrEmpty(substring)) return false;
            lock (_entries)
            {
                return _entries.Any(e => e.Level == LogLevel.Error
                                         && !string.IsNullOrEmpty(e.Message)
                                         && e.Message.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }

        private class LogEntry
        {
            public LogLevel Level { get; init; }
            public string? Message { get; init; }
            public Exception? Exception { get; init; }
        }
    }
}