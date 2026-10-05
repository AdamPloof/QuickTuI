using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using QuickTui.UI;
using QuickTui.UI.App;
using QuickTui.UI.Drawing;
using QuickTui.UI.Events;
using QuickTui.UI.Layout;
using QuickTui.UI.Style;

namespace QuickTui.UI.Widgets;

/// <summary>
/// A text box widget for displaying text from user input. 
/// </summary>
public class TextBox : Widget {
    /// <inheritdoc />
    public override bool AcceptsFocus { get; protected set; } = true;

    /// <summary>
    /// When true, the TextBox will ignore user input. Content can still be
    /// set directly via SetContent().
    /// </summary>
    public bool ReadOnly { get; set; } = false;

    private List<string> _lines;
    private CellStyle _style;
    private Cell[] _buffer;
    private bool _insertMode = false;

    public TextBox(IWidget? parent, string? content, CellStyle? style) : base(parent) {
        _style = style ?? new CellStyle();
        _buffer = [];
        _lines = NormalizeContent(content ?? "").Split('\n').ToList();

        // TODO: should move cursor position management to a separate class
        _cursor.Position = new Point(0, 0);

        AddHandler<TextInputEvent>(HandleTextInput);
        AddHandler<KeyEvent>(HandleKeyInput);
    }

    /// <summary>
    /// Set the text content.
    /// </summary>
    /// <remarks>
    /// Even if the text box is ReadOnly, it is still possible to set the content
    /// via this method. ReadOnly prevents text content from being edited via
    /// input events.
    /// </remarks>
    /// <param name="content"></param>
    public void SetContent(string? content) {
        _lines = NormalizeContent(content ?? "").Split('\n').ToList();
        IsDirty = true;
    }

    /// <summary>
    /// Set the style for the entire text content.
    /// 
    /// TODO: this is temporary. Style needs to be embedded within the content.
    /// </summary>
    /// <param name="style"></param>
    public void SetStyle(CellStyle style) {
        _style = style;
    }

    public IReadOnlyList<string> GetLines() {
        return _lines;
    }

    public bool IsEmpty() {
        if (_lines.Count == 0) {
            return true;
        }

        if (_lines.Count == 1 && _lines[0] == "") {
            return true;
        }

        return false;
    }

