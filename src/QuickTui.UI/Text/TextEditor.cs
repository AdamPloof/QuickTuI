namespace QuickTui.UI.Text;

/// <inheritdoc/>
public class TextEditor : ITextEditor {
    private TextContent _content;
    private TextPosition _cursorPos;

    public TextEditor() {
        _content = new();
        _cursorPos = TextPosition.Start;
    }

    /// <inheritdoc/>
    public void HandleCommand(EditorCommand command) {

    }
}
