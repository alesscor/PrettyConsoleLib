using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PrettyConsoleLib.Specs.Acceptance.Helpers
{
    // Minimal in-memory logger for assertions
    public class TestLogger : ILogger
    {
        private readonly List<LogEntry> _entries = new List<LogEntry>();

        IDisposable ILogger.BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _entries.Add(new LogEntry
            {
                Level = logLevel,
                Message = formatter(state, exception),
                Exception = exception
            });
        }

        public bool ContainsLevel(LogLevel level) => _entries.Exists(e => e.Level == level);

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }

        private record LogEntry
        {
            public LogLevel Level { get; init; }
            public string? Message { get; init; }
            public Exception? Exception { get; init; }
        }
        // add this helper to the existing TestLogger class so tests can assert on messages
        public bool ContainsErrorMessage(string substring) {
            if (string.IsNullOrEmpty(substring)) return false;
            lock (_entries) {
                return _entries.Any(e =>
                    e.Level == LogLevel.Error &&
                    !string.IsNullOrEmpty(e.Message) &&
                    e.Message.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
    }
}