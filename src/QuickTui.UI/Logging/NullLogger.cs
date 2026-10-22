using System;

namespace QuickTui.UI.Logging;

/// <summary>
/// A dummy logger.
/// </summary>
public class NullLogger : ILogger {
    /// <inheritdoc />
    public void Debug(string message) { }

    /// <inheritdoc />
    public void Info(string message) { }

    /// <inheritdoc />
    public void Warning(string message) { }

    /// <inheritdoc />
    public void Error(string message, Exception? exception = null) { }

    /// <inheritdoc />
    public void Log(LogLevel level, string message, Exception? exception = null) { }
}
