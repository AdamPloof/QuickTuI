using System;
using System.IO;

using QuickTui.UI;
using QuickTui.UI.Layout;
using QuickTui.UI.Widgets;
using QuickTui.UI.Drawing;
using QuickTui.UI.Style;
using QuickTui.UI.Events;
using QuickTui.UI.App;
using QuickTui.UI.Input;
using QuickTui.UI.Logging;

namespace QuickTui.Demo;

internal static class Program {
    private static void Main() {
        Animation animation = new();
        UI ui = new UI(animation);
        ui.Run();
    }
}

public class UI {
    private readonly string _logPath;

    private IApplication _app;
    private Animation _animation;
    private Canvas? _canvas = null;

    public UI(Animation animation) {
        _logPath = Path.Combine(
            AppContext.BaseDirectory,
            "./var/logs/",
            $"{DateTime.Now.ToString("yyyyMMdd")}.log"
        );

        _animation = animation;
        _app = Build();
        ScheduleAnimation();
    }

    public void Run() {
        _app.Run();
    }
    
    private IApplication Build() {
        VBoxLayout layout = new();
        Container root = new Container(null, layout) {
            StretchVertical = 1,
            StretchHorizontal = 1,
            Padding = new Padding(1)
        };
        TextBox label = new TextBox(
            root,
            "Build something quickly.",
            new CellStyle(TextFormat.Normal, new Color(255, 0, 0))
        ) {
            StretchHorizontal = 1,
            StretchVertical = 1,
            Border = new Border(BorderStyle.Solid)
        };

        _canvas = new Canvas(
            root,
            _animation.GetNextFrame(),
            new CellStyle(TextFormat.Normal, new Color(0, 255, 45))
        ) {
            StretchHorizontal = 1,
            StretchVertical = 1,
            Border = new Border(BorderStyle.Solid)
        };

        DateTime now = DateTime.Now;
        FileLogger logger = new(_logPath);
        Renderer renderer = new();
        EventDispatcher dispatcher = new(logger);
        InputParser inputParser = new();
        FocusManager focusManager = new(root, logger);
        CursorManager cursorManager = new(logger);
        Scheduler scheduler = new();
        Application app = new (
            root,
            renderer,
            dispatcher,
            inputParser,
            focusManager,
            cursorManager,
            scheduler
        );

        return app;
    }

    private void ScheduleAnimation() {
        if (_canvas is null) {
            throw new InvalidOperationException("Canvas not initialized");
        }

        TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
        Timer timer = new Timer() { Repeat = true, Timeout = timeout };
        _app.Connect(timer, (ITimer _) => _canvas.SetContent(_animation.GetNextFrame()));
    }
}
