using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;

namespace EvalProService.api;

/// <summary>
/// API Interface for frontend to interact with backend
/// All operations are thread-safe and automatically persisted
/// </summary>
public interface IEvalProServiceApi
{
    // ===== Committee CRUD =====
    AuditCommittee AddCommittee(string designation, string apprenticeShip, List<DateTime> testDates);
    bool UpdateCommittee(string id, string? designation = null, string? apprenticeShip = null, List<DateTime>? testDates = null);
    bool RemoveCommittee(string id);
    AuditCommittee? GetCommitteeById(string id);
    IReadOnlyList<AuditCommittee> GetAllCommittees();

    // ===== Examinee CRUD =====
    Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle);
    bool UpdateExaminee(string id, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null);
    bool RemoveExaminee(string id);
    Examinee? GetExamineeById(string id);
    IReadOnlyList<Examinee> GetAllExaminees();

    // ===== Relationship Management: Committee <-> Examinee =====
    bool AssignExamineeToCommittee(string committeeId, string examineeId);
    bool RemoveExamineeFromCommittee(string committeeId);
    Examinee? GetExamineeForCommittee(string committeeId);
    AuditCommittee? GetCommitteeForExaminee(string examineeId);

    // ===== Relationship Management: Examinee <-> Ratings =====
    bool AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation);
    ProjectDocumentation? GetProjectDocumentationForExaminee(string examineeId);

    bool AssignProjectPresentation(string examineeId, ProjectPresentation presentation);
    ProjectPresentation? GetProjectPresentationForExaminee(string examineeId);

    bool AssignTechConversation(string examineeId, TechConversation conversation);
    TechConversation? GetTechConversationForExaminee(string examineeId);

    bool AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam);
    SupplementaryExamination? GetSupplementaryExaminationForExaminee(string examineeId);
}