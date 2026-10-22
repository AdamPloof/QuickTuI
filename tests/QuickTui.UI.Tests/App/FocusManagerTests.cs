using Xunit;

using QuickTui.UI;
using QuickTui.UI.App;
using QuickTui.UI.Widgets;
using QuickTui.UI.Layout;
using QuickTui.UI.Logging;

namespace QuickTui.UI.Tests.App;

public class FocusManagerTests {
    public ILayout Layout { get; init; } = new HBoxLayout();
    private NullLogger _stubLogger = new NullLogger();

    [Fact]
    public void FocusNextSingleWidgetTree() {
        Container root = new Container(null, Layout);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.FocusNext();

        Assert.True(root.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocuPreviousSingleWidgetTree() {
        Container root = new Container(null, Layout);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.FocusPrevious();

        Assert.True(root.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusNextOnlyRootAcceptsFocus() {
        Container root = new Container(null, Layout);
        MockNonFocusable childA = new MockNonFocusable(root);
        MockNonFocusable childB = new MockNonFocusable(root);
        MockNonFocusable childC = new MockNonFocusable(root);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.FocusNext();

        Assert.True(root.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusPreviousOnlyRootAcceptsFocus() {
        Container root = new Container(null, Layout);
        MockNonFocusable childA = new MockNonFocusable(root);
        MockNonFocusable childB = new MockNonFocusable(root);
        MockNonFocusable childC = new MockNonFocusable(root);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.FocusPrevious();

        Assert.True(root.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusFirstChild() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.FocusNext();

        Assert.True(childA.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusNextSibling() {
        Container root = new Container(null, Layout);
        MockFocusable siblingA = new MockFocusable(root);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childA);
        focusManager.FocusNext();

        Assert.True(childB.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusNextParent() {
        Container root = new Container(null, Layout);
        MockFocusable siblingA = new MockFocusable(root);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        MockFocusable siblingB = new MockFocusable(root);
        MockFocusable childBA = new MockFocusable(siblingB);
        MockFocusable childBB = new MockFocusable(siblingB);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childB);
        focusManager.FocusNext();

        Assert.True(siblingB.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusNextChild() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        Container siblingB = new Container(root, Layout);
        MockFocusable childBA = new MockFocusable(siblingB);
        MockFocusable childBB = new MockFocusable(siblingB);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childB);
        focusManager.FocusNext();

        Assert.True(childBA.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusNextWrapToRoot() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        Container siblingB = new Container(root, Layout);
        MockFocusable childBA = new MockFocusable(siblingB);
        MockFocusable childBB = new MockFocusable(siblingB);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childBB);
        focusManager.FocusNext();

        Assert.True(root.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusLastWidgetFromRoot() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        Container siblingB = new Container(root, Layout);
        MockFocusable childBA = new MockFocusable(siblingB);
        MockFocusable childBB = new MockFocusable(siblingB);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.FocusPrevious();

        Assert.True(childBB.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusPreviousSibling() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childB);
        focusManager.FocusPrevious();

        Assert.True(childA.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusPreviousSiblingDeepestChild() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        Container siblingB = new Container(root, Layout);
        MockFocusable childBA = new MockFocusable(siblingB);
        MockFocusable childBB = new MockFocusable(siblingB);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childBA);
        focusManager.FocusPrevious();

        Assert.True(childB.GetId() == focusManager.GetFocusedWidget().GetId());
    }

    [Fact]
    public void FocusParent() {
        Container root = new Container(null, Layout);
        Container siblingA = new Container(root, Layout);
        MockFocusable childA = new MockFocusable(siblingA);
        MockFocusable childB = new MockFocusable(siblingA);
        MockFocusable siblingB = new MockFocusable(root);
        MockFocusable childBA = new MockFocusable(siblingB);
        MockFocusable childBB = new MockFocusable(siblingB);
        FocusManager focusManager = new FocusManager(root, _stubLogger);
        focusManager.ChangeFocus(childBA);
        focusManager.FocusPrevious();

        Assert.True(siblingB.GetId() == focusManager.GetFocusedWidget().GetId());
    }
}

internal class MockFocusable : Widget {
    public override bool AcceptsFocus { get; protected set; } = true;

    public MockFocusable(IWidget? parent) : base(parent) { }

    public override Cell[] Paint() {
        return Cell.EmptyCells(4);
    }

    public override bool SetFocus() {
        HasFocus = true;

        return true;
    }

    public override bool ClearFocus() {
        HasFocus = false;

        return true;
    }

    /// <inheritdoc />
    public override bool CaptureTabKey() {
        return false;
    }
}

internal class MockNonFocusable : Widget {
    public override bool AcceptsFocus { get; protected set; } = true;

    public MockNonFocusable(IWidget? parent) : base(parent) { }

    public override Cell[] Paint() {
        return Cell.EmptyCells(4);
    }

    public override bool SetFocus() {
        return false;
    }

    public override bool ClearFocus() {
        HasFocus = false;

        return true;
    }

    /// <inheritdoc />
    public override bool CaptureTabKey() {
        return false;
    }
}
