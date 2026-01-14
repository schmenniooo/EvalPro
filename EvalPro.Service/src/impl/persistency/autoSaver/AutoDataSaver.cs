using System.Timers;

namespace EvalProService.impl.persistency.autoSaver;

public class AutoDataSaver
{
    private const int TimerDuration = 500;
    private readonly ServiceData _data;

    public AutoDataSaver(ServiceData data)
    {
        _data = data;
    }

    // Creates a new timer instance for 500ms
    public void StartAutoSaveTimer()
    {
        var timer = new System.Timers.Timer(TimerDuration);
        timer.Elapsed += SaveDataTimerEvent;
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Start();
    }

    private void SaveDataTimerEvent(object? source, ElapsedEventArgs e)
    {
        _data.SaveConfigToJson();
    }
}