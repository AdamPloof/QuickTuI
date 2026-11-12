using System;
using System.Collections.Generic;

using QuickTui.UI.Text;

namespace QuickTui.UI.Tests.Text;

public class TextCaretTests {
    private static readonly string baseStr = """
        abcdefghijklmnop
        qrstuvwxyz
        ABCDEFGHIJKLMNOP
        QRSTUVWXYZ
        0123456789
        """;

    [Fact]
    public void MoveToValidPosition() {

    }

    [Fact]
    public void MovePastLineEndStopsAtLineEnd() {

    }

    [Fact]
    public void MovePastContentEndStopsAtContentEnd() {

    }

    [Fact]
    public void MoveRightFromLineStart() {

    }

    [Fact]
    public void MoveRightFromMidLine() {

    }

    [Fact]
    public void MoveRightFromLineEndToNextLine() {

    }
    
    [Fact]
    public void MoveLeftFromLineEnd() {

    }

    [Fact]
    public void MoveLeftFromMidLine() {

    }
    
    [Fact]
    public void MoveLeftFromLineStartToPreviousLine() {
        
    }
}
