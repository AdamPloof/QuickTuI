using System;

namespace QuickTui.UI.Text;

/// <summary>
/// Zero-based position within TextContent.
/// </summary>
/// <param name="Line"></param>
/// <param name="Column"></param>
public readonly record struct TextPosition {
    public int Line { get; }
    public int Column { get; }

    public TextPosition(int line, int column) {
        ArgumentOutOfRangeException.ThrowIfNegative(line);
        ArgumentOutOfRangeException.ThrowIfNegative(column);

        Line = line;
        Column = column;
    }

    public static TextPosition Start => default;
}
