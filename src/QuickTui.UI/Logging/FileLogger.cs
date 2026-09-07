using System;
using System.IO;
using System.Text;

namespace QuickTui.UI.Logging;

/// <summary>
/// Logger for internal app usage. Writes logs to a file.
/// </summary>
public class FileLogger : ILogger, IDisposable {
    private readonly object _sync = new object();
    private readonly StreamWriter _writer;
    private bool _disposed;

    public FileLogger(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.Read);
        _writer = new(stream, new UTF8Encoding(false)) {
            AutoFlush = true,
        };
        _disposed = false;
    }

    /// <inheritdoc />
    public void Debug(string message) {
        Log(LogLevel.Debug, message);
    }

    /// <inheritdoc />
    public void Info(string message) {
        Log(LogLevel.Info, message);
    }

    /// <inheritdoc />
    public void Warning(string message) {
        Log(LogLevel.Warning, message);
    }

    /// <inheritdoc />
    public void Error(string message, Exception? exception = null) {
        Log(LogLevel.Error, message, exception);
    }

    /// <inheritdoc />
    public void Log(LogLevel level, string message, Exception? exception = null) {
        Write(level, message, exception);
    }

    public void Dispose() {
        lock (_sync) {
            if (_disposed) {
                return;
            }

            _writer.Dispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    private void Write(LogLevel level, string message, Exception? exception = null) {
        ArgumentNullException.ThrowIfNull(message);

        lock (_sync) {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _writer.Write(DateTimeOffset.UtcNow.ToString("O"));
            _writer.Write($" [{level.ToString("G").ToUpper()}] ");
            _writer.WriteLine(message);

            if (exception is not null) {
                _writer.WriteLine(exception);
            }
        }
    }
}
