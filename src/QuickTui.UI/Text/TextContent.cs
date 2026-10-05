using System.Linq;
using System.Collections.Generic;

using QuickTui.UI.Helpers;

namespace QuickTui.UI.Text;

/// <summary>
/// Container for the content lines of an ITextEditor. Translates logical lines into
/// text buffers.
/// </summary>
public class TextContent : ITextContent {
    public List<string> Lines;

    public TextContent() {
        Lines = [""];
    }

    public TextContent(string content) {
        Lines = NormalizeContent(content).Split('\n').ToList();
    }

    /// <inheritdoc/>
    public void Insert(string text, TextPosition position) {
        int line = Numeric.Clamp(position.Line, 0, Lines.Count);
        if (line > Lines.Count) {
            Lines.Add(text);
        } else {
            Lines[line] = Lines[line].Insert(
                Numeric.Clamp(position.Column, 0, Lines[line].Length),
                text
            );
        }
    }

    /// <inheritdoc/>
    public void Insert(char c, TextPosition position) {
        Insert(c.ToString(), position);
    }

    /// <inheritdoc/>
    public void Delete(int length, TextPosition position) {
        int line = Numeric.Clamp(position.Line, 0, Lines.Count);
        if (line > Lines.Count || length == 0) {
            return;
        }

        int col = Numeric.Clamp(position.Column, 0, Lines[line].Length - 1);
        int lengthInLine = Lines[line].Length - col;
        int lineDeleteLength = length > lengthInLine ? lengthInLine : length;

        Lines[line] = Lines[line].Remove(col, lineDeleteLength);
        length -= lineDeleteLength;

        Delete(length, new TextPosition(position.Line + 1, 0));
    }

    /// <inheritdoc/>
    public void Backspace(int length, TextPosition position) {
        int line = Numeric.Clamp(position.Line, 0, Lines.Count);
        if (line < 0 || length == 0) {
            return;
        }

        int col = Numeric.Clamp(position.Column, 0, Lines[line].Length);

        int startDeleteCol;
        int lineDeleteLength;
        if (length <= col) {
            startDeleteCol = col - length;
            lineDeleteLength = length;
        } else {
            startDeleteCol = 0;
            lineDeleteLength= col + 1;
        }

        Lines[line] = Lines[line].Remove(startDeleteCol, lineDeleteLength);
        length -= lineDeleteLength;

        int previousLine = position.Line - 1;
        if (previousLine < 0) {
            return;
        }

        Backspace(
            length,
            new TextPosition(
                previousLine,
                Lines[previousLine].Length - 1
            )
        );
    }

    /// <summary>
    /// Replaces variations of newlines with \n to make splitting content into lines consistent.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private static string NormalizeContent(string content) {
        return content.Replace("\r\n", "\n").Replace("\n", "\n");
    }
}
