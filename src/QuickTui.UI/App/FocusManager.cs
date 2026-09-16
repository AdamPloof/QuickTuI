using System;
using System.Linq;
using System.Collections.Generic;

using QuickTui.UI.Widgets;
using QuickTui.UI.Events;
using QuickTui.UI.Logging;

namespace QuickTui.UI.App;

/// <inheritdoc />
public class FocusManager : IFocusManager {
    private IWidget _root;
    private IWidget _focused;
    private List<Action<FocusChangedEvent>> _listeners;
    private ILogger _logger;

    public FocusManager(IWidget root, ILogger logger) {
        if (root.Parent is not null) {
            throw new ArgumentException("Root widget must have a null parent");
        }

        if (!root.AcceptsFocus) {
            throw new ArgumentException("Root widget must accept focus");
        }

        _root = root;
        _focused = root;
        _listeners = [];
        _logger = logger;
    }

    /// <inheritdoc />
    public IWidget GetFocusedWidget() {
        return _focused;
    }

    /// <inheritdoc />
    public bool ChangeFocus(IWidget widget) {
        if (!widget.AcceptsFocus) {
            return false;
        }

        if (!_focused.ClearFocus()) {
            // focused widget refused to give up focus.
            return false;
        }

        if (!widget.SetFocus()) {
            return false;
        }

        IWidget prevFocus = _focused;
        _focused = widget;
        _logger.Debug($"Focused widget changed: {_focused.GetType().Name}");

        if (_listeners.Count > 0) {
            FocusChangedEvent focusChanged = new FocusChangedEvent(prevFocus, _focused);
            foreach (Action<FocusChangedEvent> action in _listeners) {
                action(focusChanged);
            }
        }

        return true;
    }

    /// <inheritdoc />
    public bool FocusedWidgetCapturesTab() {
        return _focused.CaptureTabKey();
    }

    /// <inheritdoc />
    public void FocusNext() {
        IWidget start = _focused;
        IWidget current = start;
        while (true) {
            current = GetNextNode(current);
            if (current.AcceptsFocus && ChangeFocus(current)) {
                return;
            }

            if (current == start) {
                return;
            }
        }
    }

    public void RegisterListener(Action<FocusChangedEvent> action) {
        _listeners.Add(action);
    }

    /// <inheritdoc />
    public void FocusPrevious() {
        if (_focused == _root) {
            IWidget last = GetDeepestLastDescendent(_root);
            while (last != _root) {
                if (last.AcceptsFocus && ChangeFocus(last)) {
                    return;
                }

                last = GetPreviousNode(last);
            }

            return;
        }

        IWidget current = _focused;
        while (true) {
            current = GetPreviousNode(current);
            if (current == _root) {
                // Root must always accept focus.
                ChangeFocus(_root);
                return;
            }

            if (current.AcceptsFocus && ChangeFocus(current)) {
                return;
            }
        }
    }

    /// <summary>
    /// To get the next sibling, we iterate through the children of
    /// the current node's parent until we find the current node, then take the
    /// next child.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static IWidget? GetNextSibling(IWidget node) {
        IWidget? parent = node.Parent;
        if (parent is null) {
            return null;
        }

        IWidget[] siblings = parent.GetChildren().ToArray();
        for (int i = 0; i < siblings.Length - 1; i++) {
            if (siblings[i].GetId() == node.GetId()) {
                return siblings[i + 1];
            }
        }

        return null;
    }

    /// <summary>
    /// To get the previous sibling, we iterate through the children of the
    /// current node's parent until we find the current node, then take the
    /// previous child.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static IWidget? GetPreviousSibling(IWidget node) {
        IWidget? parent = node.Parent;
        if (parent is null) {
            return null;
        }

        IWidget[] siblings = parent.GetChildren().ToArray();
        for (int i = 1; i < siblings.Length; i++) {
            if (siblings[i].GetId() == node.GetId()) {
                return siblings[i - 1];
            }
        }

        return null;
    }

    /// <summary>
    /// Get the last leaf node of the current node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static IWidget GetDeepestLastDescendent(IWidget node) {
        IWidget current = node;
        while (true) {
            IWidget? lastChild = current.GetChildren().LastOrDefault();
            if (lastChild is null) {
                return current;
            }

            current = lastChild;
        }
    }

    /// <summary>
    /// The next node is one of:
    /// - The first child of the current node
    /// - The next sibling of the current node
    /// - The next sibling of the current node's parent
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private IWidget GetNextNode(IWidget node) {
        IWidget? firstChild = node.GetChildren().FirstOrDefault();
        if (firstChild is not null) {
            return firstChild;
        }

        IWidget? current = node;
        while (current is not null) {
            IWidget? sibling = GetNextSibling(current);
            if (sibling is not null) {
                return sibling;
            }

            current = current.Parent;
        }

        return _root;
    }

    /// <summary>
    /// Previous node is one of:
    /// - The deepest child of the previous sibling
    /// - The previous sibling
    /// - The parent of the current node
    /// - The root widget
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private IWidget GetPreviousNode(IWidget node) {
        IWidget? previousSibling = GetPreviousSibling(node);
        if (previousSibling is not null) {
            return GetDeepestLastDescendent(previousSibling);
        }

        return node.Parent ?? _root;
    }
}
