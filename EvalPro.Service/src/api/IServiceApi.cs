using EvalProService.impl.model;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;

namespace EvalProService.api;

public interface IServiceApi
{
    List<AuditCommittee> GetCommitteesList();
    
    List<Examinee> GetExamineesList();

    List<ProjectDocumentation> GetProjectDocumentationList();

    List<ProjectPresentation> GetProjectPresentationList();

    List<TechConversation> GetProjectTechConversationList();

    List<SupplementaryExamination> GetSupplementaryExaminationList();

}