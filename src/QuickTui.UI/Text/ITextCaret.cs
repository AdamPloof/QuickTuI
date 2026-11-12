namespace QuickTui.UI.Text;

/// <summary>
/// ITextCaret is responsible for managaing the cursor position within a TextEditor's content. 
/// </summary>
public interface ITextCaret {
    /// <summary>
    /// Move caret to arbitrary position. If the position is outside of the
    /// content range, it is clamped (e.g. end of line, last line, etc.)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="content"></param>
    public void MoveTo(TextPosition position, ITextContent content);

    /// <summary>
    /// Move one character left
    /// </summary>
    /// <param name="content"></param>
    public void MoveLeft(ITextContent content);

    /// <summary>
    /// Move one character right
    /// </summary>
    /// <param name="content"></param>
    public void MoveRight(ITextContent content);

    /// <summary>
    /// Move up one line, preferred column or fallback to line end.
    /// </summary>
    /// <param name="content"></param>
    public void MoveUp(ITextContent content);

    /// <summary>
    /// Move down one line, preferred column or fallback to line end.
    /// </summary>
    /// <param name="content"></param>
    public void MoveDown(ITextContent content);

    /// <summary>
    /// Move on number of lines visible on page up from current position
    /// </summary>
    /// <param name="content"></param>
    public void MovePageUp(ITextContent content);

    /// <summary>
    /// Move on number of lines visible on page down from current position
    /// </summary>
    /// <param name="content"></param>
    public void MovePageDown(ITextContent content);

    /// <summary>
    /// Move to end of current word or end of next word if already at end of word.
    /// </summary>
    /// <param name="content"></param>
    public void MoveNextWord(ITextContent content);

    /// <summary>
    /// Move to start of current word or start of previous word if already at start of word.
    /// </summary>
    /// <param name="content"></param>
    public void MovePreviousWord(ITextContent content);

    /// <summary>
    /// Move to the first column of the current line
    /// </summary>
    /// <param name="content"></param>
    public void MoveToLineStart(ITextContent content);

    /// <summary>
    /// Move to the last column of the current line
    /// </summary>
    /// <param name="content"></param>
    public void MoveToLineEnd(ITextContent content);

    /// <summary>
    /// Move to the first line, first column
    /// </summary>
    /// <param name="content"></param>
    public void MoveToStart(ITextContent content);

    /// <summary>
    /// Move to the last line, last column
    /// </summary>
    /// <param name="content"></param>
    public void MoveToEnd(ITextContent content);
}
