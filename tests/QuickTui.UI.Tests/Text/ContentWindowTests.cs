using System.Linq;

using QuickTui.UI;
using QuickTui.UI.Text;

public class ContentWindowTests {
    private static Cell[] Cells(params string[] rows) {
        return rows
            .SelectMany(row => row.Select(val => new Cell(val)))
            .ToArray();
    }

    private static readonly string baseStr = """
        abcdefghijklmnop
        qrstuvwxyz
        ABCDEFGHIJKLMNOP
        QRSTUVWXYZ
        0123456789
        """;

    // Caret Position tests

    [Fact]
    public void RelativePositionOriginAtStart() {
        ContentWindow window = new();
        window.SetDimensions(5, 5);
        window.SetOrigin(new TextPosition(0, 0));
        TextCaret absCaret = new TextCaret(new TextPosition(2, 2));
        TextPosition relPos = window.GetRelativePosition(absCaret);

        Assert.Equal(new TextPosition(2, 2), relPos);
    }

    [Fact]
    public void RelativePositionOriginInMiddle() {
        ContentWindow window = new();
        window.SetDimensions(5, 5);
        window.SetOrigin(new TextPosition(1, 2));
        TextCaret absCaret = new TextCaret(new TextPosition(2, 4));
        TextPosition relPos = window.GetRelativePosition(absCaret);

        Assert.Equal(new TextPosition(1, 2), relPos);
    }

    [Fact]
    public void RelativePositionOriginAtEnd() {
        ContentWindow window = new();
        window.SetDimensions(5, 5);
        window.SetOrigin(new TextPosition(3, 3));
        TextCaret absCaret = new TextCaret(new TextPosition(7, 6));
        TextPosition relPos = window.GetRelativePosition(absCaret);

        Assert.Equal(new TextPosition(4, 3), relPos);
    }

    // Content buffer tests

    [Fact]
    public void WindowAtStartFullContent() {
        TextContent content = new(baseStr);
        ContentWindow window = new();
        window.SetDimensions(16, 5);
        window.SetOrigin(new TextPosition(0, 0));

        Cell[] buffer = window.ContentBuffer(content);
        Cell[] expected = Cells(
            "abcdefghijklmnop",
            "qrstuvwxyz".PadRight(16),
            "ABCDEFGHIJKLMNOP",
            "QRSTUVWXYZ".PadRight(16),
            "0123456789".PadRight(16)
        );

        Assert.Equal(expected, buffer);
    }

    [Fact]
    public void WindowAtStartPartialContent() {
        TextContent content = new(baseStr);
        ContentWindow window = new();
        window.SetDimensions(10, 4);
        window.SetOrigin(new TextPosition(0, 0));

        Cell[] buffer = window.ContentBuffer(content);
        Cell[] expected = Cells(
            "abcdefghij",
            "qrstuvwxyz",
            "ABCDEFGHIJ",
            "QRSTUVWXYZ"
        );

        Assert.Equal(expected, buffer);
    }

    [Fact]
    public void WindowMidPage() {
        TextContent content = new(baseStr);
        ContentWindow window = new();
        window.SetDimensions(16, 3);
        window.SetOrigin(new TextPosition(1, 0));

        Cell[] buffer = window.ContentBuffer(content);
        Cell[] expected = Cells(
            "qrstuvwxyz".PadRight(16),
            "ABCDEFGHIJKLMNOP",
            "QRSTUVWXYZ".PadRight(16)
        );

        Assert.Equal(expected, buffer);
    }

    [Fact]
    public void WindowMidPageWideRows() {
        TextContent content = new(baseStr);
        ContentWindow window = new();
        window.SetDimensions(10, 3);
        window.SetOrigin(new TextPosition(1, 2));

        Cell[] buffer = window.ContentBuffer(content);
        Cell[] expected = Cells(
            "stuvwxyz".PadRight(10),
            "CDEFGHIJKL",
            "STUVWXYZ".PadRight(10)
        );

        Assert.Equal(expected, buffer);
    }
}
