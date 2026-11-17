using System;
using System.Drawing;
using System.Diagnostics;

using QuickTui.UI.Events;

namespace QuickTui.UI.Text;

/// <inheritdoc/>
public class TextEditor : ITextEditor {
    private ITextContent _content;
    private ITextCaret _caret;

    public TextEditor(ITextContent content, ITextCaret caret) {
        _content = content;
        _caret = caret;

        Debug.Assert(_content.LineCount() > 0, "Expected TextContent to contain at least one line");
    }

    public Cell[] ContentBuffer() {
        string content = _content.ToString();

        // TODO: this is obviously not right. Needs to be width/height of content window.
        return TextBuffer.FromString(content, _content.Lines[0].Length, _content.Lines.Count);
    }
    
    public Cursor GetCursor() {
        return new Cursor() {
            Position = new Point(_caret.Position.Column, _caret.Position.Line)
        };
    }

    public void HandleInput(UiEvent uiEvent) {
        TextPosition newPosition = uiEvent switch {
            TextInputEvent e => _content.Insert(e.Text, _caret.Position),
            KeyEvent e => HandleKeyEvent(e),
            _ => throw new NotSupportedException(
                $"Cannot parse event of type {uiEvent.GetType()} to EditorCommand"
            )
        };

        if (newPosition != _caret.Position) {
            _caret.MoveTo(newPosition, _content);
        }
    }

    private TextPosition HandleKeyEvent(KeyEvent keyEvent) {
        // TODO: It be nice if there was a more reliable way to keep TextCaret and TextContent
        //       in sync. Maybe a signal/slot?
        TextPosition newPosition = _caret.Position;
        switch (keyEvent.Key) {
            case ConsoleKey.Backspace:
                newPosition = _content.Backspace(1, _caret.Position);
                break;
            case ConsoleKey.Delete:
                newPosition = _content.Delete(1, _caret.Position);
                break;
            case ConsoleKey.Enter:
                newPosition = _content.InsertLineBreak(_caret.Position);
                break;
            case ConsoleKey.LeftArrow:
                if (keyEvent.Modifiers.HasFlag(ConsoleModifiers.Alt)) {
                    _caret.MovePreviousWord(_content);
                } else {
                    _caret.MoveLeft(_content);
                }
                break;
            case ConsoleKey.RightArrow:
                if (keyEvent.Modifiers.HasFlag(ConsoleModifiers.Alt)) {
                    _caret.MoveNextWord(_content);
                } else {
                    _caret.MoveRight(_content);
                }
                break;
            case ConsoleKey.DownArrow:
                // TODO: this is platform specific behavior (mac)
                if (keyEvent.Modifiers.HasFlag(ConsoleModifiers.Control)) {
                    _caret.MoveToEnd(_content);
                } else {
                    _caret.MoveDown(_content);
                }
                break;
            case ConsoleKey.UpArrow:
                // TODO: this is platform specific behavior (mac)
                if (keyEvent.Modifiers.HasFlag(ConsoleModifiers.Control)) {
                    _caret.MoveToStart(_content);
                } else {
                    _caret.MoveUp(_content);
                }
                break;
            case ConsoleKey.PageDown:
                _caret.MovePageDown(_content);
                break;
            case ConsoleKey.PageUp:
                _caret.MovePageUp(_content);
                break;
            case ConsoleKey.End:
                _caret.MoveToLineEnd(_content);
                break;
            case ConsoleKey.Home:
                _caret.MoveToLineStart(_content);
                break;
            default:
                throw new NotSupportedException(
                    $"Unsupported KeyEvent cannot be convert to EditorCommand. Key: {keyEvent.Key}"
                );
        };

        return newPosition;
    }
}
