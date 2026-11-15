using System;
using System.Diagnostics;

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
}
