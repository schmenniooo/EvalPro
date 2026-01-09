using EvalProService.api;
using EvalProService.db;
using EvalProService.impl.model;

namespace EvalProService.impl;

public class EvalProService : IServiceApi
{
    private readonly ServiceDataController _dataController = new();
    
    private List<Examinee> _examineeslist = [];
    private List<AuditCommittee> _committeeslist = [];
    private List<ProjectDocumentation>  _projectDocumentationList = [];
    private List<ProjectPresentation>  _projectPresentationList = [];
    private List<TechConversation>  _projectTechConversationList = [];
    private List<SupplementaryExamination>  _supplementaryExaminationList = [];
    
    public void SaveConfig()
    {
        throw new NotImplementedException();
    }

    public void LoadConfig()
    {
        throw new NotImplementedException();
    }
}