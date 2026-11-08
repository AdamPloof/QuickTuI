using Xunit;
using System;
using System.Collections.Generic;

using QuickTui.UI.Text;

namespace QuickTui.UI.Tests.Text;

public class TextContentTests {
    private static readonly string baseStr = """
        abcdefghijklmnop
        qrstuvwxyz
        ABCDEFGHIJKLMNOP
        QRSTUVWXYZ
        0123456789
        """;

    [Fact]
    public void AppendCharToEnd() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Insert('$', new TextPosition(4, 10));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('9', content.Lines[4][9]);
        Assert.Equal('$', content.Lines[4][10]);
        Assert.Equal(new TextPosition(4, 11), pos);
    }

    [Fact]
    public void InsertCharAtStart() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Insert('$', new TextPosition(0, 0));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('$', content.Lines[0][0]);
        Assert.Equal('a', content.Lines[0][1]);
        Assert.Equal(new TextPosition(0, 1), pos);
    }

    [Fact]
    public void InsertCharInMiddle() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Insert('$', new TextPosition(2, 6));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('F', content.Lines[2][5]);
        Assert.Equal('$', content.Lines[2][6]);
        Assert.Equal('G', content.Lines[2][7]);
        Assert.Equal(new TextPosition(2, 7), pos);
    }

    [Fact]
    public void AppendStringToEnd() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Insert("foo", new TextPosition(4, 10));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('9', content.Lines[4][9]);
        Assert.Equal("foo", content.Lines[4].Substring(10, 3));
        Assert.Equal(new TextPosition(4, 13), pos);
    }

    [Fact]
    public void InsertStringAtStart() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Insert("foo", new TextPosition(0, 0));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal("foo", content.Lines[0].Substring(0, 3));
        Assert.Equal('a', content.Lines[0][3]);
        Assert.Equal(new TextPosition(0, 3), pos);
    }

    [Fact]
    public void InsertStringInMiddle() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Insert("foo", new TextPosition(2, 6));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('F', content.Lines[2][5]);
        Assert.Equal("foo", content.Lines[2].Substring(6, 3));
        Assert.Equal('G', content.Lines[2][9]);
        Assert.Equal(new TextPosition(2, 9), pos);
    }

    [Fact]
    public void DeleteLastChar() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Delete(1, new TextPosition(4, 9));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[4].Length);
        Assert.Equal('8', content.Lines[4][8]);
        Assert.Equal(new TextPosition(4, 9), pos);
    }

    [Fact]
    public void DeleteFirstChar() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Delete(1, new TextPosition(0, 0));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(15, content.Lines[0].Length);
        Assert.Equal('b', content.Lines[0][0]);
        Assert.Equal(new TextPosition(0, 0), pos);
    }

    [Fact]
    public void DeleteMiddleChar() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Delete(1, new TextPosition(3, 4));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[3].Length);
        Assert.Equal('T', content.Lines[3][3]);
        Assert.Equal('V', content.Lines[3][4]);
        Assert.Equal(new TextPosition(3, 4), pos);
    }

    [Fact]
    public void BackspaceLastChar() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Backspace(1, new TextPosition(4, 10));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[4].Length);
        Assert.Equal('8', content.Lines[4][8]);
        Assert.Equal(new TextPosition(4, 9), pos);
    }

    [Fact]
    public void BackspaceFirstChar() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Backspace(1, new TextPosition(0, 1));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(15, content.Lines[0].Length);
        Assert.Equal('b', content.Lines[0][0]);
        Assert.Equal(new TextPosition(0, 0), pos);
    }

    [Fact]
    public void BackspaceMiddleChar() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Backspace(1, new TextPosition(3, 5));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[3].Length);
        Assert.Equal('T', content.Lines[3][3]);
        Assert.Equal('V', content.Lines[3][4]);
        Assert.Equal(new TextPosition(3, 4), pos);
    }

    [Fact]
    public void DeleteNewLine() {
        List<string> lines = [
            "abc",
            "def"
        ];
        TextContent content = new(lines);
        TextPosition pos = content.Delete(1, new TextPosition(0, 3));

        Assert.Equal(new TextPosition(0, 3), pos);
        Assert.Equal("abcdef", content.ToString());
    }

    [Fact]
    public void BackspaceNewLine() {
        List<string> lines = [
            "abc",
            "def"
        ];
        TextContent content = new(lines);
        TextPosition pos = content.Backspace(1, new TextPosition(1, 0));

        Assert.Equal(new TextPosition(0, 3), pos);
        Assert.Equal("abcdef", content.ToString());
    }

    [Fact]
    public void DeleteAcrossLines() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Delete(8, new TextPosition(1, 5));

        Assert.Equal(new TextPosition(1, 5), pos);
        string expected = """
            abcdefghijklmnop
            qrstuCDEFGHIJKLMNOP
            QRSTUVWXYZ
            0123456789
            """;
        Assert.Equal(expected, content.ToString());
    }

    [Fact]
    public void DeleteEntireLine() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Delete(11, new TextPosition(1, 0));

        Assert.Equal(new TextPosition(1, 0), pos);
        string expected = """
            abcdefghijklmnop
            ABCDEFGHIJKLMNOP
            QRSTUVWXYZ
            0123456789
            """;
        Assert.Equal(expected, content.ToString());
    }

    [Fact]
    public void DeleteEntireLineExceptNewLine() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Delete(10, new TextPosition(1, 0));

        Assert.Equal(new TextPosition(1, 0), pos);
        string expected = """
            abcdefghijklmnop

            ABCDEFGHIJKLMNOP
            QRSTUVWXYZ
            0123456789
            """;
        Assert.Equal(expected, content.ToString());
    }

    [Fact]
    public void BackspaceAcrossLines() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Backspace(8, new TextPosition(1, 5));

        Assert.Equal(new TextPosition(0, 14), pos);
        string expected = """
            abcdefghijklmnvwxyz
            ABCDEFGHIJKLMNOP
            QRSTUVWXYZ
            0123456789
            """;
        Assert.Equal(expected, content.ToString());
    }

    [Fact]
    public void BackspaceEntireLine() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Backspace(11, new TextPosition(1, 10));

        Assert.Equal(new TextPosition(0, 16), pos);
        string expected = """
            abcdefghijklmnop
            ABCDEFGHIJKLMNOP
            QRSTUVWXYZ
            0123456789
            """;
        Assert.Equal(expected, content.ToString());
    }

    [Fact]
    public void BackspaceEntireLineExceptNewLine() {
        TextContent content = new TextContent(baseStr);
        TextPosition pos = content.Backspace(10, new TextPosition(1, 10));

        Assert.Equal(new TextPosition(1, 0), pos);
        string expected = """
            abcdefghijklmnop

            ABCDEFGHIJKLMNOP
            QRSTUVWXYZ
            0123456789
            """;
        Assert.Equal(expected, content.ToString());
    }
}

