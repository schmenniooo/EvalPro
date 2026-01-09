namespace EvalProService.db.autoSaver;

public class AutoDataSaver
{
    private readonly ServiceData _data;

    public AutoDataSaver(ServiceData data)
    {
        _data = data;
    }

    public void StartAutoSaveTimer()
    {
        // TODO: Init timer to call save methods every 500 ms
    }
}