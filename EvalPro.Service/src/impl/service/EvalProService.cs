using EvalProService.api;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;
using EvalProService.impl.persistency;
using EvalProService.impl.persistency.autoSaver;

namespace EvalProService.impl.service;

/// <summary>
/// Service layer that manages business logic and relationships between entities
/// Frontend interacts only with this class
/// </summary>
public class EvalProService : IEvalProServiceApi, IDisposable
{
    private readonly ServiceData _data;

    public EvalProService()
    {
        _data = new ServiceData();
        var autoDataSaver = new AutoDataSaver(_data);
        autoDataSaver.StartAutoSaveTimer();
    }

    // ===== Committee Operations =====
    
    /// <summary>
    /// Creates a new committee object with given parameters
    /// </summary>
    /// <param name="designation"></param>
    /// <param name="apprenticeShip"></param>
    /// <param name="testDates"></param>
    /// <returns>The newly created AuditCommittee object</returns>
    public AuditCommittee AddCommittee(string designation, string apprenticeShip, List<DateTime> testDates)
    {
        var committee = new AuditCommittee(designation, apprenticeShip, testDates);
        _data.AddCommittee(committee);
        return committee;
    }

    /// <summary>
    /// Updates a committee object by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="designation"></param>
    /// <param name="apprenticeShip"></param>
    /// <param name="testDates"></param>
    /// <returns>True if the committee was found and updated, false if not found</returns>
    public bool UpdateCommittee(string id, string? designation = null, string? apprenticeShip = "", List<DateTime>? testDates = null)
    {
        return _data.UpdateCommittee(id, committee =>
        {
            if (designation != null) committee.Designation = designation;
            if (apprenticeShip != null) committee.ApprenticeShip = apprenticeShip;
            if (testDates != null) committee.TestDates = testDates;
        });
    }

    /// <summary>
    /// Deletes a committee object
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True if the committee was found and removed, false if not found</returns>
    public bool RemoveCommittee(string id)
    {
        return _data.RemoveCommittee(id);
    }

    /// <summary>
    /// Returns a committee object found with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The AuditCommittee object if found, null otherwise</returns>
    public AuditCommittee? GetCommitteeById(string id)
    {
        return _data.GetCommitteeById(id);
    }

    /// <summary>
    /// Returns a readonly list of committee's to avoid unmanaged access on it
    /// </summary>
    /// <returns>A readonly list containing all AuditCommittee objects</returns>
    public IReadOnlyList<AuditCommittee> GetAllCommittees()
    {
        return _data.GetAllCommittees();
    }

    // ===== Examinee Operations =====

