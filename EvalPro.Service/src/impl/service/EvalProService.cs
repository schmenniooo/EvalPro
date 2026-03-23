using System.Text.Json;
using EvalProService.api;
using EvalProService.impl.exceptions;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.events;
using EvalProService.impl.model.ratings;
using EvalProService.impl.persistency.autoSaver;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EvalProService.impl.service;

/// <summary>
/// Service layer that manages business logic, data storage, and persistence.
/// Frontend interacts only with this class.
/// </summary>
public class EvalProService : IEvalProServiceApi, IDisposable
{
    private readonly Lock _lock = new();
    private readonly AutoDataSaver _autoDataSaver;
    private readonly ILogger<EvalProService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private const string ConfigFilePath = "config.json";

    private List<AuditCommittee> _committeesList = [];
    private List<Examinee> _examineesList = [];

    /// <summary>
    /// Event raised when auto-save fails. UI can subscribe to show warnings to the user.
    /// Check IsCritical property to determine if this was a final save attempt.
    /// </summary>
    public event EventHandler<AutoSaveErrorEventArgs>? OnSaveError;

    /// <summary>
    /// Initializes the service: configures logging, loads persisted data, and starts auto-save.
    /// </summary>
    public EvalProService()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("evalpro.log", rollingInterval: RollingInterval.Hour)
            .CreateLogger();

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog();
        });
        _logger = _loggerFactory.CreateLogger<EvalProService>();

        LoadConfigFromJson();
        _autoDataSaver = new AutoDataSaver(SaveConfigToJson, _logger);
        _autoDataSaver.OnSaveError += (_, args) => OnSaveError?.Invoke(this, args);
        _autoDataSaver.StartAutoSaveTimer();
    }

    // ===== Persistence =====

    /// <summary>
    /// Writes current attribute values into local json files
    /// </summary>
    public void SaveConfigToJson()
    {
        _logger.LogInformation("Saving config to {FilePath}", ConfigFilePath);
        object data;
        lock (_lock)
        {
            data = new
            {
                Committees = _committeesList.ToList(),
                Examinees = _examineesList.ToList()
            };
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(data, options);
        File.WriteAllText(ConfigFilePath, jsonString);
        _logger.LogInformation("Config saved successfully");
    }

    /// <summary>
    /// Reads values from json files
    /// </summary>
    private void LoadConfigFromJson()
    {
        _logger.LogInformation("Loading config from {FilePath}", ConfigFilePath);
        if (!File.Exists(ConfigFilePath))
        {
            _logger.LogInformation("Config file not found, creating new file");
            File.Create(ConfigFilePath).Dispose();
            return;
        }

        try
        {
            var jsonString = File.ReadAllText(ConfigFilePath);
            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            var committees = root.TryGetProperty("Committees", out JsonElement committeesElement)
                ? JsonSerializer.Deserialize<List<AuditCommittee>>(committeesElement.GetRawText()) ?? []
                : null;

            var examinees = root.TryGetProperty("Examinees", out JsonElement examineesElement)
                ? JsonSerializer.Deserialize<List<Examinee>>(examineesElement.GetRawText()) ?? []
                : null;

            lock (_lock)
            {
                if (committees != null) _committeesList = committees;
                if (examinees != null) _examineesList = examinees;
            }
            _logger.LogInformation("Config loaded successfully: {CommitteeCount} committees, {ExamineeCount} examinees",
                _committeesList.Count, _examineesList.Count);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse config file - malformed JSON");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load config file");
        }
    }

    // ===== Committee Operations =====

    /// <summary>
    /// Creates a new committee object with given parameters
    /// </summary>
    public AuditCommittee AddCommittee(string designation, string apprenticeShip, List<DateTime> testDates)
    {
        _logger.LogInformation("Creating committee with: {Designation}, {ApprenticeShip}, {TestDates}", designation, apprenticeShip, testDates);
        var committee = new AuditCommittee(designation, apprenticeShip, testDates);
        lock (_lock)
        {
            committee.CreatedAt = DateTime.Now;
            committee.UpdatedAt = DateTime.Now;
            _committeesList.Add(committee);
        }
        return committee;
    }

    /// <summary>
    /// Updates a committee object by id
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when committee with given ID is not found</exception>
    public void UpdateCommittee(string id, string? designation = null, string? apprenticeShip = null, List<DateTime>? testDates = null)
    {
        _logger.LogInformation("Updating committee {Id}", id);
        lock (_lock)
        {
            var committee = _committeesList.FirstOrDefault(c => c.Id == id);
            if (committee == null)
            {
                _logger.LogWarning("Committee {Id} not found for update", id);
                throw new EntityNotFoundException("Committee", id);
            }

            if (designation != null) committee.Designation = designation;
            if (apprenticeShip != null) committee.ApprenticeShip = apprenticeShip;
            if (testDates != null) committee.TestDates = testDates;
            committee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Deletes a committee object
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when committee with given ID is not found</exception>
    public void RemoveCommittee(string id)
    {
        _logger.LogInformation("Removing committee {Id}", id);
        lock (_lock)
        {
            var committee = _committeesList.FirstOrDefault(c => c.Id == id);
            if (committee == null)
            {
                _logger.LogWarning("Committee {Id} not found for removal", id);
                throw new EntityNotFoundException("Committee", id);
            }
            _committeesList.Remove(committee);
        }
    }

    /// <summary>
    /// Returns a committee object found with given id
    /// </summary>
    public AuditCommittee? GetCommitteeById(string id)
    {
        _logger.LogInformation("Getting committee by id {Id}", id);
        lock (_lock)
        {
            return _committeesList.FirstOrDefault(c => c.Id == id);
        }
    }

    /// <summary>
    /// Returns a readonly list of committee's to avoid unmanaged access on it
    /// </summary>
    public IReadOnlyList<AuditCommittee> GetAllCommittees()
    {
        _logger.LogInformation("Getting all committees");
        lock (_lock)
        {
            return _committeesList.ToList().AsReadOnly();
        }
    }

    // ===== Examinee Operations =====

    /// <summary>
    /// Creates a new Examinee object with given parameters
    /// </summary>
    public Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle)
    {
        _logger.LogInformation("Creating examinee: {Name}, {Company}, {ContactPerson}, {ProjectTitle}", name, company, contactPerson, projectTitle);
        var examinee = new Examinee(name, company, contactPerson, projectTitle);
        lock (_lock)
        {
            examinee.CreatedAt = DateTime.Now;
            examinee.UpdatedAt = DateTime.Now;
            _examineesList.Add(examinee);
        }
        return examinee;
    }

    /// <summary>
    /// Updates an examinee object by given parameters
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee with given ID is not found</exception>
    public void UpdateExaminee(string id, string? name = null, string? company = null, string? contactPerson = null, string? projectTitle = null)
    {
        _logger.LogInformation("Updating examinee {Id}", id);
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == id);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {Id} not found for update", id);
                throw new EntityNotFoundException("Examinee", id);
            }

            if (name != null) examinee.Name = name;
            if (company != null) examinee.Company = company;
            if (contactPerson != null) examinee.ContactPerson = contactPerson;
            if (projectTitle != null) examinee.ProjectTitle = projectTitle;
            examinee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Removes an examinee and clears any committee references to it
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee with given ID is not found</exception>
    public void RemoveExaminee(string id)
    {
        _logger.LogInformation("Removing examinee {Id}", id);
        lock (_lock)
        {
            foreach (var committee in _committeesList)
            {
                if (committee.Examinee?.Id == id)
                {
                    _logger.LogInformation("Removing examinee {ExamineeId} reference from committee {CommitteeId}", id, committee.Id);
                    committee.Examinee = null;
                    committee.UpdatedAt = DateTime.Now;
                }
            }

            var examinee = _examineesList.FirstOrDefault(e => e.Id == id);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {Id} not found for removal", id);
                throw new EntityNotFoundException("Examinee", id);
            }
            _examineesList.Remove(examinee);
        }
    }

    /// <summary>
    /// Returns an examinee object found with given id
    /// </summary>
    public Examinee? GetExamineeById(string id)
    {
        _logger.LogInformation("Getting examinee by id {Id}", id);
        lock (_lock)
        {
            return _examineesList.FirstOrDefault(e => e.Id == id);
        }
    }

    /// <summary>
    /// Returns examinee list as readonly to avoid unmanaged access
    /// </summary>
    public IReadOnlyList<Examinee> GetAllExaminees()
    {
        _logger.LogInformation("Getting all examinees");
        lock (_lock)
        {
            return _examineesList.ToList().AsReadOnly();
        }
    }

    // ===== Relationship Management: Committee <-> Examinee =====

    /// <summary>
    /// Connects an examinee to a committee by setting a direct reference
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee or committee is not found</exception>
    public void AssignExamineeToCommittee(string committeeId, string examineeId)
    {
        _logger.LogInformation("Assigning examinee {ExamineeId} to committee {CommitteeId}", examineeId, committeeId);
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == examineeId);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {ExamineeId} not found for assignment", examineeId);
                throw new EntityNotFoundException("Examinee", examineeId);
            }

            var committee = _committeesList.FirstOrDefault(c => c.Id == committeeId);
            if (committee == null)
            {
                _logger.LogWarning("Committee {CommitteeId} not found for assignment", committeeId);
                throw new EntityNotFoundException("Committee", committeeId);
            }

            committee.Examinee = examinee;
            committee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Removes the examinee from a committee
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when committee is not found</exception>
    public void RemoveExamineeFromCommittee(string committeeId)
    {
        _logger.LogInformation("Removing examinee from committee {CommitteeId}", committeeId);
        lock (_lock)
        {
            var committee = _committeesList.FirstOrDefault(c => c.Id == committeeId);
            if (committee == null)
            {
                _logger.LogWarning("Committee {CommitteeId} not found", committeeId);
                throw new EntityNotFoundException("Committee", committeeId);
            }

            committee.Examinee = null;
            committee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Gets the examinee assigned to a committee
    /// </summary>
    public Examinee? GetExamineeForCommittee(string committeeId)
    {
        _logger.LogInformation("Getting examinee for committee {CommitteeId}", committeeId);
        lock (_lock)
        {
            var committee = _committeesList.FirstOrDefault(c => c.Id == committeeId);
            return committee?.Examinee;
        }
    }

    /// <summary>
    /// Finds which committee an examinee belongs to
    /// </summary>
    public AuditCommittee? GetCommitteeForExaminee(string examineeId)
    {
        _logger.LogInformation("Getting committee for examinee {ExamineeId}", examineeId);
        lock (_lock)
        {
            return _committeesList.FirstOrDefault(c => c.Examinee?.Id == examineeId);
        }
    }

    // ===== Rating Assignment =====

    /// <summary>
    /// Assigns a ProjectDocumentation rating to an examinee
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignProjectDocumentation(string examineeId, ProjectDocumentation documentation)
    {
        _logger.LogInformation("Assigning project documentation to examinee {ExamineeId}", examineeId);
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == examineeId);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {ExamineeId} not found for documentation assignment", examineeId);
                throw new EntityNotFoundException("Examinee", examineeId);
            }

            examinee.ProjectDocumentation = documentation;
            examinee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Assigns a ProjectPresentation rating to an examinee
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignProjectPresentation(string examineeId, ProjectPresentation presentation)
    {
        _logger.LogInformation("Assigning project presentation to examinee {ExamineeId}", examineeId);
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == examineeId);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {ExamineeId} not found for presentation assignment", examineeId);
                throw new EntityNotFoundException("Examinee", examineeId);
            }

            examinee.ProjectPresentation = presentation;
            examinee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Assigns a TechConversation rating to an examinee
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignTechConversation(string examineeId, TechConversation conversation)
    {
        _logger.LogInformation("Assigning tech conversation to examinee {ExamineeId}", examineeId);
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == examineeId);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {ExamineeId} not found for tech conversation assignment", examineeId);
                throw new EntityNotFoundException("Examinee", examineeId);
            }

            examinee.TechConversation = conversation;
            examinee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Assigns a SupplementaryExamination to an examinee
    /// </summary>
    /// <exception cref="EntityNotFoundException">Thrown when examinee is not found</exception>
    public void AssignSupplementaryExamination(string examineeId, SupplementaryExamination exam)
    {
        _logger.LogInformation("Assigning supplementary examination to examinee {ExamineeId}", examineeId);
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == examineeId);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {ExamineeId} not found for supplementary examination assignment", examineeId);
                throw new EntityNotFoundException("Examinee", examineeId);
            }

            examinee.SupplementaryExamination = exam;
            examinee.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Dispose function to clean up references
    /// </summary>
    public void Dispose()
    {
        _autoDataSaver.Dispose();
        _loggerFactory.Dispose();
        Log.CloseAndFlush();
    }
}
