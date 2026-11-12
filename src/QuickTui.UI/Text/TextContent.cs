using System;
using System.Linq;
using System.Collections.Generic;

using QuickTui.UI.Helpers;
using System.Diagnostics;

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

    public TextContent(List<string> lines) {
        Lines = lines;
    }

    /// <inheritdoc/>
    public List<string> GetLines() {
        return Lines;
    }

    /// <inheritdoc/>
    public char CharAt(TextPosition position) {
        if (!PositionIsValid(position)) {
            throw new ArgumentOutOfRangeException(
                $"Invalid position. Line {position.Line}, Column: {position.Column}"
            );
        }

        return Lines[position.Line][position.Column];
    }

    /// <inheritdoc/>
    public int LineCount() {
        return Lines.Count;
    }

    /// <inheritdoc/>
    public int LineLength(int line) {
        if (line < 0 || line >= Lines.Count) {
            throw new ArgumentOutOfRangeException(
                $"Line {line} is out of range. Line ount: {Lines.Count}"
            );
        }

        return Lines[line].Length;
    }

    /// <inheritdoc/>
    public TextPosition Insert(string text, TextPosition position) {
        int line = Numeric.Clamp(position.Line, 0, Lines.Count);
        if (line > Lines.Count) {
            Lines.Add(text);
            return new TextPosition(Lines.Count - 1, text.Length);
        }

        Lines[line] = Lines[line].Insert(
            Numeric.Clamp(position.Column, 0, Lines[line].Length),
            text
        );

        return new TextPosition(line, position.Column + text.Length);
    }

    /// <inheritdoc/>
    public TextPosition Insert(char c, TextPosition position) {
        return Insert(c.ToString(), position);
    }

    /// <inheritdoc/>
    public TextPosition Delete(int length, TextPosition position) {
        int line = Numeric.Clamp(position.Line, 0, Lines.Count);
        if (line > Lines.Count || length == 0) {
            // Nothing to delete at this position/length
            return position;
        }

        int col = Numeric.Clamp(position.Column, 0, Lines[line].Length);
        int distanceToLineEnd = Lines[line].Length - col;
        int lineDeleteLength = length > distanceToLineEnd ? distanceToLineEnd : length;

        Lines[line] = Lines[line].Remove(col, lineDeleteLength);
        length -= lineDeleteLength;

        if (length == 0 || line >= Lines.Count - 1) {
            // Nothing left to delete
            return position;
        }

        TextPosition nextPos;
        if (Lines[line].Length == 0) {
            // Full line has been deleted
            Lines.RemoveAt(line);
            length -= 1;
            nextPos = new TextPosition(position.Line, 0);
        } else {
            Debug.Assert(position.Column == Lines[line].Length, "Expected cursor to be at line end");
            // Cursor at end with more to delete, merge lines
            Lines[line] += Lines[line + 1];
            Lines.RemoveAt(line + 1);
            length -= 1;
            nextPos = position;
        }

        return Delete(length, nextPos);
    }

    /// <inheritdoc/>
    public TextPosition Backspace(int length, TextPosition position) {
        int line = Numeric.Clamp(position.Line, 0, Lines.Count);
        if (line < 0 || length == 0) {
            return position;
        }

        int col = Numeric.Clamp(position.Column, 0, Lines[line].Length);
        int maxDeleteLength = col;
        for (int i = 0; i < line; i++) {
            maxDeleteLength += Lines[line].Length;
        }

        int deleteLength = length > maxDeleteLength ? maxDeleteLength : length;

        return Delete(
            deleteLength,
            PositionDelta(position, deleteLength * -1)
        );
    }

    /// <inheritdoc/>
    public TextPosition PositionDelta(TextPosition position, int delta) {
        if (position.Line > Lines.Count - 1) {
            throw new ArgumentOutOfRangeException(
                $"Unable to calculate position: Line {position.Line} is past last line"
            );
        }

        if (position.Column > Lines[position.Line].Length) {
            string err = $"""
                Unable to calculate position: Column ({position.Column}) greater
                than line {position.Line} length ({Lines[position.Line].Length})
                """;
            throw new ArgumentOutOfRangeException(err);
        }

        if (delta == 0) {
            return position;
        }

        if (position.Column + delta >= 0 && position.Column + delta < Lines[position.Line].Length) {
            // Complete the move on the same line
            return new TextPosition(position.Line, position.Column + delta);
        }

        if (delta < 0) {
            if (position.Line == 0) {
                return new TextPosition(0, 0);
            }

            int deltaFromPreviousLineEnd = delta + position.Column + 1;
            TextPosition previousLineEnd = new TextPosition(
                position.Line - 1,
                Lines[position.Line - 1].Length
            );

            return PositionDelta(previousLineEnd, deltaFromPreviousLineEnd);
        } else {
            if (position.Line == Lines.Count - 1) {
                return new TextPosition(position.Line, Lines[position.Line].Length);
            }

            int deltaFromNextLineStart = delta - (Lines[position.Line].Length - position.Column) - 1;
            TextPosition nextLineStart = new TextPosition(position.Line + 1, 0);

            return PositionDelta(nextLineStart, deltaFromNextLineStart);
        }
    }

    /// <inheritdoc/>
    public override string ToString() {
        return String.Join('\n', Lines);
    }

    /// <summary>
    /// Replaces variations of newlines with \n to make splitting content into lines consistent.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private static string NormalizeContent(string content) {
        return content.Replace("\r\n", "\n").Replace("\n", "\n");
    }

    /// <summary>
    /// Returns true if the position is within the content.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool PositionIsValid(TextPosition position) {
        if (
            position.Line >= Lines.Count
            || position.Line < 0
            || position.Column > Lines[position.Line].Length
            || position.Column < 0
        ) {
            return false;
        }

        return true;
    }
}
