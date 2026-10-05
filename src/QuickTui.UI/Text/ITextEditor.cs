namespace QuickTui.UI.Text;

/// <summary>
/// ITextEditor is responsible for handling EditorCommands (e.g. move cursor),
/// tracking cursor position, and inserting/deleting text from TextContent. This is
/// the engine of TextBox and other editing widgets.
/// </summary>
public interface ITextEditor {
    /// <summary>
    /// Handle an editor command by updating the state of the text editor.
    /// </summary>
    /// <remarks>TODO: eventually this should dispatch a EditorChangeEvent</remarks>
    /// <param name="command"></param>
    public void HandleCommand(EditorCommand command);
}
