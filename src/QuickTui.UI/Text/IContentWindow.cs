namespace QuickTui.UI.Text;

/// <summary>
/// IContentWindow is responseible for presenting the visible slice of
/// TextContent based on the dimensions available to an ITextEditor.
/// </summary>
public interface IContentWindow {
    /// <summary>
    /// Get visible content as a cell buffer.
    /// </summary>
    /// <returns></returns>
    public Cell[] ContentBuffer(ITextContent content);

    /// <summary>
    /// Get the caret position relative to its location in the IContentWindow.
    /// </summary>
    /// <returns></returns>
    public TextPosition GetRelativePosition(ITextCaret caret);

    /// <summary>
    /// Update the window's position in the content when the caret's position
    /// is updated.
    /// </summary>
    /// <param name="caret"></param>
    /// <param name="content"></param>
    public void HandleCaretMove(ITextCaret caret, ITextContent content);

    /// <summary>
    /// Update the dimensions of the window on resizing terminal window or the widget.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void HandleResize(int width, int height);
}
