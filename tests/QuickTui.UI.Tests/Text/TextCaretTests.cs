using System;
using System.Collections.Generic;

using QuickTui.UI.Text;

namespace QuickTui.UI.Tests.Text;

public class TextCaretTests {
    private static readonly string baseStr = """
        abcdefghijklmnop
        qrstuvwxyz
        ABCDEFGHIJKLMNOP
        QRSTUVWXYZ!@#
        0123456789&
        """;

    private static readonly string lorem = """
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Proin suscipit justo sed posuere venenatis. Donec pellentesque,
        metus eu interdum finibus, eros erat mollis dui, vel aliquet
        lacus dolor ut tellus. Aenean ac lacus velit. Etiam tempus
        maximus augue sit amet pharetra. Integer ac velit.
        """;

    [Fact]
    public void MoveToValidPosition() {
        TextCaret caret = new();
        TextContent content = new(baseStr);
        TextPosition newPos = new(1, 3);
        caret.MoveTo(newPos, content);

        Assert.Equal(newPos, caret.Position);
    }

    [Fact]
    public void MovePastLineEndStopsAtLineEnd() {
        TextCaret caret = new();
        TextContent content = new(baseStr);
        TextPosition newPos = new(1, 100);
        caret.MoveTo(newPos, content);

        Assert.Equal(new TextPosition(1, 10), caret.Position);
    }

    [Fact]
    public void MovePastContentEndStopsAtContentEnd() {
        TextCaret caret = new();
        TextContent content = new(baseStr);
        TextPosition newPos = new(100, 100);
        caret.MoveTo(newPos, content);

        Assert.Equal(new TextPosition(4, 11), caret.Position);
    }

    [Fact]
    public void MoveRightFromLineStart() {
        TextCaret caret = new(new TextPosition(1, 0));
        TextContent content = new(baseStr);
        caret.MoveRight(content);

        Assert.Equal(new TextPosition(1, 1), caret.Position);

        caret.MoveRight(content);
        Assert.Equal(new TextPosition(1, 2), caret.Position);
    }

    [Fact]
    public void MoveRightFromMidLine() {
        TextCaret caret = new(new TextPosition(2, 5));
        TextContent content = new(baseStr);
        caret.MoveRight(content);

        Assert.Equal(new TextPosition(2, 6), caret.Position);

        caret.MoveRight(content);
        Assert.Equal(new TextPosition(2, 7), caret.Position);
    }

    [Fact]
    public void MoveRightFromLineEndToNextLine() {
        TextCaret caret = new(new TextPosition(2, 16));
        TextContent content = new(baseStr);
        caret.MoveRight(content);

        Assert.Equal(new TextPosition(3, 0), caret.Position);

        caret.MoveRight(content);
        Assert.Equal(new TextPosition(3, 1), caret.Position);
    }

    [Fact]
    public void MoveLeftFromLineEnd() {
        TextCaret caret = new(new TextPosition(2, 16));
        TextContent content = new(baseStr);
        caret.MoveLeft(content);

        Assert.Equal(caret.Position, new TextPosition(2, 15));

        caret.MoveLeft(content);
        Assert.Equal(caret.Position, new TextPosition(2, 14));
    }

    [Fact]
    public void MoveLeftFromMidLine() {
        TextCaret caret = new(new TextPosition(2, 5));
        TextContent content = new(baseStr);
        caret.MoveLeft(content);

        Assert.Equal(new TextPosition(2, 4), caret.Position);

        caret.MoveLeft(content);
        Assert.Equal(new TextPosition(2, 3), caret.Position);
    }

    [Fact]
    public void MoveLeftFromLineStartToPreviousLine() {
        TextCaret caret = new(new TextPosition(2, 0));
        TextContent content = new(baseStr);
        caret.MoveLeft(content);

        Assert.Equal(new TextPosition(1, 10), caret.Position);

        caret.MoveLeft(content);
        Assert.Equal(new TextPosition(1, 9), caret.Position);
    }

    [Fact]
    public void MoveDownAllLines() {
        TextCaret caret = new(new TextPosition(4, 11));
        TextContent content = new(baseStr);

        caret.MoveDown(content);
        Assert.Equal(new TextPosition(3, 11), caret.Position);

        caret.MoveDown(content);
        Assert.Equal(new TextPosition(2, 11), caret.Position);

        caret.MoveDown(content);
        Assert.Equal(new TextPosition(1, 10), caret.Position);

        caret.MoveDown(content);
        Assert.Equal(new TextPosition(0, 11), caret.Position);
    }

    [Fact]
    public void MoveUpAllLines() {
        TextCaret caret = new(new TextPosition(0, 13));
        TextContent content = new(baseStr);

        caret.MoveUp(content);
        Assert.Equal(new TextPosition(1, 10), caret.Position);

        caret.MoveUp(content);
        Assert.Equal(new TextPosition(2, 13), caret.Position);

        caret.MoveUp(content);
        Assert.Equal(new TextPosition(3, 13), caret.Position);

        caret.MoveUp(content);
        Assert.Equal(new TextPosition(4, 11), caret.Position);
    }

