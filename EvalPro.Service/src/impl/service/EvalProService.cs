using EvalProService.api;
using EvalProService.db;
using EvalProService.db.autoSaver;
using EvalProService.impl.model;
using EvalProService.impl.model.entities;

namespace EvalProService.impl;

public class EvalProService : IServiceApi
{
    private readonly ServiceData _data;

    public EvalProService()
    {
        _data = new ServiceData();
        new AutoDataSaver(_data).StartAutoSaveTimer();
    }
    
    public List<AuditCommittee> GetCommitteesList()
    {
        return _data.GetCommitteesList();
    }

    public List<Examinee> GetExamineesList()
    {
        return _data.GetExamineesList();
    }

    public List<ProjectDocumentation> GetProjectDocumentationList()
    {
        return _data.GetProjectDocumentationList();
    }

    public List<ProjectPresentation> GetProjectPresentationList()
    {
        return _data.GetProjectPresentationList();
    }

    public List<TechConversation> GetProjectTechConversationList()
    {
        return _data.GetProjectTechConversationList();
    }

    public List<SupplementaryExamination> GetSupplementaryExaminationList()
    {
        return _data.GetSupplementaryExaminationList();
    }
    
}