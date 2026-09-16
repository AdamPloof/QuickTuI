using System;
using QuickTui.UI.Widgets;
using QuickTui.UI.Logging;

namespace QuickTui.UI.Events;

/// <inheritdoc />
public class EventDispatcher : IEventDispatcher {
    private ILogger _logger;

    public EventDispatcher(ILogger logger) {
        _logger = logger;
    }

    /// <summary>
    /// Forward the event to the widget's handler. Broadcast the event downward through
    /// the widget tree unless a handler stops propagation.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="uiEvent"></param>
    public void Dispatch(IWidget target, UiEvent uiEvent) {
        _logger.Debug(
            $"Dispatching event. Target: {target.GetType().Name}, event: {uiEvent.GetType().Name}"
        );

        target.HandleEvent(uiEvent);
        if (uiEvent.Handled) {
            return;
        }

        foreach (IWidget child in target.GetChildren()) {
            Dispatch(child, uiEvent);

            if (uiEvent.Handled) {
                return;
            }
        }
    }
}
