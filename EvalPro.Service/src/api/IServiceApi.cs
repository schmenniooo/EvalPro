using EvalProService.impl.model;

namespace EvalProService.api;

public interface IServiceApi
{
    List<AuditCommittee> GetCommitteesList();

    List<ProjectDocumentation> GetProjectDocumentationList();

    List<ProjectPresentation> GetProjectPresentationList();

    List<TechConversation> GetProjectTechConversationList();

    List<SupplementaryExamination> GetSupplementaryExaminationList();

}