    /// <summary>
    /// Creates a new Examinee object with given parameters
    /// </summary>
    /// <param name="name"></param>
    /// <param name="company"></param>
    /// <param name="contactPerson"></param>
    /// <param name="projectTitle"></param>
    /// <returns>The newly created Examinee object</returns>
    public Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle)
    {
        var examinee = new Examinee(name, company, contactPerson, projectTitle);
        _data.AddExaminee(examinee);
        return examinee;
    }

    /// <summary>
    /// Updates an examinee object by given parameters
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="company"></param>
    /// <param name="contactPerson"></param>
    /// <param name="projectTitle"></param>
    /// <returns>True if the examinee was found and updated, false if not found</returns>
    public bool UpdateExaminee(string id, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null)
    {
        return _data.UpdateExaminee(id, examinee =>
        {
            if (name != null) examinee.Name = name;
            if (company != null) examinee.Company = company;
            if (contactPerson != null) examinee.ContactPerson = contactPerson;
            if (projectTitle != null) examinee.ProjectTitle = projectTitle;
        });
    }

    /// <summary>
    /// Searches for examinee object in committee's and deletes it's reference to the committee
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True if the examinee was found and removed, false if not found</returns>
    public bool RemoveExaminee(string id)
    {
        // First, remove from any committees that reference this examinee
        var committees = _data.GetAllCommittees();
        foreach (var committee in committees)
        {
            if (committee.ExamineeId == id)
            {
                _data.UpdateCommittee(committee.Id, c => c.ExamineeId = null);
            }
        }

        return _data.RemoveExaminee(id);
    }

    /// <summary>
    /// Returns an examinee object found with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The Examinee object if found, null otherwise</returns>
    public Examinee? GetExamineeById(string id)
    {
        return _data.GetExamineeById(id);
    }

    /// <summary>
    /// Returns examinee list from data class as readonly to avoid unmanaged access
    /// </summary>
    /// <returns>A readonly list containing all Examinee objects</returns>
    public IReadOnlyList<Examinee> GetAllExaminees()
    {
        return _data.GetAllExaminees();
    }

    // ===== Relationship Management: Committee <-> Examinee =====

    /// <summary>
    /// Connects an examinee to a committee by setting the ID reference
    /// </summary>
    /// <returns>True if the assignment succeeded, false if examinee or committee not found</returns>
    public bool AssignExamineeToCommittee(string committeeId, string examineeId)
    {
        // Verify examinee exists
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee == null) return false;

        // Set the relationship
        return _data.UpdateCommittee(committeeId, committee =>
        {
            committee.ExamineeId = examineeId;
        });
    }

    /// <summary>
    /// Removes the examinee from a committee
    /// </summary>
    /// <returns>True if the committee was found and the examinee reference was cleared, false if committee not found</returns>
    public bool RemoveExamineeFromCommittee(string committeeId)
    {
        return _data.UpdateCommittee(committeeId, committee =>
        {
            committee.ExamineeId = null;
        });
    }

    /// <summary>
    /// Resolves the relationship: Gets the examinee for a committee
    /// </summary>
    /// <returns>The Examinee object if found and assigned to the committee, null otherwise</returns>
    public Examinee? GetExamineeForCommittee(string committeeId)
    {
        var committee = _data.GetCommitteeById(committeeId);
        if (committee?.ExamineeId == null) return null;

        return _data.GetExamineeById(committee.ExamineeId);
    }

    /// <summary>
    /// Finds which committee an examinee belongs to
    /// </summary>
    /// <returns>The AuditCommittee object if the examinee is assigned to one, null otherwise</returns>
    public AuditCommittee? GetCommitteeForExaminee(string examineeId)
    {
        var committees = _data.GetAllCommittees();
        return committees.FirstOrDefault(c => c.ExamineeId == examineeId);
    }

    // ===== Relationship Management: Examinee <-> Ratings =====

    /// <summary>
    /// Adds a reference to a given ProjectDocumentation object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="documentation"></param>
    /// <returns>True if the assignment succeeded, false if examinee not found</returns>
    public bool AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation)
    {
        _data.AddProjectDocumentation(documentation);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.ProjectDocumentationId = documentation.Id;
        });
    }

    /// <summary>
    /// Returns a ProjectDocumentation object from a examinee with given id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The ProjectDocumentation object if found and assigned to the examinee, null otherwise</returns>
    public ProjectDocumentation? GetProjectDocumentationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.ProjectDocumentationId == null) return null;

        return _data.GetProjectDocumentationById(examinee.ProjectDocumentationId);
    }

    /// <summary>
    /// Adds a reference to a given ProjectPresentation object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="presentation"></param>
    /// <returns>True if the assignment succeeded, false if examinee not found</returns>
    public bool AssignProjectPresentation(string examineeId, ProjectPresentation presentation)
    {
        _data.AddProjectPresentation(presentation);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.ProjectPresentationId = presentation.Id;
        });
    }

    /// <summary>
    /// Returns a ProjectPresentation object from an Examinee with id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The ProjectPresentation object if found and assigned to the examinee, null otherwise</returns>
    public ProjectPresentation? GetProjectPresentationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.ProjectPresentationId == null) return null;

        return _data.GetProjectPresentationById(examinee.ProjectPresentationId);
    }

    /// <summary>
    /// Adds a reference to a given TechConversation object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="conversation"></param>
    /// <returns>True if the assignment succeeded, false if examinee not found</returns>
    public bool AssignTechConversation(string examineeId, TechConversation conversation)
    {
        _data.AddTechConversation(conversation);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.TechConversationId = conversation.Id;
        });
    }

    /// <summary>
    /// Returns a TechConversation object from an Examinee with id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The TechConversation object if found and assigned to the examinee, null otherwise</returns>
    public TechConversation? GetTechConversationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.TechConversationId == null) return null;

        return _data.GetTechConversationById(examinee.TechConversationId);
    }

    /// <summary>
    /// Adds a reference to a given SupplementaryExamination object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="exam"></param>
    /// <returns>True if the assignment succeeded, false if examinee not found</returns>
    public bool AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam)
    {
        _data.AddSupplementaryExamination(exam);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.SupplementaryExaminationId = exam.Id;
        });
    }

    /// <summary>
    /// Returns a SupplementaryExamination object from an Examinee with id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The SupplementaryExamination object if found and assigned to the examinee, null otherwise</returns>
    public SupplementaryExamination? GetSupplementaryExaminationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.SupplementaryExaminationId == null) return null;

        return _data.GetSupplementaryExaminationById(examinee.SupplementaryExaminationId);
    }

    /// <summary>
    /// Dispose function to clean up references
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}