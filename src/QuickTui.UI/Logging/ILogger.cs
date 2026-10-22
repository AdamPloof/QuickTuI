using System;

namespace QuickTui.UI.Logging;

/// <summary>
/// The internal logger service.
/// </summary>
public interface ILogger {
    /// <summary>
    /// Debug information which would generally be surpressed in production builds.
    /// </summary>
    public void Debug(string message);

    /// <summary>
    /// Interesting events
    /// </summary>
    public void Info(string message);

    /// <summary>
    /// Important events that require notifications but are not critical.
    /// </summary>
    public void Warning(string message);

    /// <summary>
    /// Critical errors that require notification.
    /// </summary>
    public void Error(string message, Exception? exception = null);

    /// <summary>
    /// Base logging method.
    /// </summary>
    public void Log(LogLevel level, string message, Exception? exception = null);
}
