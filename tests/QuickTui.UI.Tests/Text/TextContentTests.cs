using Xunit;

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
        content.Insert('$', new TextPosition(4, 10));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('9', content.Lines[4][9]);
        Assert.Equal('$', content.Lines[4][10]);
    }

    [Fact]
    public void InsertCharAtStart() {
        TextContent content = new TextContent(baseStr);
        content.Insert('$', new TextPosition(0, 0));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('$', content.Lines[0][0]);
        Assert.Equal('a', content.Lines[0][1]);
    }

    [Fact]
    public void InsertCharInMiddle() {
        TextContent content = new TextContent(baseStr);
        content.Insert('$', new TextPosition(2, 6));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('F', content.Lines[2][5]);
        Assert.Equal('$', content.Lines[2][6]);
        Assert.Equal('G', content.Lines[2][7]);
    }

    [Fact]
    public void AppendStringToEnd() {
        TextContent content = new TextContent(baseStr);
        content.Insert("foo", new TextPosition(4, 10));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('9', content.Lines[4][9]);
        Assert.Equal("foo", content.Lines[4].Substring(10, 3));
    }

    [Fact]
    public void InsertStringAtStart() {
        TextContent content = new TextContent(baseStr);
        content.Insert("foo", new TextPosition(0, 0));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal("foo", content.Lines[0].Substring(0, 3));
        Assert.Equal('a', content.Lines[0][3]);
    }

    [Fact]
    public void InsertStringInMiddle() {
        TextContent content = new TextContent(baseStr);
        content.Insert("foo", new TextPosition(2, 6));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal('F', content.Lines[2][5]);
        Assert.Equal("foo", content.Lines[2].Substring(6, 3));
        Assert.Equal('G', content.Lines[2][9]);
    }

    [Fact]
    public void DeleteLastChar() {
        TextContent content = new TextContent(baseStr);
        content.Delete(1, new TextPosition(4, 9));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[4].Length);
        Assert.Equal('8', content.Lines[4][8]);
    }

    [Fact]
    public void DeleteFirstChar() {
        TextContent content = new TextContent(baseStr);
        content.Delete(1, new TextPosition(0, 0));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(15, content.Lines[0].Length);
        Assert.Equal('b', content.Lines[0][0]);
    }

    [Fact]
    public void DeleteMiddleChar() {
        TextContent content = new TextContent(baseStr);
        content.Delete(1, new TextPosition(3, 4));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[3].Length);
        Assert.Equal('T', content.Lines[3][3]);
        Assert.Equal('V', content.Lines[3][4]);
    }

    [Fact]
    public void BackspaceLastChar() {
        TextContent content = new TextContent(baseStr);
        content.Backspace(1, new TextPosition(4, 10));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[4].Length);
        Assert.Equal('8', content.Lines[4][8]);
    }

    [Fact]
    public void BackspaceFirstChar() {
        TextContent content = new TextContent(baseStr);
        content.Backspace(1, new TextPosition(0, 1));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(15, content.Lines[0].Length);
        Assert.Equal('b', content.Lines[0][0]);
    }

    [Fact]
    public void BackspaceMiddleChar() {
        TextContent content = new TextContent(baseStr);
        content.Backspace(1, new TextPosition(3, 5));

        Assert.Equal(5, content.Lines.Count);
        Assert.Equal(9, content.Lines[3].Length);
        Assert.Equal('T', content.Lines[3][3]);
        Assert.Equal('V', content.Lines[3][4]);
    }
}
