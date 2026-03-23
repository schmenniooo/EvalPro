namespace EvalProService.impl.model.events;

/// <summary>
/// Event arguments for save error notifications.
/// </summary>
public class AutoSaveErrorEventArgs : EventArgs
{
    /// <summary>The exception that occurred during the save operation</summary>
    public Exception Exception { get; }
    
    /// <summary>When the error occurred</summary>
    public DateTime Timestamp { get; }
    
    /// <summary>True if this was a final save attempt (e.g. during dispose), false for timer-based saves</summary>
    public bool IsCritical { get; }

    /// <summary>
    /// Creates a new save error event.
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <param name="isCritical">Whether this was a critical (final) save attempt</param>
    public AutoSaveErrorEventArgs(Exception exception, bool isCritical = false)
    {
        Exception = exception;
        Timestamp = DateTime.Now;
        IsCritical = isCritical;
    }
}
