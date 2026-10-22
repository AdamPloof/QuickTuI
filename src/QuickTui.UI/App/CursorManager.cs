using System;
using System.Drawing;

using QuickTui.UI;
using QuickTui.UI.Widgets;
using QuickTui.UI.Events;
using QuickTui.UI.Layout;
using QuickTui.UI.Logging;

namespace QuickTui.UI.App;

/// <inheritdoc />
public class CursorManager : ICursorManager {
    private IWidget? _cursorOwner;
    private int _screenWidth;
    private int _screenHeight;
    private ILogger _logger;

    public CursorManager(ILogger logger) {
        _cursorOwner = null;
        _screenWidth = 0;
        _screenHeight = 0;
        _logger = logger;
    }

    /// <inheritdoc />
    public void PlaceCursor() {
        if (_cursorOwner is null) {
            return;
        }

        Point screenPos = CalcScreenPosition(
            _cursorOwner.GetCursor(),
            _cursorOwner,
            _screenWidth,
            _screenHeight
        );

        // TODO: writing should be managed by a driver
        // TODO: use cursor style
        Console.Out.Write(Ansi.MoveCursorTo(screenPos.Y, screenPos.X));
    }

    /// <inheritdoc /> 
    public Point CalcScreenPosition(
        Cursor cursor,
        IWidget cursorOwner,
        int screenWidth,
        int screenHeight
    ) {
        Rect bounds = cursorOwner.BoundingBox;

        return new Point() {
            X = bounds.X + cursor.Position.X,
            Y = bounds.Y + cursor.Position.Y
        };
    }

    /// <inheritdoc />    
    public void HandleFocusChanged(FocusChangedEvent focusChanged) {
        // TODO: the cursor owner isn't necessarily the focused widget.
        // Need to figure out whether a cursor owns the widget.
        // Or maybe the focused widget does always own the cursor, but just
        // sometimes it's not visible (e.g. buttons)
        _cursorOwner = focusChanged.Focused;
        _logger.Debug($"Cursor owner changed to {focusChanged.Focused.GetType().Name}");
    }

    public void HandleResize(ResizeEvent resizeEvent) {
        _screenWidth = resizeEvent.Width;
        _screenHeight = resizeEvent.Height;
    }
}
