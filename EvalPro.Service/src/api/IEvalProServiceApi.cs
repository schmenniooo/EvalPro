using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;

namespace EvalProService.api;

/// <summary>
/// API Interface for frontend to interact with backend
/// All operations are thread-safe and automatically persisted
/// Methods throw EntityNotFoundException when referenced entities are not found
/// </summary>
public interface IEvalProServiceApi
{
    // ===== Committee CRUD =====
    AuditCommittee AddCommittee(string designation, string apprenticeShip, List<DateTime> testDates);
    void UpdateCommittee(string id, string? designation = null, string? apprenticeShip = null, List<DateTime>? testDates = null);
    void RemoveCommittee(string id);
    AuditCommittee? GetCommitteeById(string id);
    IReadOnlyList<AuditCommittee> GetAllCommittees();

    // ===== Examinee CRUD =====
    Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle);
    void UpdateExaminee(string id, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null);
    void RemoveExaminee(string id);
    Examinee? GetExamineeById(string id);
    IReadOnlyList<Examinee> GetAllExaminees();

    // ===== Relationship Management: Committee <-> Examinee =====
    void AssignExamineeToCommittee(string committeeId, string examineeId);
    void RemoveExamineeFromCommittee(string committeeId);
    Examinee? GetExamineeForCommittee(string committeeId);
    AuditCommittee? GetCommitteeForExaminee(string examineeId);

    // ===== Relationship Management: Examinee <-> Ratings =====
    void AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation);
    ProjectDocumentation? GetProjectDocumentationForExaminee(string examineeId);

    void AssignProjectPresentation(string examineeId, ProjectPresentation presentation);
    ProjectPresentation? GetProjectPresentationForExaminee(string examineeId);

    void AssignTechConversation(string examineeId, TechConversation conversation);
    TechConversation? GetTechConversationForExaminee(string examineeId);

    void AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam);
    SupplementaryExamination? GetSupplementaryExaminationForExaminee(string examineeId);
}
