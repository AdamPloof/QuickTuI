using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

using QuickTui.UI.Logging;

namespace QuickTui.UI.Tests.Logging;

public class FileLoggerTests : IDisposable {
    private readonly string _logDir;

    public FileLoggerTests() {
        _logDir = Path.Combine(
            Path.GetTempPath(),
            nameof(FileLoggerTests),
            Guid.NewGuid().ToString("N")
        );

        Directory.CreateDirectory(_logDir);
    }

    [Fact]
    public void LogFileIsCreated() {
        string logPath = GetLogPath("file_created.log");
        FileLogger logger = new(logPath);
        logger.Debug("test");

        Assert.True(File.Exists(logPath));
    }

    [Fact]
    public void DebugLogIsWritten() {
        string msg = "First line";
        string logPath = GetLogPath("deubg_new.log");
        FileLogger logger = new(logPath);
        logger.Debug(msg);
        List<string> lines = ReadLogLines(logPath);

        Assert.Single(lines);
        Assert.Contains("DEBUG", lines[0]);
        Assert.Contains(msg, lines[0]);
    }

    [Fact]
    public void LogsAreAppended() {
        string msg1 = "First line";
        string msg2 = "Second line";
        string msg3 = "Third line";
        string logPath = GetLogPath("log_append.log");
        FileLogger logger = new(logPath);
        logger.Debug(msg1);
        logger.Info(msg2);
        logger.Warning(msg3);
        List<string> lines = ReadLogLines(logPath);

        Assert.Equal(3, lines.Count);
        Assert.Contains("DEBUG", lines[0]);
        Assert.Contains(msg1, lines[0]);

        Assert.Contains("INFO", lines[1]);
        Assert.Contains(msg2, lines[1]);

        Assert.Contains("WARNING", lines[2]);
        Assert.Contains(msg3, lines[2]);
    }

    [Fact]
    public void LogsAreAppendedWithException() {
        string msg1 = "First line";
        string msg2 = "Second line";
        string msg3 = "Third line";
        string logPath = GetLogPath("exception_log.log");
        FileLogger logger = new(logPath);
        logger.Debug(msg1);
        logger.Info(msg2);

        Exception e = new Exception("Test Exception");
        logger.Error(msg3, e);
        List<string> lines = ReadLogLines(logPath);

        Assert.Equal(4, lines.Count);
        Assert.Contains("DEBUG", lines[0]);
        Assert.Contains(msg1, lines[0]);

        Assert.Contains("INFO", lines[1]);
        Assert.Contains(msg2, lines[1]);

        Assert.Contains("ERROR", lines[2]);
        Assert.Contains(msg3, lines[2]);
        Assert.Contains("Test Exception", lines[3]);
    }

    [Fact]
    public void EmptyLogPathThrows() {
        Assert.Throws<ArgumentException>(() => {
            FileLogger logger = new FileLogger("");
        });
    }

    public void Dispose() {
        Directory.Delete(_logDir, recursive: true);
    }

    private List<string> ReadLogLines(string logPath) {
        List<string> lines = [];
        using (StreamReader reader = new StreamReader(logPath)) {
            string? line = reader.ReadLine();
            while (line is not null) {
                lines.Add(line);
                line = reader.ReadLine();
            }
        }

        return lines;
    }

    private string GetLogPath(string filename) {
        return Path.Combine(_logDir, filename);
    }
}
