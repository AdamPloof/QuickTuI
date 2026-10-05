# Text Editor Refactor TODOs

## Goal

Move text editing and viewport bookkeeping out of `TextBox` while keeping the first implementation small. `TextBox` should remain responsible for adapting UI events, painting cells, managing focus, and exposing a widget-local cursor.

The MVP uses logical lines, vertical and horizontal scrolling, and no soft wrapping.

## 1. Establish the behavior to preserve

- [ ] Treat document positions as zero-based line and column values.
- [ ] Allow the caret column to equal the line length so it can represent the insertion point after the final character.
- [ ] Keep logical document positions distinct from widget-local cursor positions.
- [ ] Represent empty content consistently as one empty logical line.
- [ ] Continue treating one UTF-16 `char` as one terminal cell for the MVP.
- [ ] Add focused tests for any existing `TextBox` behavior that must survive the refactor before moving it.

## 2. Extract the editing state

- [ ] Add an internal `TextEditorState` that owns the logical lines and logical caret position.
- [ ] Add a small `TextPosition` value type with named `Line` and `Column` properties instead of using `Point` for document coordinates.
- [ ] Move content normalization and caret validation out of `TextBox`.
- [ ] Keep at least one logical line after construction, content replacement, and deletion.
- [ ] Move text insertion into `TextEditorState`, including insertion of text containing newlines.
- [ ] Move Enter behavior into `TextEditorState` by splitting the current line.
- [ ] Move Backspace behavior into `TextEditorState`, including joining with the previous line at column zero.
- [ ] Move Delete behavior into `TextEditorState`, including joining with the next line at line end.
- [ ] Move left, right, up, down, Home, End, document-start, and document-end navigation into `TextEditorState`.
- [ ] Preserve the intended column when moving vertically across lines of unequal length.
- [ ] Ensure every edit leaves the caret at a valid position.
- [ ] Decide and test whether replacing all content resets the caret or clamps it to the new content.
- [ ] Expose enough change information for `TextBox` to know when visible content or the caret changed; keep the initial mechanism simple.
- [ ] Add `TextEditorStateTests` for newline normalization, insertion, line splitting and joining, deletion boundaries, movement boundaries, cross-line movement, preferred-column behavior, empty content, and content replacement.

## 3. Extract the viewport state

- [ ] Add an internal `TextViewport` that tracks the first visible logical line, first visible column, width, and height.
- [ ] Update viewport dimensions from the `TextBox` content area.
- [ ] Keep viewport offsets valid when content changes or the widget is resized.
- [ ] Add behavior that scrolls only as much as necessary to keep the caret visible.
- [ ] Handle the caret at the insertion point immediately after the last visible character.
- [ ] Provide the minimal coordinate conversion needed to map the logical caret into the visible content area.
- [ ] Add `TextViewportTests` for each edge, long lines, short documents, empty dimensions, resizing while scrolled, and content shrinkage.

## 4. Route input through `TextEditorState`

- [ ] Keep `InputParser` responsible for producing `TextInputEvent` and `KeyEvent`; do not add editor behavior to it.
- [ ] Have `TextBox` pass `TextInputEvent.Text` to the editing state as a single insertion operation so multi-character input remains possible.
- [ ] Have `TextBox` translate supported `KeyEvent` values into direct editing-state operations.
- [ ] Use modifier flag checks where modifiers affect movement.
- [ ] After an edit or caret movement, ask the viewport to keep the caret visible and update the widget-local cursor.
- [ ] Mark a supported input event handled even when the caret is already at a boundary and no state changes.
- [ ] Leave unsupported keys unhandled.
- [ ] Keep read-only handling in `TextBox`: allow supported navigation if read-only text boxes remain focusable, but do not invoke mutation operations.
- [ ] Add tests for text input, supported navigation and editing keys, boundary keys, unsupported keys, modifiers, and read-only behavior.

## 5. Refactor painting

- [ ] Replace the line, logical-caret, and viewport fields in `TextBox` with the extracted state objects.
- [ ] Stop using the wrapping behavior of `TextBuffer.FromLines` for editable text.
- [ ] Paint one logical line per content-area row.
- [ ] Slice each visible line at the viewport's first visible column and clip it to the content-area width.
- [ ] Paint only inside `ContentArea()`, preserving border and padding cells.
- [ ] Mark the widget dirty when an edit, style change, resize, or viewport movement changes visible cells.
- [ ] Add `TextBoxTests` for clipping without wrapping, horizontal and vertical scrolling, resizing, borders, padding, empty content areas, content replacement, and dirty state.

## 6. Synchronize the terminal cursor

- [ ] Convert the logical caret into a viewport-relative position after caret or viewport changes.
- [ ] Add the content-area offset to produce the widget-local `Cursor.Position` exposed by `TextBox`.
- [ ] Show the cursor only while `TextBox` has focus and the calculated position is inside its drawable content area.
- [ ] Keep `CursorManager` unaware of logical lines, editor state, and viewport offsets.
- [ ] Ensure the content-area offset and widget bounding-box offset are each applied exactly once.
- [ ] Add tests for cursor placement with scrolling, border, padding, focus changes, and an empty content area.

## 7. Verify the integrated editor

- [ ] Add or update the demo so multiline editing and overflow can be exercised interactively.
- [ ] Manually verify typing, multiline input, Enter, Backspace, Delete, navigation, horizontal scrolling, vertical scrolling, focus changes, and terminal resizing.
- [ ] Manually verify cursor placement in a text box with both border and padding.
- [ ] Run `dotnet test QuickTui.sln -nodeReuse:false -maxcpucount:1` after each extraction step.
- [ ] Run `dotnet build QuickTui.sln -nodeReuse:false -maxcpucount:1` after integration.
- [ ] Record legitimate failing tests and their causes instead of weakening assertions.

## Not part of the MVP

Defer these until current behavior demonstrates a need for them:

- A separate `TextDocument` abstraction. For now, `TextEditorState` can own the logical lines.
- An editor-command class hierarchy or configurable key map. Direct calls from `TextBox` to `TextEditorState` are sufficient initially.
- Selection, clipboard integration, mouse hit testing, and undo/redo.
- Insert/overwrite mode, word movement, and page movement.
- Soft wrapping and a separate text-layout abstraction.
- Full Unicode grapheme and terminal cell-width handling.
- Rich text or syntax styling.
- A general cursor-provider abstraction.
- Changes to the application-wide event-dispatch model unless the editor exposes a concrete routing problem.
- A terminal-driver abstraction.
