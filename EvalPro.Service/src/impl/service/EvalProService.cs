using EvalProService.api;
using EvalProService.impl.exceptions;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;
using EvalProService.impl.persistency;
using EvalProService.impl.persistency.autoSaver;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EvalProService.impl.service;

/// <summary>
/// Service layer that manages business logic and relationships between entities
/// Frontend interacts only with this class
/// </summary>
public class EvalProService : IEvalProServiceApi, IDisposable
{
    private readonly AutoDataSaver _autoDataSaver;
    private readonly ServiceData _data;
    private readonly ILogger<EvalProService> _logger;

    public EvalProService()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("evalpro.log", rollingInterval: RollingInterval.Hour)
            .CreateLogger();
        
        _logger = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog();
        }).CreateLogger<EvalProService>();
        
        _data = new ServiceData(_logger);
        _autoDataSaver = new AutoDataSaver(_data, _logger);
        _autoDataSaver.StartAutoSaveTimer();
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
        _logger.LogInformation("Creating committee with: {Designation}, {ApprenticeShip}, {TestDates}", designation, apprenticeShip, testDates);
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
    /// <exception cref="EntityNotFoundException">Thrown when committee with given ID is not found</exception>
    public void UpdateCommittee(string id, string? designation = null, string? apprenticeShip = "", List<DateTime>? testDates = null)
    {
        _logger.LogInformation("Updating committee {Id}", id);
        var result = _data.UpdateCommittee(id, committee =>
        {
            if (designation != null) committee.Designation = designation;
            if (apprenticeShip != null) committee.ApprenticeShip = apprenticeShip;
            if (testDates != null) committee.TestDates = testDates;
        });
        if (!result)
        {
            _logger.LogWarning("Committee {Id} not found for update", id);
            throw new EntityNotFoundException("Committee", id);
        }
    }

    /// <summary>
    /// Deletes a committee object
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="EntityNotFoundException">Thrown when committee with given ID is not found</exception>
    public void RemoveCommittee(string id)
    {
        _logger.LogInformation("Removing committee {Id}", id);
        var result = _data.RemoveCommittee(id);
        if (!result)
        {
            _logger.LogWarning("Committee {Id} not found for removal", id);
            throw new EntityNotFoundException("Committee", id);
        }
    }

    /// <summary>
    /// Returns a committee object found with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The AuditCommittee object if found, null otherwise</returns>
    public AuditCommittee? GetCommitteeById(string id)
    {
        _logger.LogInformation("Getting committee by id {Id}", id);
        return _data.GetCommitteeById(id);
    }

    /// <summary>
    /// Returns a readonly list of committee's to avoid unmanaged access on it
    /// </summary>
    /// <returns>A readonly list containing all AuditCommittee objects</returns>
    public IReadOnlyList<AuditCommittee> GetAllCommittees()
    {
        _logger.LogInformation("Getting all committees");
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
        _logger.LogInformation("Creating examinee: {Name}, {Company}, {ContactPerson}, {ProjectTitle}", name, company, contactPerson, projectTitle);
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
    /// <exception cref="EntityNotFoundException">Thrown when examinee with given ID is not found</exception>
    public void UpdateExaminee(string id, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null)
    {
        _logger.LogInformation("Updating examinee {Id}", id);
        var result = _data.UpdateExaminee(id, examinee =>
        {
            if (name != null) examinee.Name = name;
            if (company != null) examinee.Company = company;
            if (contactPerson != null) examinee.ContactPerson = contactPerson;
            if (projectTitle != null) examinee.ProjectTitle = projectTitle;
        });
        if (!result)
        {
            _logger.LogWarning("Examinee {Id} not found for update", id);
            throw new EntityNotFoundException("Examinee", id);
        }
    }

    /// <summary>
    /// Searches for examinee object in committee's and deletes it's reference to the committee
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="EntityNotFoundException">Thrown when examinee with given ID is not found</exception>
    public void RemoveExaminee(string id)
    {
        _logger.LogInformation("Removing examinee {Id}", id);
        // First, remove from any committees that reference this examinee
        var committees = _data.GetAllCommittees();
        foreach (var committee in committees)
        {
            if (committee.ExamineeId == id)
            {
                _logger.LogInformation("Removing examinee {ExamineeId} reference from committee {CommitteeId}", id, committee.Id);
                _data.UpdateCommittee(committee.Id, c => c.ExamineeId = null);
            }
        }

        var result = _data.RemoveExaminee(id);
        if (!result)
        {
            _logger.LogWarning("Examinee {Id} not found for removal", id);
            throw new EntityNotFoundException("Examinee", id);
        }
    }

    /// <summary>
    /// Returns an examinee object found with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The Examinee object if found, null otherwise</returns>
    public Examinee? GetExamineeById(string id)
    {
        _logger.LogInformation("Getting examinee by id {Id}", id);
        return _data.GetExamineeById(id);
    }

    /// <summary>
    /// Returns examinee list from data class as readonly to avoid unmanaged access
    /// </summary>
    /// <returns>A readonly list containing all Examinee objects</returns>
    public IReadOnlyList<Examinee> GetAllExaminees()
    {
        _logger.LogInformation("Getting all examinees");
        return _data.GetAllExaminees();
    }

    // ===== Relationship Management: Committee <-> Examinee =====

    /// <summary>
    /// Connects an examinee to a committee by setting the ID reference
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee or committee is not found</exception>
    public void AssignExamineeToCommittee(string committeeId, string examineeId)
    {
        _logger.LogInformation("Assigning examinee {ExamineeId} to committee {CommitteeId}", examineeId, committeeId);
        // Verify examinee exists
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee == null)
        {
            _logger.LogWarning("Examinee {ExamineeId} not found for assignment", examineeId);
            throw new EntityNotFoundException("Examinee", examineeId);
        }

        // Set the relationship
        var result = _data.UpdateCommittee(committeeId, committee =>
        {
            committee.ExamineeId = examineeId;
        });
        if (!result)
        {
            _logger.LogWarning("Committee {CommitteeId} not found for assignment", committeeId);
            throw new EntityNotFoundException("Committee", committeeId);
        }
    }

    /// <summary>
    /// Removes the examinee from a committee
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when committee is not found</exception>
    public void RemoveExamineeFromCommittee(string committeeId)
    {
        _logger.LogInformation("Removing examinee from committee {CommitteeId}", committeeId);
        var result = _data.UpdateCommittee(committeeId, committee =>
        {
            committee.ExamineeId = null;
        });
        if (!result)
        {
            _logger.LogWarning("Committee {CommitteeId} not found", committeeId);
            throw new EntityNotFoundException("Committee", committeeId);
        }
    }

    /// <summary>
    /// Resolves the relationship: Gets the examinee for a committee
    /// </summary>
    /// <returns>The Examinee object if found and assigned to the committee, null otherwise</returns>
    public Examinee? GetExamineeForCommittee(string committeeId)
    {
        _logger.LogInformation("Getting examinee for committee {CommitteeId}", committeeId);
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
        _logger.LogInformation("Getting committee for examinee {ExamineeId}", examineeId);
        var committees = _data.GetAllCommittees();
        return committees.FirstOrDefault(c => c.ExamineeId == examineeId);
    }

    // ===== Relationship Management: Examinee <-> Ratings =====

    /// <summary>
    /// Adds a reference to a given ProjectDocumentation object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="documentation"></param>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation)
    {
        _logger.LogInformation("Assigning project documentation {DocumentationId} to examinee {ExamineeId}", documentation.Id, examineeId);
        _data.AddProjectDocumentation(documentation);

        var result = _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.ProjectDocumentationId = documentation.Id;
        });
        if (!result)
        {
            _logger.LogWarning("Examinee {ExamineeId} not found for documentation assignment", examineeId);
            throw new EntityNotFoundException("Examinee", examineeId);
        }
    }

    /// <summary>
    /// Returns a ProjectDocumentation object from a examinee with given id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The ProjectDocumentation object if found and assigned to the examinee, null otherwise</returns>
    public ProjectDocumentation? GetProjectDocumentationForExaminee(string examineeId)
    {
        _logger.LogInformation("Getting project documentation for examinee {ExamineeId}", examineeId);
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.ProjectDocumentationId == null) return null;

        return _data.GetProjectDocumentationById(examinee.ProjectDocumentationId);
    }

    /// <summary>
    /// Adds a reference to a given ProjectPresentation object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="presentation"></param>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignProjectPresentation(string examineeId, ProjectPresentation presentation)
    {
        _logger.LogInformation("Assigning project presentation {PresentationId} to examinee {ExamineeId}", presentation.Id, examineeId);
        _data.AddProjectPresentation(presentation);

        var result = _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.ProjectPresentationId = presentation.Id;
        });
        if (!result)
        {
            _logger.LogWarning("Examinee {ExamineeId} not found for presentation assignment", examineeId);
            throw new EntityNotFoundException("Examinee", examineeId);
        }
    }

    /// <summary>
    /// Returns a ProjectPresentation object from an Examinee with id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The ProjectPresentation object if found and assigned to the examinee, null otherwise</returns>
    public ProjectPresentation? GetProjectPresentationForExaminee(string examineeId)
    {
        _logger.LogInformation("Getting project presentation for examinee {ExamineeId}", examineeId);
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.ProjectPresentationId == null) return null;

        return _data.GetProjectPresentationById(examinee.ProjectPresentationId);
    }

    /// <summary>
    /// Adds a reference to a given TechConversation object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="conversation"></param>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignTechConversation(string examineeId, TechConversation conversation)
    {
        _logger.LogInformation("Assigning tech conversation {ConversationId} to examinee {ExamineeId}", conversation.Id, examineeId);
        _data.AddTechConversation(conversation);

        var result = _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.TechConversationId = conversation.Id;
        });
        if (!result)
        {
            _logger.LogWarning("Examinee {ExamineeId} not found for tech conversation assignment", examineeId);
            throw new EntityNotFoundException("Examinee", examineeId);
        }
    }

    /// <summary>
    /// Returns a TechConversation object from an Examinee with id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The TechConversation object if found and assigned to the examinee, null otherwise</returns>
    public TechConversation? GetTechConversationForExaminee(string examineeId)
    {
        _logger.LogInformation("Getting tech conversation for examinee {ExamineeId}", examineeId);
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.TechConversationId == null) return null;

        return _data.GetTechConversationById(examinee.TechConversationId);
    }

    /// <summary>
    /// Adds a reference to a given SupplementaryExamination object to an Examinee object with an id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <param name="exam"></param>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam)
    {
        _logger.LogInformation("Assigning supplementary examination {ExamId} to examinee {ExamineeId}", exam.Id, examineeId);
        _data.AddSupplementaryExamination(exam);

        var result = _data.UpdateExaminee(examineeId, examinee =>
        {
            examinee.SupplementaryExaminationId = exam.Id;
        });
        if (!result)
        {
            _logger.LogWarning("Examinee {ExamineeId} not found for supplementary examination assignment", examineeId);
            throw new EntityNotFoundException("Examinee", examineeId);
        }
    }

    /// <summary>
    /// Returns a SupplementaryExamination object from an Examinee with id
    /// </summary>
    /// <param name="examineeId"></param>
    /// <returns>The SupplementaryExamination object if found and assigned to the examinee, null otherwise</returns>
    public SupplementaryExamination? GetSupplementaryExaminationForExaminee(string examineeId)
    {
        _logger.LogInformation("Getting supplementary examination for examinee {ExamineeId}", examineeId);
        var examinee = _data.GetExamineeById(examineeId);
        if (examinee?.SupplementaryExaminationId == null) return null;

        return _data.GetSupplementaryExaminationById(examinee.SupplementaryExaminationId);
    }

    /// <summary>
    /// Dispose function to clean up references
    /// </summary>
    public void Dispose()
    {
        _autoDataSaver.Dispose();
        Log.CloseAndFlush();
    }
}