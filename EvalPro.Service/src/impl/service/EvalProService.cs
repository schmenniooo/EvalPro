using EvalProService.api;
using EvalProService.impl.model;
using EvalProService.impl.model.entities;
using EvalProService.impl.persistency;
using EvalProService.impl.persistency.autoSaver;

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
        return _data.CommitteesList;
    }

    public List<Examinee> GetExamineesList()
    {
        return _data.ExamineesList;
    }

    public List<ProjectDocumentation> GetProjectDocumentationList()
    {
        return _data.ProjectDocumentationList;
    }

    public List<ProjectPresentation> GetProjectPresentationList()
    {
        return _data.ProjectPresentationList;
    }

    public List<TechConversation> GetProjectTechConversationList()
    {
        return _data.ProjectTechConversationList;
    }

    public List<SupplementaryExamination> GetSupplementaryExaminationList()
    {
        return _data.SupplementaryExaminationList;
    }
    
}