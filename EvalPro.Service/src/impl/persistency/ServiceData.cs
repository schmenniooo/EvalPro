using System.Text.Json;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;

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
    private const string ConfigFilePath = "config.json";
    
    public List<AuditCommittee> CommitteesList = [];
    public List<Examinee> ExamineesList = [];
    public List<ProjectDocumentation> ProjectDocumentationList = [];
    public List<ProjectPresentation> ProjectPresentationList = [];
    public List<TechConversation> ProjectTechConversationList = [];
    public List<SupplementaryExamination> SupplementaryExaminationList = [];

    public ServiceData()
    {
        LoadConfigFromJson();
    }

    /// <summary>
    /// Writes current attribute values into local json files
    /// </summary>
    public void SaveConfigToJson()
    {
        // Capture snapshot while locked (fast)
        var data = CreateSnapshot();

        // Serialize and write to disk without lock (slow I/O)
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var jsonString = JsonSerializer.Serialize(data, options);
        File.WriteAllText(ConfigFilePath, jsonString);
    }
    
    /// <summary>
    /// Reads values from json files
    /// </summary>
    private void LoadConfigFromJson()
    {
        if (!File.Exists(ConfigFilePath))
        {
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
                if (committees != null) CommitteesList = committees;
                if (examinees != null) ExamineesList = examinees;
                if (projectDocs != null) ProjectDocumentationList = projectDocs;
                if (projectPres != null) ProjectPresentationList = projectPres;
                if (techConv != null) ProjectTechConversationList = techConv;
                if (suppExam != null) SupplementaryExaminationList = suppExam;
            }
        }
        catch (JsonException)
        {
            // Logging - malformed JSON, keep existing data
        }
        catch (Exception)
        {
            // Logging - other errors (file access, etc.), keep existing data
        }
    }
    
    /// <summary>
    /// Creates a copy of the current stored data for thread locking
    /// </summary>
    private object CreateSnapshot()
    {
        lock (_lock)
        {
            return new
            {
                // Creating copies from current values
                Committees = CommitteesList.ToList(),
                Examinees = ExamineesList.ToList(),
                ProjectDocumentation = ProjectDocumentationList.ToList(),
                ProjectPresentation = ProjectPresentationList.ToList(),
                TechConversation = ProjectTechConversationList.ToList(),
                SupplementaryExamination = SupplementaryExaminationList.ToList()
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
            CommitteesList.Add(committee);
        }
    }

    /// <summary>
    /// Updates a committee by id with given call backs
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateAction"></param>
    /// <returns></returns>
    public bool UpdateCommittee(string id, Action<AuditCommittee> updateAction)
    {
        lock (_lock)
        {
            var committee = CommitteesList.FirstOrDefault(c => c.Id == id);
            if (committee == null) return false;

            updateAction(committee);
            committee.UpdatedAt = DateTime.Now;
            return true;
        }
    }

    /// <summary>
    /// Searches for committee object with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveCommittee(string id)
    {
        lock (_lock)
        {
            var committee = CommitteesList.FirstOrDefault(c => c.Id == id);
            return committee != null && CommitteesList.Remove(committee);
        }
    }

    /// <summary>
    /// Searches for a committee object with given id and returns it
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AuditCommittee? GetCommitteeById(string id)
    {
        lock (_lock)
        {
            return CommitteesList.FirstOrDefault(c => c.Id == id);
        }
    }

    /// <summary>
    ///  Returns the list of committee's as readonly to manage access on it
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<AuditCommittee> GetAllCommittees()
    {
        lock (_lock)
        {
            return CommitteesList.ToList().AsReadOnly();
        }
    }

    // ----- Examinee Operations -----

    public void AddExaminee(Examinee examinee)
    {
        lock (_lock)
        {
            examinee.CreatedAt = DateTime.Now;
            examinee.UpdatedAt = DateTime.Now;
            ExamineesList.Add(examinee);
        }
    }

    public bool UpdateExaminee(string id, Action<Examinee> updateAction)
    {
        lock (_lock)
        {
            var examinee = ExamineesList.FirstOrDefault(e => e.Id == id);
            if (examinee == null) return false;

            updateAction(examinee);
            examinee.UpdatedAt = DateTime.Now;
            return true;
        }
    }

    public bool RemoveExaminee(string id)
    {
        lock (_lock)
        {
            var examinee = ExamineesList.FirstOrDefault(e => e.Id == id);
            if (examinee == null) return false;

            return ExamineesList.Remove(examinee);
        }
    }

    public Examinee? GetExamineeById(string id)
    {
        lock (_lock)
        {
            return ExamineesList.FirstOrDefault(e => e.Id == id);
        }
    }

    public IReadOnlyList<Examinee> GetAllExaminees()
    {
        lock (_lock)
        {
            return ExamineesList.ToList().AsReadOnly();
        }
    }

    // ----- ProjectDocumentation Operations -----

    public void AddProjectDocumentation(ProjectDocumentation doc)
    {
        lock (_lock)
        {
            doc.CreatedAt = DateTime.Now;
            doc.ModifiedAt = DateTime.Now;
            ProjectDocumentationList.Add(doc);
        }
    }

    public ProjectDocumentation? GetProjectDocumentationById(string id)
    {
        lock (_lock)
        {
            return ProjectDocumentationList.FirstOrDefault(d => d.Id == id);
        }
    }

    // ----- ProjectPresentation Operations -----

    public void AddProjectPresentation(ProjectPresentation presentation)
    {
        lock (_lock)
        {
            presentation.CreatedAt = DateTime.Now;
            presentation.ModifiedAt = DateTime.Now;
            ProjectPresentationList.Add(presentation);
        }
    }

    public ProjectPresentation? GetProjectPresentationById(string id)
    {
        lock (_lock)
        {
            return ProjectPresentationList.FirstOrDefault(p => p.Id == id);
        }
    }

    // ----- TechConversation Operations -----

    public void AddTechConversation(TechConversation conversation)
    {
        lock (_lock)
        {
            conversation.CreatedAt = DateTime.Now;
            conversation.ModifiedAt = DateTime.Now;
            ProjectTechConversationList.Add(conversation);
        }
    }

    public TechConversation? GetTechConversationById(string id)
    {
        lock (_lock)
        {
            return ProjectTechConversationList.FirstOrDefault(t => t.Id == id);
        }
    }

    // ----- SupplementaryExamination Operations -----

    public void AddSupplementaryExamination(SupplementaryExamination exam)
    {
        lock (_lock)
        {
            //exam.ModifiedAt = DateTime.Now;
            //exam.ModifiedAt = DateTime.Now;
            SupplementaryExaminationList.Add(exam);
        }
    }

    public SupplementaryExamination? GetSupplementaryExaminationById(string id)
    {
        lock (_lock)
        {
            //return SupplementaryExaminationList.FirstOrDefault(s => s.Id == id);
        }

        return null;
    }
}