/// <summary>
/// Tests for calculating new positions by number of characters.
/// </summary>
public class PositionDeltaTests {
    private static readonly string baseStr = """
        abcdefghijklmnop
        qrstuvwxyz
        ABCDEFGHIJKLMNOP
        QRSTUVWXYZ
        0123456
        """;

    [Fact]
    public void NoDelta() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(1, 3);
        TextPosition newPos = content.PositionDelta(oldPos, 0);

        Assert.Equal(oldPos, newPos);
    }

    [Fact]
    public void MoveForwardSameLine() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(1, 3);
        TextPosition newPos = content.PositionDelta(oldPos, 3);

        Assert.Equal(new TextPosition(1, 6), newPos);
    }

    [Fact]
    public void MoveBackwardSameLine() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(1, 3);
        TextPosition newPos = content.PositionDelta(oldPos, -3);

        Assert.Equal(new TextPosition(1, 0), newPos);
    }

    [Fact]
    public void MoveForwardNextLine() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(1, 3);
        TextPosition newPos = content.PositionDelta(oldPos, 9);

        Assert.Equal(new TextPosition(2, 1), newPos);
    }

    [Fact]
    public void MoveBackwardPrevLineSimple() {
        List<string> lines = [
            "abc",
            "def"
        ];
        TextContent content = new(lines);
        TextPosition oldPos = new(1, 0);
        TextPosition newPos = content.PositionDelta(oldPos, -1);

        Assert.Equal(new TextPosition(0, 3), newPos);
    }

    [Fact]
    public void MoveBackwardPrevLine() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(1, 3);
        TextPosition newPos = content.PositionDelta(oldPos, -5);

        Assert.Equal(new TextPosition(0, 15), newPos);
    }

    [Fact]
    public void MoveForwardMultipleLines() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(2, 3);
        TextPosition newPos = content.PositionDelta(oldPos, 27);

        Assert.Equal(new TextPosition(4, 2), newPos);
    }

    [Fact]
    public void MoveBackwardMultipleLines() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(3, 3);
        TextPosition newPos = content.PositionDelta(oldPos, -24);

        Assert.Equal(new TextPosition(1, 7), newPos);
    }

    [Fact]
    public void MoveForwardPastEndReturnsEnd() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(3, 3);
        TextPosition newPos = content.PositionDelta(oldPos, 1000);

        Assert.Equal(new TextPosition(4, 7), newPos);
    }

    [Fact]
    public void MoveBackwardPastStartReturnsStart() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(3, 3);
        TextPosition newPos = content.PositionDelta(oldPos, -1000);

        Assert.Equal(new TextPosition(0, 0), newPos);
    }

    [Fact]
    public void PositionPastLineEndThrows() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(0, 20);

        Assert.Throws<ArgumentOutOfRangeException>(() => {
            content.PositionDelta(oldPos, 1);
        });
    }

    [Fact]
    public void PositionPastLastLineThrows() {
        TextContent content = new(baseStr);
        TextPosition oldPos = new(10, 0);

        Assert.Throws<ArgumentOutOfRangeException>(() => {
            content.PositionDelta(oldPos, 1);
        });
    }
}
