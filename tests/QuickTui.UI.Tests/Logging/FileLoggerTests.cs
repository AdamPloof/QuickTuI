using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

using QuickTui.UI.Logging;

namespace QuickTui.UI.Tests.Logging;

public class FileLoggerTests {
    private readonly string _baseLogPath = Path.Combine(
        AppContext.BaseDirectory,
        "./var/logs/"
    );

    [Fact]
    public void LogFileIsCreated() {
        string logPath = Path.Combine(_baseLogPath, "file_created.log");
        FileLogger logger = new(logPath);
        logger.Debug("test");

        Assert.True(File.Exists(logPath));
        Cleanup(logPath);
    }

    [Fact]
    public void DebugLogIsWritten() {
        string msg = "First line";
        string logPath = Path.Combine(_baseLogPath, "deubg_new.log");
        FileLogger logger = new(logPath);
        logger.Debug(msg);
        List<string> lines = ReadLogLines(logPath);

        Assert.Single(lines);
        Assert.Contains("DEBUG", lines[0]);
        Assert.Contains(msg, lines[0]);

        Cleanup(logPath);
    }

    [Fact]
    public void LogsAreAppended() {
        string msg1 = "First line";
        string msg2 = "Second line";
        string msg3 = "Third line";
        string logPath = Path.Combine(_baseLogPath, "log_append.log");
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

        Cleanup(logPath);
    }

    [Fact]
    public void LogsAreAppendedWithException() {
        string msg1 = "First line";
        string msg2 = "Second line";
        string msg3 = "Third line";
        string logPath = Path.Combine(_baseLogPath, "exception_log.log");
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

        Cleanup(logPath);
    }

    [Fact]
    public void EmptyLogPathThrows() {
        Assert.Throws<ArgumentException>(() => {
            FileLogger logger = new FileLogger("");
        });
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

    private void Cleanup(string logPath) {
        File.Delete(logPath);
    }
}
