using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;

namespace EvalProService.api;

/// <summary>
/// API Interface for frontend to interact with backend.
/// All operations are thread-safe and automatically persisted.
/// Methods throw EntityNotFoundException when referenced entities are not found.
/// </summary>
public interface IEvalProServiceApi
{
    // ===== Committee CRUD =====

    /// <summary>Creates a new audit committee</summary>
    AuditCommittee AddCommittee(string designation, string apprenticeShip, List<DateTime> testDates);

    /// <summary>Updates an existing committee's properties (pass null to leave unchanged)</summary>
    void UpdateCommittee(string id, string? designation = null, string? apprenticeShip = null, List<DateTime>? testDates = null);

    /// <summary>Removes a committee by ID</summary>
    void RemoveCommittee(string id);

    /// <summary>Finds a committee by ID, or returns null</summary>
    AuditCommittee? GetCommitteeById(string id);

    /// <summary>Returns all committees as a read-only list</summary>
    IReadOnlyList<AuditCommittee> GetAllCommittees();

    // ===== Examinee CRUD =====

    /// <summary>Creates a new examinee</summary>
    Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle);

    /// <summary>Updates an existing examinee's properties (pass null to leave unchanged)</summary>
    void UpdateExaminee(string id, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null);

    /// <summary>Removes an examinee by ID and clears any committee references to it</summary>
    void RemoveExaminee(string id);

    /// <summary>Finds an examinee by ID, or returns null</summary>
    Examinee? GetExamineeById(string id);

    /// <summary>Returns all examinees as a read-only list</summary>
    IReadOnlyList<Examinee> GetAllExaminees();

    // ===== Relationship Management: Committee <-> Examinee =====

    /// <summary>Assigns an examinee to a committee</summary>
    void AssignExamineeToCommittee(string committeeId, string examineeId);

    /// <summary>Removes the examinee reference from a committee</summary>
    void RemoveExamineeFromCommittee(string committeeId);

    /// <summary>Gets the examinee assigned to a committee, or null</summary>
    Examinee? GetExamineeForCommittee(string committeeId);

    /// <summary>Finds which committee an examinee belongs to, or null</summary>
    AuditCommittee? GetCommitteeForExaminee(string examineeId);

    // ===== Rating Assignment =====

    /// <summary>Assigns a project documentation rating to an examinee</summary>
    void AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation);

    /// <summary>Assigns a project presentation rating to an examinee</summary>
    void AssignProjectPresentation(string examineeId, ProjectPresentation presentation);

    /// <summary>Assigns a tech conversation rating to an examinee</summary>
    void AssignTechConversation(string examineeId, TechConversation conversation);

    /// <summary>Assigns a supplementary examination to an examinee</summary>
    void AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam);
}
