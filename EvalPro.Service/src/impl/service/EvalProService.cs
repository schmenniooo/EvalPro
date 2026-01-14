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
public class EvalProService : IEvalProServiceApi
{
    private readonly ServiceData _data;

    public EvalProService()
    {
        _data = new ServiceData();
        var autoSaver = new AutoDataSaver(_data);
        autoSaver.StartAutoSaveTimer();
    }

    // ===== Committee Operations =====
    
    /// <summary>
    /// Creates a new committee object with given parameters
    /// </summary>
    /// <param name="designation"></param>
    /// <param name="apprenticeShip"></param>
    /// <param name="testDates"></param>
    /// <returns></returns>
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
    /// <returns></returns>
    public bool UpdateCommittee(string id, string? designation = "", string? apprenticeShip = "", List<DateTime>? testDates = null)
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
    /// <returns></returns>
    public bool RemoveCommittee(string id)
    {
        return _data.RemoveCommittee(id);
    }

    /// <summary>
    /// Returns a committee object found with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AuditCommittee? GetCommitteeById(string id)
    {
        return _data.GetCommitteeById(id);
    }

    /// <summary>
    /// Returns a readonly list of committee's to avoid unmanaged access on it
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<AuditCommittee> GetAllCommittees()
    {
        return _data.GetAllCommittees();
    }

    // ===== Examinee Operations =====

    public Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle)
    {
        var examinee = new Examinee(name, company, contactPerson, projectTitle);
        _data.AddExaminee(examinee);
        return examinee;
    }

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

    public Examinee? GetExamineeById(string id)
    {
        return _data.GetExamineeById(id);
    }

    public IReadOnlyList<Examinee> GetAllExaminees()
    {
        return _data.GetAllExaminees();
    }

    // ===== Relationship Management: Committee <-> Examinee =====

    /// <summary>
    /// Connects an examinee to a committee by setting the ID reference
    /// </summary>
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
    public Examinee? GetExamineeForCommittee(string committeeId)
    {
        var committee = _data.GetCommitteeById(committeeId);
        if (committee?.ExamineeId == null) return null;

        return _data.GetExamineeById(committee.ExamineeId);
    }

    /// <summary>
    /// Finds which committee an examinee belongs to
    /// </summary>
    public AuditCommittee? GetCommitteeForExaminee(string examineeId)
    {
        var committees = _data.GetAllCommittees();
        return committees.FirstOrDefault(c => c.ExamineeId == examineeId);
    }

    // ===== Relationship Management: Examinee <-> Ratings =====

    public bool AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation)
    {
        _data.AddProjectDocumentation(documentation);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.ProjectDocumentationId = documentation.Id;
        });
    }

    public ProjectDocumentation? GetProjectDocumentationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.ProjectDocumentationId == null) return null;

        return _data.GetProjectDocumentationById(examinee.ProjectDocumentationId);
    }

    public bool AssignProjectPresentation(string examineeId, ProjectPresentation presentation)
    {
        _data.AddProjectPresentation(presentation);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.ProjectPresentationId = presentation.Id;
        });
    }

    public ProjectPresentation? GetProjectPresentationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.ProjectPresentationId == null) return null;

        return _data.GetProjectPresentationById(examinee.ProjectPresentationId);
    }

    public bool AssignTechConversation(string examineeId, TechConversation conversation)
    {
        _data.AddTechConversation(conversation);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.TechConversationId = conversation.Id;
        });
    }

    public TechConversation? GetTechConversationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.TechConversationId == null) return null;

        return _data.GetTechConversationById(examinee.TechConversationId);
    }

    public bool AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam)
    {
        _data.AddSupplementaryExamination(exam);

        return _data.UpdateExaminee(examineeId, examinee =>
        {
            //examinee.SupplementaryExaminationId = exam.Id;
        });
    }

    public SupplementaryExamination? GetSupplementaryExaminationForExaminee(string examineeId)
    {
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.SupplementaryExaminationId == null) return null;

        return _data.GetSupplementaryExaminationById(examinee.SupplementaryExaminationId);
    }
}