using System.Text.Json;
using EvalProService.impl.model;
using EvalProService.impl.model.entities;

namespace EvalProService.impl.persistency;

public class ServiceData
{
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

    /**
     * Writes current attribute values into local json files
     */
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
    
    /**
     * Reads values from json files
     */
    private void LoadConfigFromJson()
    {
        if (!File.Exists(ConfigFilePath))
        {
            File.Create(ConfigFilePath);
            return;
        }

        // Read and parse JSON without lock (slow I/O)
        var jsonString = File.ReadAllText(ConfigFilePath);
        var document = JsonDocument.Parse(jsonString);
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
    
    /**
     * Creates a copy of the current stored data for thread locking
     */
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
    
}