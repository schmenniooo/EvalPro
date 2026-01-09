using EvalProService.api;
using EvalProService.db;
using EvalProService.impl.model;

namespace EvalProService.impl;

public class EvalProService : IServiceApi
{
    private readonly ServiceDataController _dataController;
    
    private List<Examinee> _examineeslist = [];
    private List<AuditCommittee> _committeeslist = [];
    private List<ProjectDocumentation>  _projectDocumentationList = [];
    private List<ProjectPresentation>  _projectPresentationList = [];
    private List<TechConversation>  _projectTechConversationList = [];
    private List<SupplementaryExamination>  _supplementaryExaminationList = [];

    public EvalProService()
    {
        _dataController = new ServiceDataController();
    }
    
    public List<AuditCommittee> GetCommitteesList()
    {
        return _committeeslist;
    }

    public List<ProjectDocumentation> GetProjectDocumentationList()
    {
        return _projectDocumentationList;
    }

    public List<ProjectPresentation> GetProjectPresentationList()
    {
        return _projectPresentationList;
    }

    public List<TechConversation> GetProjectTechConversationList()
    {
        return _projectTechConversationList;
    }

    public List<SupplementaryExamination> GetSupplementaryExaminationList()
    {
        return _supplementaryExaminationList;
    }
    
}