# Text Editor Refactor TODOs

## Goal

Move text storage, editing behavior, and viewport bookkeeping out of `TextBox`. Keep `TextBox` focused on its role as a widget: receiving UI events, painting visible cells, managing focus and dirty state, and exposing a widget-local cursor.

The MVP uses logical lines with vertical and horizontal scrolling and no soft wrapping.

## Component overview

```text
TextInputEvent / KeyEvent -> TextBox
                              |-- commands ----------> TextEditor -> TextContent
                              |-- view coordination -> TextViewport
                              |-- paints visible content
                              `-- exposes cursor ----> CursorManager
```

### `TextContent`

Owns the logical line buffer and the rules for changing text. It is responsible for maintaining valid content, normalizing line endings, and performing insertion and deletion. It should provide the editor and renderer with controlled access to the content without taking on cursor, viewport, widget, or terminal responsibilities.

### `TextEditor`

Owns the logical cursor position and interprets `EditorCommand` values. It coordinates navigation and editing operations with `TextContent`, ensuring the cursor remains valid after every command. It should not depend on widget geometry or terminal coordinates.

### `TextViewport`

Tracks the portion of the logical content that is visible within the widget. It uses the content-area dimensions and logical cursor position to manage vertical and horizontal offsets. It also provides the mapping from a logical text position to a position within the visible content area.

This component has not been started. Its exact API can be discovered while integrating scrolling into `TextBox`.

### `TextBox`

Translates UI input into editor commands, passes those commands to `TextEditor`, and reflects resulting state changes in the viewport, rendered cells, dirty state, and displayed cursor. It owns presentation concerns such as style, border, padding, focus, and read-only behavior.

### `CursorManager`

Continues to manage the physical terminal cursor. It should receive a widget-local cursor position from `TextBox` and remain unaware of logical text positions, line content, and viewport offsets.

## Current progress

- [x] Add a zero-based `TextPosition` value type with named `Line` and `Column` properties.
- [x] Add `ITextContent` and `TextContent` as the beginning of a dedicated content layer.
- [x] Store content as logical lines and represent initially empty content with one empty line.
- [x] Implement and test initial same-line insertion, forward deletion, and backward deletion.
- [x] Define the initial `EditorCommand` vocabulary for text insertion, deletion, navigation, line breaks, paging, and insert mode.
- [x] Add `ITextEditor` and the initial `TextEditor` structure.
- [x] Establish `TextEditor` as the owner of `TextContent` and the logical cursor position.
- [ ] Connect command handling, content mutation, and cursor movement inside `TextEditor`.
- [ ] Replace the editing logic still held by `TextBox` with the new components.

## Main implementation work

### 1. Complete the core `TextContent` behavior

- [ ] Finish content normalization and multiline editing behavior.
- [ ] Define consistent behavior for insertion and deletion at line and document boundaries.
- [ ] Support the line splitting and joining needed by Enter, Backspace, and Delete.
- [ ] Provide the content access needed by `TextEditor` and `TextBox` while keeping ownership of the line buffer in `TextContent`.
- [ ] Expand tests around multiline changes, empty content, and boundary behavior.

### 2. Implement `TextEditor` command handling

- [ ] Handle the MVP subset of `EditorCommand` values.
- [ ] Update the logical cursor after edits and navigation.
- [ ] Support movement within a line and across line boundaries.
- [ ] Preserve the intended column during vertical movement across lines of different lengths.
- [ ] Ensure the cursor remains valid whenever content changes.
- [ ] Expose enough editor state for `TextBox` to render content and react to changes without exposing widget concerns to the editor.
- [ ] Add tests for command handling, cursor movement, edits, and boundary conditions.

Commands already represented in the type hierarchy do not all need to be implemented in the first pass. Paging, selection extension, and insert mode can remain unsupported until their behavior is needed.

### 3. Add viewport management

- [ ] Add viewport state for the visible line and column offsets and the available content dimensions.
- [ ] Keep the viewport valid as content and widget dimensions change.
- [ ] Scroll only as needed to keep the logical cursor visible.
- [ ] Map the logical cursor position to a position within the visible content area.
- [ ] Add focused tests for vertical and horizontal overflow, resizing, and viewport boundaries.

### 4. Integrate the editor with `TextBox`

- [ ] Give `TextBox` a `TextEditor` and viewport instead of maintaining its own line buffer and logical cursor.
- [ ] Translate `TextInputEvent` and supported `KeyEvent` values into `EditorCommand` values.
- [ ] Keep read-only policy, focus behavior, and event-handled state in the widget layer.
- [ ] Update the viewport and widget dirty state after commands change editor state.
- [ ] Preserve the existing public content API where practical, delegating it to the new content/editor layer.
- [ ] Add integration tests covering input events, editor commands, visible content, and dirty state.

### 5. Update painting and cursor placement

- [ ] Paint the visible slice of `TextContent` without soft wrapping.
- [ ] Keep painting constrained to `ContentArea()` so border and padding remain presentation concerns.
- [ ] Convert the editor's logical cursor through the viewport into a widget-local cursor position.
- [ ] Ensure focus, scrolling, border, padding, and empty content areas produce the expected cursor visibility and position.
- [ ] Keep `CursorManager` limited to translating the widget-local cursor to terminal coordinates.

### 6. Verify the integrated behavior

- [ ] Exercise multiline editing and overflow in the demo.
- [ ] Manually verify editing, navigation, scrolling, focus changes, cursor placement, and terminal resizing.
- [ ] Keep unit tests close to their owning component: content mutations in `TextContentTests`, command and cursor behavior in `TextEditorTests`, and rendering/input integration in `TextBoxTests`.
- [ ] Run the full test suite and build after integration.

## Deferred work

These features are outside the initial refactor unless the implementation reveals an immediate need:

- Selection, clipboard integration, mouse input, and undo/redo
- Insert/overwrite behavior, word movement, and page movement
- Soft wrapping and a width-aware text-layout component
- Full Unicode grapheme and terminal cell-width handling
- Rich text or syntax styling
- Configurable key bindings
- Broader changes to application event dispatch
- A generalized cursor-provider or terminal-driver abstraction
