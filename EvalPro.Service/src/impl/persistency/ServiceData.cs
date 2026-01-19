using System.Text.Json;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;
using Microsoft.Extensions.Logging;

namespace EvalProService.impl.persistency;

/// <summary>
/// Data class that stores the data and handles persistency.
/// </summary>
public class ServiceData
{
    /// <summary>
    /// Key to work on this thread with the data.
    /// Used to handle Thread-Safety.
    /// </summary>
    private readonly Lock _lock = new();
    private readonly ILogger? _logger;
    private const string ConfigFilePath = "config.json";
    
    private List<AuditCommittee> _committeesList = [];
    private List<Examinee> _examineesList = [];
    private List<ProjectDocumentation> _projectDocumentationList = [];
    private List<ProjectPresentation> _projectPresentationList = [];
    private List<TechConversation> _projectTechConversationList = [];
    private List<SupplementaryExamination> _supplementaryExaminationList = [];

    public ServiceData(ILogger? logger)
    {
        _logger = logger;
        LoadConfigFromJson();
    }

    /// <summary>
    /// Writes current attribute values into local json files
    /// </summary>
    public void SaveConfigToJson()
    {
        _logger.LogInformation("Saving config to {FilePath}", ConfigFilePath);
        // Capture snapshot while locked (fast)
        var data = CreateSnapshot();

        // Serialize and write to disk without lock (slow I/O)
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

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
            // Read and parse JSON without lock (slow I/O)
            var jsonString = File.ReadAllText(ConfigFilePath);

            // Use 'using' to ensure JsonDocument is properly disposed
            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            // Deserialize all lists outside the lock
            var committees = root.TryGetProperty("Committees", out JsonElement committeesElement)
                ? JsonSerializer.Deserialize<List<AuditCommittee>>(committeesElement.GetRawText()) ?? []
                : null;

            var examinees = root.TryGetProperty("Examinees", out JsonElement examineesElement)
                ? JsonSerializer.Deserialize<List<Examinee>>(examineesElement.GetRawText()) ?? []
                : null;

            var projectDocs = root.TryGetProperty("ProjectDocumentation", out JsonElement projectDocElement)
                ? JsonSerializer.Deserialize<List<ProjectDocumentation>>(projectDocElement.GetRawText()) ?? []
                : null;

            var projectPres = root.TryGetProperty("ProjectPresentation", out JsonElement projectPresElement)
                ? JsonSerializer.Deserialize<List<ProjectPresentation>>(projectPresElement.GetRawText()) ?? []
                : null;

            var techConv = root.TryGetProperty("TechConversation", out JsonElement techConvElement)
                ? JsonSerializer.Deserialize<List<TechConversation>>(techConvElement.GetRawText()) ?? []
                : null;

            var suppExam = root.TryGetProperty("SupplementaryExamination", out JsonElement suppExamElement)
                ? JsonSerializer.Deserialize<List<SupplementaryExamination>>(suppExamElement.GetRawText()) ?? []
                : null;

            // Lock only for the assignment (fast)
            lock (_lock)
            {
                if (committees != null) _committeesList = committees;
                if (examinees != null) _examineesList = examinees;
                if (projectDocs != null) _projectDocumentationList = projectDocs;
                if (projectPres != null) _projectPresentationList = projectPres;
                if (techConv != null) _projectTechConversationList = techConv;
                if (suppExam != null) _supplementaryExaminationList = suppExam;
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
    
    /// <summary>
    /// Creates a copy of the current stored data for thread locking
    /// </summary>
    /// <returns>An anonymous object containing copies of all data lists</returns>
    private object CreateSnapshot()
    {
        lock (_lock)
        {
            return new
            {
                // Creating copies from current values
                Committees = _committeesList.ToList(),
                Examinees = _examineesList.ToList(),
                ProjectDocumentation = _projectDocumentationList.ToList(),
                ProjectPresentation = _projectPresentationList.ToList(),
                TechConversation = _projectTechConversationList.ToList(),
                SupplementaryExamination = _supplementaryExaminationList.ToList()
            };
        }
    }

    // ===== Thread-Safe CRUD Operations =====

    // ----- Committee Operations -----

    /// <summary>
    /// Adds a committee object to the list
    /// </summary>
    /// <param name="committee"></param>
    public void AddCommittee(AuditCommittee committee)
    {
        lock (_lock)
        {
            committee.CreatedAt = DateTime.Now;
            committee.UpdatedAt = DateTime.Now;
            _committeesList.Add(committee);
            _logger.LogInformation("Added committee {Id} to data store", committee.Id);
        }
    }

    /// <summary>
    /// Updates a committee by id with given call backs
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateAction"></param>
    /// <returns>True if the committee was found and updated, false if not found</returns>
    public bool UpdateCommittee(string id, Action<AuditCommittee> updateAction)
    {
        lock (_lock)
        {
            var committee = _committeesList.FirstOrDefault(c => c.Id == id);
            if (committee == null)
            {
                _logger.LogWarning("Committee {Id} not found in data store", id);
                return false;
            }

            updateAction(committee);
            committee.UpdatedAt = DateTime.Now;
            _logger.LogInformation("Updated committee {Id} in data store", id);
            return true;
        }
    }

    /// <summary>
    /// Searches for committee object with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True if the committee was found and removed, false if not found</returns>
    public bool RemoveCommittee(string id)
    {
        lock (_lock)
        {
            var committee = _committeesList.FirstOrDefault(c => c.Id == id);
            if (committee == null)
            {
                _logger.LogWarning("Committee {Id} not found in data store for removal", id);
                return false;
            }
            _committeesList.Remove(committee);
            _logger.LogInformation("Removed committee {Id} from data store", id);
            return true;
        }
    }

    /// <summary>
    /// Searches for a committee object with given id and returns it
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The AuditCommittee object if found, null otherwise</returns>
    public AuditCommittee? GetCommitteeById(string id)
    {
        lock (_lock)
        {
            return _committeesList.FirstOrDefault(c => c.Id == id);
        }
    }

    /// <summary>
    ///  Returns the list of committee's as readonly to manage access on it
    /// </summary>
    /// <returns>A readonly list containing copies of all AuditCommittee objects</returns>
    public IReadOnlyList<AuditCommittee> GetAllCommittees()
    {
        lock (_lock)
        {
            return _committeesList.ToList().AsReadOnly();
        }
    }

    // ----- Examinee Operations -----

    /// <summary>
    /// Adds a given Examinee object to it's list
    /// </summary>
    /// <param name="examinee"></param>
    public void AddExaminee(Examinee examinee)
    {
        lock (_lock)
        {
            examinee.CreatedAt = DateTime.Now;
            examinee.UpdatedAt = DateTime.Now;
            _examineesList.Add(examinee);
            _logger.LogInformation("Added examinee {Id} to data store", examinee.Id);
        }
    }

    /// <summary>
    /// Searches for examinee object with given callbacks
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateAction"></param>
    /// <returns>True if the examinee was found and updated, false if not found</returns>
    public bool UpdateExaminee(string id, Action<Examinee> updateAction)
    {
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == id);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {Id} not found in data store", id);
                return false;
            }

            updateAction(examinee);
            examinee.UpdatedAt = DateTime.Now;
            _logger.LogInformation("Updated examinee {Id} in data store", id);
            return true;
        }
    }

    /// <summary>
    /// Searches for Examinee object with given id and removes it
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True if the examinee was found and removed, false if not found</returns>
    public bool RemoveExaminee(string id)
    {
        lock (_lock)
        {
            var examinee = _examineesList.FirstOrDefault(e => e.Id == id);
            if (examinee == null)
            {
                _logger.LogWarning("Examinee {Id} not found in data store for removal", id);
                return false;
            }
            _examineesList.Remove(examinee);
            _logger.LogInformation("Removed examinee {Id} from data store", id);
            return true;
        }
    }

    /// <summary>
    /// Searches for examinee object by given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The Examinee object if found, null otherwise</returns>
    public Examinee? GetExamineeById(string id)
    {
        lock (_lock)
        {
            return _examineesList.FirstOrDefault(e => e.Id == id);
        }
    }

    /// <summary>
    /// Returns examinee list as readonly list
    /// </summary>
    /// <returns>A readonly list containing copies of all Examinee objects</returns>
    public IReadOnlyList<Examinee> GetAllExaminees()
    {
        lock (_lock)
        {
            return _examineesList.ToList().AsReadOnly();
        }
    }

    // ----- ProjectDocumentation Operations -----

    /// <summary>
    /// Adds given ProjectDocumentation object to the list
    /// </summary>
    /// <param name="doc"></param>
    public void AddProjectDocumentation(ProjectDocumentation doc)
    {
        lock (_lock)
        {
            doc.CreatedAt = DateTime.Now;
            doc.UpdatedAt = DateTime.Now;
            _projectDocumentationList.Add(doc);
            _logger.LogInformation("Added project documentation {Id} to data store", doc.Id);
        }
    }

    /// <summary>
    /// Returns ProjectDocumentation object if found by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The ProjectDocumentation object if found, null otherwise</returns>
    public ProjectDocumentation? GetProjectDocumentationById(string id)
    {
        lock (_lock)
        {
            return _projectDocumentationList.FirstOrDefault(d => d.Id == id);
        }
    }

    // ----- ProjectPresentation Operations -----

    /// <summary>
    /// Adds given ProjectPresentation object to the list
    /// </summary>
    /// <param name="presentation"></param>
    public void AddProjectPresentation(ProjectPresentation presentation)
    {
        lock (_lock)
        {
            presentation.CreatedAt = DateTime.Now;
            presentation.UpdatedAt = DateTime.Now;
            _projectPresentationList.Add(presentation);
            _logger.LogInformation("Added project presentation {Id} to data store", presentation.Id);
        }
    }

    /// <summary>
    /// Returns ProjectPresentation object if found by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The ProjectPresentation object if found, null otherwise</returns>
    public ProjectPresentation? GetProjectPresentationById(string id)
    {
        lock (_lock)
        {
            return _projectPresentationList.FirstOrDefault(p => p.Id == id);
        }
    }

    // ----- TechConversation Operations -----

    /// <summary>
    /// Adds a given TechConversation object to the list
    /// </summary>
    /// <param name="conversation"></param>
    public void AddTechConversation(TechConversation conversation)
    {
        lock (_lock)
        {
            conversation.CreatedAt = DateTime.Now;
            conversation.UpdatedAt = DateTime.Now;
            _projectTechConversationList.Add(conversation);
            _logger.LogInformation("Added tech conversation {Id} to data store", conversation.Id);
        }
    }

    /// <summary>
    /// Returns a TechConversation object if found by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The TechConversation object if found, null otherwise</returns>
    public TechConversation? GetTechConversationById(string id)
    {
        lock (_lock)
        {
            return _projectTechConversationList.FirstOrDefault(t => t.Id == id);
        }
    }

    // ----- SupplementaryExamination Operations -----

    /// <summary>
    /// Adds a given SupplementaryExamination object to the list
    /// </summary>
    /// <param name="exam"></param>
    public void AddSupplementaryExamination(SupplementaryExamination exam)
    {
        lock (_lock)
        {
            exam.CreatedAt = DateTime.Now;
            exam.UpdatedAt = DateTime.Now;
            _supplementaryExaminationList.Add(exam);
            _logger.LogInformation("Added supplementary examination {Id} to data store", exam.Id);
        }
    }

    /// <summary>
    /// Returns a SupplementaryExamination object if found by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The SupplementaryExamination object if found, null otherwise</returns>
    public SupplementaryExamination? GetSupplementaryExaminationById(string id)
    {
        lock (_lock)
        {
            return _supplementaryExaminationList.FirstOrDefault(s => s.Id == id);
        }
    }
}