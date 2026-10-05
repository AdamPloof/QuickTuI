namespace QuickTui.UI.Text;

/// <summary>
/// ITextContent is responsible for owning the content of a widget, handling
/// changes to the text, and returning the content as a text buffer.
/// </summary>
/// <remarks>
/// TODO: handle insert mode (overwrite)
/// </remarks>
public interface ITextContent {
    /// <summary>
    /// Insert text at the current cursor position.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="position"></param>
    public void Insert(string text, TextPosition position);

    /// <summary>
    /// Insert a single character at the current cursor position.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="position"></param>
    public void Insert(char c, TextPosition position);

    /// <summary>
    /// Delete text in front of the current cursor position.
    /// </summary>
    /// <param name="length"></param>
    /// <param name="position"></param>
    public void Delete(int length, TextPosition position);

    /// <summary>
    /// Delete text behind the current cursor position.
    /// </summary>
    /// <param name="length"></param>
    /// <param name="position"></param>
    public void Backspace(int length, TextPosition position);
}
