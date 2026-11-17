using System;

namespace QuickTui.UI.Text;

/// <inheritdoc/>
public class ContentWindow : IContentWindow {
    private int _height;
    private int _width;
    private TextPosition _origin;

    public ContentWindow() {
        _height = 0;
        _width = 0;
        _origin = new TextPosition(0, 0);
    }

    /// <inheritdoc/>
    public Cell[] ContentBuffer(ITextContent content) {
        Cell[] buffer = Cell.EmptyCells(_width * _height);
        int row = 0;
        for (int line = _origin.Line; line < (_origin.Line + _height); line++) {
            int col = 0;
            foreach (char c in content.Lines[line]) {
                if (col < _origin.Column || col >= (_origin.Column + _width)) {
                    continue;
                }

                buffer[col + (row * _width)] = new Cell(c);
                col++;
            }

            row++;
        }

        return buffer;
    }

    /// <inheritdoc/>
    public TextPosition GetRelativePosition(ITextCaret caret) {
        return new TextPosition(
            caret.Position.Line - _origin.Line,
            caret.Position.Column - _origin.Column
        );
    }

    /// <inheritdoc/>
    public void HandleCaretMove(ITextCaret caret, ITextContent content) {

    }

    /// <inheritdoc/>
    public void HandleResize(int width, int height) {

    }
}
