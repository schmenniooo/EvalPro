
using EvalProService.api;

namespace EvalProServiceTest.service;

public class EvalProServiceTest
{
    public void Test()
    {
        IServiceApi serviceApi = new EvalProService.impl.EvalProService();
    }

}