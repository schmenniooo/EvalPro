namespace EvalProService.db.autoSaver;

public class AutoDataSaver
{
    private readonly ServiceDataController _dataController;

    public AutoDataSaver()
    {
        _dataController = new ServiceDataController();
    }

    public void StartAutoSaveTimer()
    {
        // TODO: Init timer to call save methods every 500 ms
    }
}