    public bool CursorPositionIsValid() {
        if (_cursor.Position.Y < 0 || _cursor.Position.Y > _lines.Count - 1) {
            return false;
        }

        if (_cursor.Position.X < 0 || _cursor.Position.X > _lines[_cursor.Position.Y].Length) {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override Cell[] Paint() {
        if (!IsDirty) {
            return _buffer;
        }

        if (BoundingBox.IsEmpty()) {
            return new Cell[0];
        }

        Cell[] nextBuffer = Cell.EmptyCells(BoundingBox.Width * BoundingBox.Height);
        if (!Border.IsNone()) {
            BorderPainter.ApplyBorder(nextBuffer, BoundingBox.Width, BoundingBox.Height, Border);
        }

        Rect contentRect = ContentArea();
        Cell[] contentCells = TextBuffer.FromLines(
            _lines,
            contentRect.Width,
            contentRect.Height,
            _style
        );

        int col = contentRect.X;
        int row = contentRect.Y;
        for (int i = 0; i < contentCells.Length; i++) {
            if (col - contentRect.X >= contentRect.Width) {
                col = contentRect.X;
                row++;
            }

            nextBuffer[col + (row * BoundingBox.Width)] = contentCells[i];
            col++;
        }

        _buffer = nextBuffer;
        IsDirty = false;

        return _buffer;
    }

    /// <summary>
    /// Does not accept focus.
    /// </summary>
    /// <returns></returns>
    public override bool SetFocus() {
        if (ReadOnly) {
            return false;
        }

        HasFocus = true;

        return true;
    }

    /// <summary>
    /// A Canvas cannot be focused on to begin with, they always respond to clear focus
    /// requests with true.
    /// </summary>
    /// <returns></returns>
    public override bool ClearFocus() {
        HasFocus = false;

        return true;
    }

    /// <inheritdoc />
    public override bool CaptureTabKey() {
        if (ReadOnly) {
            return false;
        }

        return true;
    }

    public void HandleTextInput(TextInputEvent textEvent) {
        InsertText(textEvent.Text);
    }

    public void HandleKeyInput(KeyEvent keyEvent) {
        // TODO: handle selecting text.
        switch (keyEvent.Key) {
            case ConsoleKey.Enter:
                HandleEnter(keyEvent.Modifiers);
                break;
            case ConsoleKey.Tab:
                // TODO: Handle Move cursor arbitrary number of chars
                break;
            case ConsoleKey.Backspace:
                DeleteText(-1);
                break;
            case ConsoleKey.Delete:
                DeleteText(1);
                break;
            case ConsoleKey.PageUp:
                HandlePageUp(keyEvent.Modifiers);
                break;
            case ConsoleKey.PageDown:
                HandlePageDown(keyEvent.Modifiers);
                break;
            case ConsoleKey.LeftArrow:
                HandleLeftArrow(keyEvent.Modifiers);
                break;
            case ConsoleKey.RightArrow:
                HandleRightArrow(keyEvent.Modifiers);
                break;
            case ConsoleKey.UpArrow:
                HandleUpArrow(keyEvent.Modifiers);
                break;
            case ConsoleKey.DownArrow:
                HandleDownArrow(keyEvent.Modifiers);
                break;
            case ConsoleKey.Home:
                HandleHome(keyEvent.Modifiers);
                break;
            case ConsoleKey.End:
                HandleEnd(keyEvent.Modifiers);
                break;
            case ConsoleKey.Insert:
                _insertMode = !_insertMode;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Insert text at the current cursor position.
    /// </summary>
    /// <param name="text"></param>
    public void InsertText(string text) {
        if (!CursorPositionIsValid()) {
            throw new InvalidOperationException(
                $"Cursor position is invalid: ({_cursor.Position.X}, {_cursor.Position.Y})"
            );
        }

        // TODO: Handle Insert mode
        if (IsEmpty()) {
            SetContent(text);
        } else if (_cursor.Position.X == 0) {
            // Cursor is at beginning of line
            string newLine = text + _lines[_cursor.Position.Y];
            _lines[_cursor.Position.Y] = newLine;
        } else if (_cursor.Position.X == _lines[_cursor.Position.Y].Length) {
            // Cursor is at end of line
            string newLine = _lines[_cursor.Position.Y] + text;
            _lines[_cursor.Position.Y] = newLine;
        } else {
            // Cursor is in middle of line
            string oldLine = _lines[_cursor.Position.Y];
            string newLine = oldLine[.._cursor.Position.X] + text + oldLine[_cursor.Position.X..];
            _lines[_cursor.Position.Y] = newLine;
        }
    }

    /// <summary>
    /// Delete text from the current cursor position. Negative length is backspace,
    /// positive length is standard delete.
    /// </summary>
    /// <param name="length"></param>
    public void DeleteText(int length) {
        if (!CursorPositionIsValid()) {
            throw new InvalidOperationException(
                $"Cursor position is invalid: ({_cursor.Position.X}, {_cursor.Position.Y})"
            );
        }

        if (length == 0) {
            return;
        }

        if (length > 0) {
            if (_cursor.Position.X >= _lines[_cursor.Position.Y].Length) {
                return;
            }

            // Delete
            string oldLine = _lines[_cursor.Position.Y];
            string newLine = oldLine[.._cursor.Position.X] + oldLine[(_cursor.Position.X + length)..];
            _lines[_cursor.Position.Y] = newLine;
        } else {
            // Backspace
            if (_cursor.Position.X == 0) {
                return;
            }

            string oldLine = _lines[_cursor.Position.Y];
            string newLine = oldLine[..(_cursor.Position.X - length)] + oldLine[_cursor.Position.X..];
            _lines[_cursor.Position.Y] = newLine;
        }
    }

    public void MoveCursorRight() {
        if (_lines.Count == 0) {
            return;
        }

        if (_cursor.Position.X == _lines[_cursor.Position.Y].Length) {
            if (_cursor.Position.Y == _lines.Count - 1) {
                // Already at end of content
                return;
            }

            // Move to first char on next line
            UpdateCursor(new Point() {
                X = 0,
                Y = _cursor.Position.Y + 1
            });
        } else {
            // Move to next char on current line
            UpdateCursor(new Point() {
                X = _cursor.Position.X + 1,
                Y = _cursor.Position.Y
            });
        }
    }

    public void MoveCursorLeft() {
        if (_lines.Count == 0) {
            return;
        }

        if (_cursor.Position.X == 0) {
            if (_cursor.Position.Y == 0) {
                // Already at beginning of content
                return;
            }

            // Move to last char on previous line
            UpdateCursor(new Point() {
                X = _lines[_cursor.Position.Y - 1].Length,
                Y = _cursor.Position.Y - 1
            });
        } else {
            // Move to previous char on current line
            UpdateCursor(new Point() {
                X = _cursor.Position.X - 1,
                Y = _cursor.Position.Y
            });
        }
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
        UpdateCursor(new Point() {
            X = 0,
            Y = _cursor.Position.Y
        });
    }

    public void MoveCursorToLineEnd() {
        if (_lines.Count == 0) {
            return;
        }

        UpdateCursor(new Point() {
            X = _lines[_cursor.Position.Y].Length,
            Y = _cursor.Position.Y
        });
    }

    public void MoveCursorToContentEnd() {
        if (_lines.Count == 0) {
            return;
        }

        UpdateCursor(new Point() {
            X = _lines[^1].Length,
            Y = _lines.Count - 1
        });
    }

    /// <summary>
    /// Updates the cursor postion, style, and visibility. Emits a CursorChanged Event.
    /// </summary>
    /// <param name="cursor"></param>
    public void UpdateCursor(Point position) {
        _cursor.Position = position;
    }

    /// <summary>
    /// Replaces variations of newlines with \n to make splitting content into lines consistent.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private static string NormalizeContent(string content) {
        return content.Replace("\r\n", "\n").Replace("\n", "\n");
    }

    private void HandleEnter(ConsoleModifiers modifiers) {
        // TODO: Effectively insert line
    }

    private void HandleLeftArrow(ConsoleModifiers modifiers) {
        if (modifiers == ConsoleModifiers.Alt) {
            MoveCursorPrevWord();
        } else if (modifiers == ConsoleModifiers.Control) {
            MoveCursorToLineStart();
        } else {
            MoveCursorLeft();
        }
    }

    private void HandleRightArrow(ConsoleModifiers modifiers) {
        if (modifiers == ConsoleModifiers.Alt) {
            MoveCursorNextWord();
        } else if (modifiers == ConsoleModifiers.Control) {
            MoveCursorToLineEnd();
        } else {
            MoveCursorRight();
        }
    }

    private void HandleUpArrow(ConsoleModifiers modifiers) {

    }

    private void HandleDownArrow(ConsoleModifiers modifiers) {

    }

    private void HandlePageUp(ConsoleModifiers modifiers) {

    }

    private void HandlePageDown(ConsoleModifiers modifiers) {

    }

    private void HandleHome(ConsoleModifiers modifiers) {
        MoveCursorToLineStart();
    }

    private void HandleEnd(ConsoleModifiers modifiers) {
        MoveCursorToLineEnd();
    }
}
