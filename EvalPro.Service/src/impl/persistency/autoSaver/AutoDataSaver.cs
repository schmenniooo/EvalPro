using System.Timers;
using Timer = System.Timers.Timer;

namespace EvalProService.impl.persistency.autoSaver;

public class AutoDataSaver : IDisposable
{
    private const int TimerDuration = 500;
    private readonly Timer _timer = new(TimerDuration);
    private readonly ServiceData _data;

    public AutoDataSaver(ServiceData data)
    {
        _data = data;
    }

    /// <summary>
    /// Creates a new timer instance for 500ms
    /// </summary>
    public void StartAutoSaveTimer()
    {
        _timer.Elapsed -= SaveDataTimerEvent;
        _timer.Elapsed += SaveDataTimerEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        _timer.Start();
    }

    /// <summary>
    /// Timer Event method for auto save timer
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void SaveDataTimerEvent(object? source, ElapsedEventArgs e)
    {
        // Handling with try catch to avoid the timer thread to crash:
        try
        {
            _data.SaveConfigToJson();
        }
        catch (Exception)
        {
            // Logging
        }
    }

    /// <summary>
    /// Dispose method to save data one last time and close timer properly
    /// </summary>
    public void Dispose()
    {
        // Save data before garbage collection:
        _data.SaveConfigToJson();
        
        // Cleaning up timer instance
        _timer.Elapsed -= SaveDataTimerEvent;
        _timer.Stop();
        _timer.Dispose();
    }
}