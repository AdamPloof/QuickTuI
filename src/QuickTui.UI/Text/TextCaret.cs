using System;
using System.Collections.Generic;

namespace QuickTui.UI.Text;

// TODO: Cache line count and line lengths to make lookup faster,
//       mark caret dirty when text content changes.
public class TextCaret : ITextCaret {
    /// <summary>
    /// A helper for finding the boundry between words. Used for
    /// moving to prev/next word.
    /// </summary>
    protected static class WordBoundryScanner {
        /// <summary>
        /// _states is a map of states to trigger/action pairs. Triggers are a boolean
        /// indicating whether the current value in the content is a whitespace character.
        /// </summary>
        private static Dictionary<string, Dictionary<bool, string>> _states = new() {
            ["start"] = new() {
                [true] = "continue",
                [false] = "enterWord"
            },
            ["inWord"] = new() {
                [true] = "end",
                [false] = "continue"
            },
        };

        public static TextPosition Next(ITextContent content, TextPosition position) {
            int lastLine = content.LineCount() - 1;
            int lastCol = content.LineLength(lastLine);
            if (position.Line == lastLine && position.Column == lastCol) {
                return position;
            }

            TextPosition currentPos = position;
            if (currentPos.Column == content.LineLength(currentPos.Line)) {
                // end of line
                currentPos = new TextPosition(currentPos.Line + 1, 0);
            } else {
                currentPos = new TextPosition(currentPos.Line, currentPos.Column + 1);
            }

            Dictionary<bool, string> state = _states["start"];
            while (true) {
                char current = content.CharAt(currentPos);
                bool trigger = char.IsWhiteSpace(current);
                string action = state[trigger];
                switch (action) {
                    case "end":
                        return currentPos;
                    case "enterWord":
                        state = _states["inWord"];
                        break;
                }

                if (currentPos.Column == content.LineLength(currentPos.Line)) {
                    // end of line
                    if (currentPos.Line == lastLine) {
                        // end of content
                        return currentPos;
                    }

                    currentPos = new TextPosition(currentPos.Line + 1, 0);
                } else {
                    currentPos = new TextPosition(currentPos.Line, currentPos.Column + 1);
                }
            }
        }

        public static TextPosition Previous(ITextContent content, TextPosition position) {
            if (position == new TextPosition(0, 0)) {
                return position;
            }

            TextPosition currentPos = position;
            if (currentPos.Column == 0) {
                // start of line
                int lastCol = content.LineLength(currentPos.Line - 1) - 1;
                currentPos = new TextPosition(currentPos.Line - 1, lastCol);
            } else {
                currentPos = new TextPosition(currentPos.Line, currentPos.Column - 1);
            }

            Dictionary<bool, string> state = _states["start"];
            while (true) {
                char current = content.CharAt(currentPos);
                bool trigger = char.IsWhiteSpace(current);
                string action = state[trigger];
                switch (action) {
                    case "end":
                        // Previous word boundry is ahead of the space.
                        return new TextPosition(currentPos.Line, currentPos.Column + 1);
                    case "enterWord":
                        state = _states["inWord"];
                        break;
                }

                if (currentPos.Column == 0) {
                    // start of line
                    return currentPos;
                } else {
                    currentPos = new TextPosition(currentPos.Line, currentPos.Column - 1);
                }
            }
        }
    }

    /// <summary>
    /// The current position within a TextContent
    /// </summary>
    public TextPosition Position { get; private set; }

    private int _requestedColumn;

    public TextCaret(TextPosition startPosition = new()) {
        Position = startPosition;
        _requestedColumn = startPosition.Column;
    }

    /// <inheritdoc/>
    public void MoveTo(TextPosition position, ITextContent content) {
        if (position.Line >= content.LineCount()) {
            MoveToEnd(content);
            return;
        }

        if (position.Line < 0) {
            MoveToStart(content);
            return;
        }

        int lastCol = content.LineLength(position.Line);
        if (position.Column > lastCol) {
            Position = new TextPosition(position.Line, lastCol);
            _requestedColumn = Position.Column;
        } else if (position.Column < 0) {
            Position = new TextPosition(position.Line, 0);
            _requestedColumn = Position.Column;
        } else {
            Position = position;
            _requestedColumn = Position.Column;
        }
    }

    /// <inheritdoc/>
    public void MoveLeft(ITextContent content) {
        Position = content.PositionDelta(Position, -1);
        _requestedColumn = Position.Column;
    }

    /// <inheritdoc/>
    public void MoveRight(ITextContent content) {
        Position = content.PositionDelta(Position, 1);
        _requestedColumn = Position.Column;
    }

    /// <inheritdoc/>
    public void MoveUp(ITextContent content) {
        int nextLine = Position.Line + 1;
        if (nextLine < content.LineCount()) {
            int nextLineLength = content.LineLength(nextLine);
            Position = new TextPosition(
                nextLine,
                _requestedColumn <= nextLineLength ? _requestedColumn : nextLineLength
            );
        }
    }

    /// <inheritdoc/>
    public void MoveDown(ITextContent content) {
        int prevLine = Position.Line - 1;
        if (prevLine >= 0) {
            int prevLineLength = content.LineLength(prevLine);
            Position = new TextPosition(
                prevLine,
                _requestedColumn <= prevLineLength ? _requestedColumn : prevLineLength
            );
        }
    }

    /// <inheritdoc/>
    public void MovePageUp(ITextContent content) {
        // TODO: implement me once we have content window
        MoveUp(content);
    }

    /// <inheritdoc/>
    public void MovePageDown(ITextContent content) {
        // TODO: implement me once we have content window
        MoveDown(content);
    }

    /// <inheritdoc/>
    public void MoveNextWord(ITextContent content) {
        Position = WordBoundryScanner.Next(content, Position);
        _requestedColumn = Position.Column;
    }

    /// <inheritdoc/>
    public void MovePreviousWord(ITextContent content) {
        Position = WordBoundryScanner.Previous(content, Position);
        _requestedColumn = Position.Column;
    }

    /// <inheritdoc/>
    public void MoveToLineStart(ITextContent content) {
        Position = new TextPosition(
            Position.Line,
            0
        );
        _requestedColumn = 0;
    }

    /// <inheritdoc/>
    public void MoveToLineEnd(ITextContent content) {
        int lastCol = content.LineLength(Position.Line);
        Position = new TextPosition(
            Position.Line,
            lastCol
        );
        _requestedColumn = lastCol;
    }

    /// <inheritdoc/>
    public void MoveToStart(ITextContent _) {
        Position = new TextPosition(0, 0);
        _requestedColumn = 0;
    }

    /// <inheritdoc/>
    public void MoveToEnd(ITextContent content) {
        int lastLine = content.LineCount() - 1;
        int lastCol = content.LineLength(lastLine);
        Position = new TextPosition(lastLine, lastCol);
        _requestedColumn = lastCol;
    }
}
