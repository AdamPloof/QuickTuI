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
    public IContentWindow SetDimensions(int width, int height) {
        _width = width;
        _height = height;

        return this;
    }

    /// <inheritdoc/>
    public IContentWindow SetOrigin(TextPosition origin) {
        _origin = origin;

        return this;
    }

    /// <inheritdoc/>
    public Cell[] ContentBuffer(ITextContent content) {
        Cell[] buffer = Cell.EmptyCells(_width * _height);
        if (_origin.Line >= content.LineCount()) {
            return buffer;   
        }

        int row = 0;
        for (int lineIdx = _origin.Line; lineIdx < (_origin.Line + _height); lineIdx++) {
            string line = content.Lines[lineIdx];
            if (_origin.Column >= line.Length) {
                row++;
                continue;
            }

            int visibleLength = Math.Min(_width, line.Length - _origin.Column);
            ReadOnlySpan<char> visible = line.AsSpan(_origin.Column, visibleLength);
            for (int col = 0; col < visible.Length; col++) {
                buffer[col + (row * _width)] = new Cell(visible[col]);
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
