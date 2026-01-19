using System.Timers;
using EvalProService.impl.exceptions;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace EvalProService.impl.persistency.autoSaver;

public class AutoDataSaver : IDisposable
{
    private const int TimerDuration = 500;
    private readonly Timer _timer = new(TimerDuration);
    private readonly ServiceData _data;
    private readonly ILogger _logger;

    /// <summary>
    /// Event raised when a save operation fails. UI can subscribe to show warnings.
    /// </summary>
    public event EventHandler<AutoSaveErrorEventArgs>? OnSaveError;

    public AutoDataSaver(ServiceData data, ILogger logger)
    {
        _data = data;
        _logger = logger;
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
        _logger.LogInformation("Auto-save timer started with interval {TimerDuration}ms", TimerDuration);
    }

    /// <summary>
    /// Timer Event method for auto save timer
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void SaveDataTimerEvent(object? source, ElapsedEventArgs e)
    {
        try
        {
            _data.SaveConfigToJson();
            _logger.LogInformation("Auto-saved data at {Timestamp}", e.SignalTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during auto-save at {Timestamp}", DateTime.Now);
            OnSaveError?.Invoke(this, new AutoSaveErrorEventArgs(ex, isCritical: false));
        }
    }

    /// <summary>
    /// Dispose method to save data one last time and close timer properly
    /// </summary>
    public void Dispose()
    {
        // Stop timer first to prevent concurrent saves
        _timer.Elapsed -= SaveDataTimerEvent;
        _timer.Stop();
        _timer.Dispose();

        // Save data before garbage collection (critical - last chance to save)
        try
        {
            _data.SaveConfigToJson();
            _logger.LogInformation("Final save completed during dispose");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during final save on dispose");
            OnSaveError?.Invoke(this, new AutoSaveErrorEventArgs(ex, isCritical: true));
        }

        _logger.LogInformation("Auto-Saver disposed");
    }
}
