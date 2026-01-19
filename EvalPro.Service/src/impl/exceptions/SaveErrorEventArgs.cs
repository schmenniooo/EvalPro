namespace EvalProService.impl.exceptions;

/// <summary>
/// Event arguments for save error notifications
/// </summary>
public class SaveErrorEventArgs : EventArgs
{
    public Exception Exception { get; }
    public DateTime Timestamp { get; }
    public bool IsCritical { get; }

    public SaveErrorEventArgs(Exception exception, bool isCritical = false)
    {
        Exception = exception;
        Timestamp = DateTime.Now;
        IsCritical = isCritical;
    }
}
