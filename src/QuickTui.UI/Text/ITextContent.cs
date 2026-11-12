using System.Collections.Generic;

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
    /// Returns the char at a given position. If the postion is not within the content
    /// throw an ArgumentOutOfRangeException.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public char CharAt(TextPosition position);

    /// <summary>
    /// Returns the number of lines in the content.
    /// </summary>
    /// <returns></returns>
    public int LineCount();

    /// <summary>
    /// Returns the length of a given line. If the line number is out of
    /// range of the content, throws ArgumentOutOfRangeException.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public int LineLength(int line);

    /// <summary>
    /// Insert text at the current cursor position.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="position"></param>
    /// <returns>The new text position after the edit</returns>
    public TextPosition Insert(string text, TextPosition position);

    /// <summary>
    /// Insert a single character at the current cursor position.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="position"></param>
    /// <returns>The new text position after the edit</returns>
    public TextPosition Insert(char c, TextPosition position);

    /// <summary>
    /// Delete text in front of the current cursor position.
    /// </summary>
    /// <param name="length"></param>
    /// <param name="position"></param>
    /// <returns>The new text position after the edit</returns>
    public TextPosition Delete(int length, TextPosition position);

    /// <summary>
    /// Delete text behind the current cursor position.
    /// </summary>
    /// <param name="length"></param>
    /// <param name="position"></param>
    /// <returns>The new text position after the edit</returns>
    public TextPosition Backspace(int length, TextPosition position);

    /// <summary>
    /// Returns a new TextPosition a distance from the provided position. Used
    /// to simplify calculating new positions using only number of characters.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public TextPosition PositionDelta(TextPosition position, int delta);

    /// <summary>
    /// Join the lines into a string.
    /// </summary>
    /// <returns></returns>
    public string ToString();
}
