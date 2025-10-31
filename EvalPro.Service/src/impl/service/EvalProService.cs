using EvalProService.api;
using EvalProService.db;

namespace EvalProService.impl;

public class EvalProService : IServiceApi
{
    private readonly ServiceDataController _dataController = new();

    public void SaveConfig()
    {
        throw new NotImplementedException();
    }

    public void LoadConfig()
    {
        throw new NotImplementedException();
    }
}