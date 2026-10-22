using System;
using System.Diagnostics;

namespace QuickTui.UI.Text;

/// <inheritdoc/>
public class TextEditor : ITextEditor {
    private TextContent _content;
    private TextPosition _cursorPos;

    /// <summary>
    /// Track the column to use when navigating vertically through lines.
    /// </summary>
    private int _requestedColumn;

    public TextEditor() {
        _content = new();
        _cursorPos = TextPosition.Start;
        _requestedColumn = 0;

        Debug.Assert(_content.Lines.Count > 0, "Expected TextContent to contain at least one line");
    }

    /// <inheritdoc/>
    public void HandleCommand(EditorCommand command) {
        switch (command) {
            case InsertText insert:
                // TODO: handle me
                break;
            case Backspace backspace:
                // TODO: handle me
                break;
            case Delete delete:
                // TODO: handle me
                break;
            case InsertLineBreak enter:
                // TODO: handle me
                break;
            case MoveLeft moveLeft:
                // TODO: handle me
                break;
            case MoveRight moveRight:
                // TODO: handle me
                break;
            case MoveDown moveDown:
                // TODO: handle me
                break;
            case MoveUp moveUp:
                // TODO: handle me
                break;
            case MovePageUp movePageUp:
                // TODO: handle me
                break;
            case MovePageDown movePageDown:
                // TODO: handle me
                break;
            case ToggleInsertMode toggleInsertMode:
                // TODO: handle me
                break;
            default:
                throw new NotImplementedException(
                    $"No handler available for command: {command.GetType()}"
                );
        }
    }

    public void MoveCursorRight() {
        if (_content.Lines.Count == 0) {
            return;
        }

        if (_cursorPos.Column == _content.Lines[_cursorPos.Line].Length) {
            if (_cursorPos.Line == _content.Lines.Count - 1) {
                // Already at end of content
                return;
            }

            // Move to first char on next line
            _cursorPos = new TextPosition(_cursorPos.Line + 1, 0);
        } else {
            // Move to next char on current line
            _cursorPos = new TextPosition(_cursorPos.Line, _cursorPos.Column + 1);
        }

        _requestedColumn = _cursorPos.Column;

        AssertCursorIsValid();
    }

    public void MoveCursorLeft() {
        if (_content.Lines.Count == 0) {
            return;
        }

        if (_cursorPos.Column == 0) {
            if (_cursorPos.Line == 0) {
                // Already at beginning of content
                return;
            }

            // Move to last char on previous line
            _cursorPos = new TextPosition(
                _cursorPos.Line - 1,
                _content.Lines[_cursorPos.Line - 1].Length
            );
        } else {
            // Move to previous char on current line
            _cursorPos = new TextPosition(_cursorPos.Line, _cursorPos.Column - 1);
        }

        _requestedColumn = _cursorPos.Column;

        AssertCursorIsValid();
    }

    public void MoveCursorNextWord() {
        // TODO: implement me
        MoveCursorRight();
    }
    
    public void MoveCursorPrevWord() {
        // TODO: implement me
        MoveCursorLeft();
    }

    public void MoveCursorToLineStart() {
        _cursorPos = new TextPosition(_cursorPos.Line, 0);
        _requestedColumn = _cursorPos.Column;
    }

    public void MoveCursorToLineEnd() {
        _cursorPos = new TextPosition(
            _cursorPos.Line,
            _content.Lines[_cursorPos.Line].Length
        );
        _requestedColumn = _cursorPos.Column;
    }

    public void MoveCursorToContentStart() {
        _cursorPos = TextPosition.Start;
        _requestedColumn = _cursorPos.Column;
    }

    public void MoveCursorToContentEnd() {
        _cursorPos = new TextPosition(
            _content.Lines.Count - 1,
            _content.Lines[^1].Length
        );
        _requestedColumn = _cursorPos.Column;
    }

    public void MoveCursorToPreviousLine() {
        
    }

    public void MoveCursorToNextLine() {
        
    }

    private void AssertCursorIsValid() {
        Debug.Assert(
            _cursorPos.Line < 0,
            "Cursor Line is less than zero"
        );
        Debug.Assert(
            _cursorPos.Line > _content.Lines.Count - 1,
            "Cursor Line is greater than content lines"
        );
        Debug.Assert(
            _cursorPos.Column < 0,
            "Cursor Column is less than zero"
        );
        Debug.Assert(
            _cursorPos.Column > _content.Lines[_cursorPos.Line].Length,
            "Cursor Column is greater than the last column of the line"
        );
    }
}