    [Fact]
    public void MoveToLineStart() {
        TextCaret caret = new(new TextPosition(2, 14));
        TextContent content = new(baseStr);
        caret.MoveToLineStart(content);

        Assert.Equal(new TextPosition(2, 0), caret.Position);
    }

    [Fact]
    public void MoveToLineEnd() {
        TextCaret caret = new(new TextPosition(2, 1));
        TextContent content = new(baseStr);
        caret.MoveToLineEnd(content);

        Assert.Equal(new TextPosition(2, 16), caret.Position);
    }

    [Fact]
    public void MoveToContentStart() {
        TextCaret caret = new(new TextPosition(2, 14));
        TextContent content = new(baseStr);
        caret.MoveToStart(content);

        Assert.Equal(new TextPosition(0, 0), caret.Position);
    }

    [Fact]
    public void MoveToContentEnd() {
        TextCaret caret = new(new TextPosition(2, 1));
        TextContent content = new(baseStr);
        caret.MoveToEnd(content);

        Assert.Equal(new TextPosition(4, 11), caret.Position);
    }

    [Fact]
    public void MovePreviousWordFromContentStart() {
        TextCaret caret = new(new TextPosition(0, 0));
        TextContent content = new(lorem);
        caret.MovePreviousWord(content);

        Assert.Equal(new TextPosition(0, 0), caret.Position);
    }

    [Fact]
    public void MovePreviousWordFromWordBoundry() {
        TextCaret caret = new(new TextPosition(1, 21));
        TextContent content = new(lorem);
        caret.MovePreviousWord(content);

        Assert.Equal(new TextPosition(1, 15), caret.Position);
    }

    [Fact]
    public void MovePreviousWordFromMiddleOfWord() {
        TextCaret caret = new(new TextPosition(0, 36));
        TextContent content = new(lorem);
        caret.MovePreviousWord(content);

        Assert.Equal(new TextPosition(0, 28), caret.Position);
    }

    [Fact]
    public void MovePreviousWordMultipleWhiteSpace() {
        TextCaret caret = new(new TextPosition(0, 22));
        TextContent content = new("Words with lots       of space");
        caret.MovePreviousWord(content);

        Assert.Equal(new TextPosition(0, 11), caret.Position);
    }

    [Fact]
    public void MovePreviousWordToPreviousLine() {
        TextCaret caret = new(new TextPosition(1, 0));
        TextContent content = new(lorem);
        caret.MovePreviousWord(content);

        Assert.Equal(new TextPosition(0, 51), caret.Position);
    }

    [Fact]
    public void MoveNextWordFromContentEnd() {
        TextCaret caret = new(new TextPosition(4, 50));
        TextContent content = new(lorem);
        caret.MoveNextWord(content);

        Assert.Equal(new TextPosition(4, 50), caret.Position);
    }

    [Fact]
    public void MoveNextWordFromWordBoundry() {
        TextCaret caret = new(new TextPosition(2, 8));
        TextContent content = new(lorem);
        caret.MoveNextWord(content);

        Assert.Equal(new TextPosition(2, 17), caret.Position);
    }

    [Fact]
    public void MoveNextWordFromMiddleOfWord() {
        TextCaret caret = new(new TextPosition(2, 12));
        TextContent content = new(lorem);
        caret.MoveNextWord(content);

        Assert.Equal(new TextPosition(2, 17), caret.Position);
    }

    [Fact]
    public void MoveNextWordMultipleWhiteSpace() {
        TextCaret caret = new(new TextPosition(0, 15));
        TextContent content = new("Words with lots       of space");
        caret.MoveNextWord(content);

        Assert.Equal(new TextPosition(0, 24), caret.Position);
    }

    [Fact]
    public void MoveNextWordToNextLine() {
        TextCaret caret = new(new TextPosition(2, 60));
        TextContent content = new(lorem);
        caret.MoveNextWord(content);

        Assert.Equal(new TextPosition(3, 5), caret.Position);
    }

    [Fact]
    public void MoveForwardAndBackward() {
        TextCaret caret = new(new TextPosition(2, 60));
        TextContent content = new(lorem);

        caret.MoveNextWord(content);
        Assert.Equal(new TextPosition(3, 5), caret.Position);

        caret.MoveNextWord(content);
        Assert.Equal(new TextPosition(3, 11), caret.Position);

        caret.MoveNextWord(content);
        Assert.Equal(new TextPosition(3, 14), caret.Position);

        caret.MovePreviousWord(content);
        Assert.Equal(new TextPosition(3, 12), caret.Position);

        caret.MovePreviousWord(content);
        Assert.Equal(new TextPosition(3, 6), caret.Position);

        caret.MoveToLineEnd(content);
        Assert.Equal(new TextPosition(3, 58), caret.Position);
    }
}
