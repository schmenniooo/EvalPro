using EvalProService.api;
using EvalProService.db;
using EvalProService.impl.model;

namespace EvalProService.impl;

public class EvalProService : IServiceApi
{
    private List<AuditCommittee> _committeeslist = [];
    private List<Examinee> _examineeslist = [];
    private List<ProjectDocumentation>  _projectDocumentationList = [];
    private List<ProjectPresentation>  _projectPresentationList = [];
    private List<TechConversation>  _projectTechConversationList = [];
    private List<SupplementaryExamination>  _supplementaryExaminationList = [];

    public EvalProService()
    {
        
    }
    
    public List<AuditCommittee> GetCommitteesList()
    {
        return _committeeslist;
    }

    public List<Examinee> GetExamineesList()
    {
        return _examineeslist;
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