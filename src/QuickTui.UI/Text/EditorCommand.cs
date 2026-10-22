namespace QuickTui.UI.Text;

/// <summary>
/// Base EditorCommand
/// </summary>
public abstract record EditorCommand;

/// <summary>
/// Add text to the editor's content
/// </summary>
/// <param name="Text"></param>
public sealed record InsertText(string Text) : EditorCommand;

/// <summary>
/// Delete backward
/// </summary>
public sealed record Backspace : EditorCommand;

/// <summary>
/// Delete forward
/// </summary>
public sealed record Delete : EditorCommand;

/// <summary>
/// Return/enter key. Adds a line break to the editor's content
/// </summary>
public sealed record InsertLineBreak : EditorCommand;

/// <summary>
/// Move cursor to the left
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MoveLeft(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor to the right
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MoveRight(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor down a line
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MoveDown(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor up a line
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MoveUp(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor to start of line
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MoveToLineStart(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor to end of line
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MoveToLineEnd(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor by page up
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MovePageUp(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Move cursor by page down
/// </summary>
/// <param name="ExtendSelection"></param>
public sealed record MovePageDown(bool ExtendSelection = false) : EditorCommand;

/// <summary>
/// Toggle the insert mode of the editor
/// </summary>
public sealed record ToggleInsertMode : EditorCommand;
