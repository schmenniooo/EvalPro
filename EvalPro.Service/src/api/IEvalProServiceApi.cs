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
    void UpdateCommittee(AuditCommittee committee, string? designation = null, string? apprenticeShip = null, List<DateTime>? testDates = null);

    /// <summary>Removes a committee</summary>
    void RemoveCommittee(AuditCommittee committee);

    /// <summary>Finds a committee by ID, or returns null</summary>
    AuditCommittee? GetCommitteeById(string id);

    /// <summary>Returns all committees as a read-only list</summary>
    IReadOnlyList<AuditCommittee> GetAllCommittees();

    // ===== Examinee CRUD =====

    /// <summary>Creates a new examinee</summary>
    Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle);

    /// <summary>Updates an existing examinee's properties (pass null to leave unchanged)</summary>
    void UpdateExaminee(Examinee examinee, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null);

    /// <summary>Removes an examinee and clears any committee references to it</summary>
    void RemoveExaminee(Examinee examinee);

    /// <summary>Finds an examinee by ID, or returns null</summary>
    Examinee? GetExamineeById(string id);

    /// <summary>Returns all examinees as a read-only list</summary>
    IReadOnlyList<Examinee> GetAllExaminees();

    // ===== Relationship Management: Committee <-> Examinee =====

    /// <summary>Assigns an examinee to a committee</summary>
    void AssignExamineeToCommittee(AuditCommittee committee, Examinee examinee);

    /// <summary>Removes the examinee reference from a committee</summary>
    void RemoveExamineeFromCommittee(AuditCommittee committee);

    /// <summary>Gets the examinee assigned to a committee, or null</summary>
    Examinee? GetExamineeForCommittee(AuditCommittee committee);

    /// <summary>Finds which committee an examinee belongs to, or null</summary>
    AuditCommittee? GetCommitteeForExaminee(Examinee examinee);

    // ===== Rating Assignment =====

    /// <summary>Assigns a project documentation rating to an examinee</summary>
    void AssignProjectDocumentation(Examinee examinee, ProjectDocumentation documentation);

    /// <summary>Assigns a project presentation rating to an examinee</summary>
    void AssignProjectPresentation(Examinee examinee, ProjectPresentation presentation);

    /// <summary>Assigns a tech conversation rating to an examinee</summary>
    void AssignTechConversation(Examinee examinee, TechConversation conversation);

    /// <summary>Assigns a supplementary examination to an examinee</summary>
    void AssignSupplementaryExamination(Examinee examinee, SupplementaryExamination exam);
}
