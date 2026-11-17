using QuickTui.UI.Events;

namespace QuickTui.UI.Text;

/// <summary>
/// ITextEditor is responsible for handling EditorCommands (e.g. move cursor),
/// tracking cursor position, and inserting/deleting text from TextContent. This is
/// the engine of TextBox and other editing widgets.
/// </summary>
public interface ITextEditor {
    /// <summary>
    /// Get visible content as a cell buffer.
    /// </summary>
    /// <returns></returns>
    public Cell[] ContentBuffer();

    /// <summary>
    /// Get the cursor position relative to the visibile content window.
    /// </summary>
    /// <returns></returns>
    public Cursor GetCursor();

    /// <summary>
    /// Handle an input event to update the state of the text editor.
    /// </summary>
    /// <remarks>TODO: eventually this should dispatch a EditorChangeEvent</remarks>
    /// <param name="uiEvent"></param>
    public void HandleInput(UiEvent uiEvent);
}
