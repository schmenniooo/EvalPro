using System.Text.Json;
using EvalProService.impl.model;
using EvalProService.impl.model.entities;

namespace EvalProService.impl.persistency;

public class ServiceData
{
    private readonly Lock _lock = new();
    private const string ConfigFilePath = "config.json";
    
    private List<AuditCommittee> _committeeslist = [];
    private List<Examinee> _examineeslist = [];
    private List<ProjectDocumentation> _projectDocumentationList = [];
    private List<ProjectPresentation> _projectPresentationList = [];
    private List<TechConversation> _projectTechConversationList = [];
    private List<SupplementaryExamination> _supplementaryExaminationList = [];
    
    // Writes current attribute values into local json files
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
    
    // Reads values from json files
    public void LoadConfigFromJson()
    {
        if (!File.Exists(ConfigFilePath))
        {
            return;
        }

        var jsonString = File.ReadAllText(ConfigFilePath);
        var document = JsonDocument.Parse(jsonString);
        var root = document.RootElement;

        if (root.TryGetProperty("Committees", out JsonElement committeesElement))
        {
            _committeeslist = JsonSerializer.Deserialize<List<AuditCommittee>>(committeesElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("Examinees", out JsonElement examineesElement))
        {
            _examineeslist = JsonSerializer.Deserialize<List<Examinee>>(examineesElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("ProjectDocumentation", out JsonElement projectDocElement))
        {
            _projectDocumentationList = JsonSerializer.Deserialize<List<ProjectDocumentation>>(projectDocElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("ProjectPresentation", out JsonElement projectPresElement))
        {
            _projectPresentationList = JsonSerializer.Deserialize<List<ProjectPresentation>>(projectPresElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("TechConversation", out JsonElement techConvElement))
        {
            _projectTechConversationList = JsonSerializer.Deserialize<List<TechConversation>>(techConvElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("SupplementaryExamination", out JsonElement suppExamElement))
        {
            _supplementaryExaminationList = JsonSerializer.Deserialize<List<SupplementaryExamination>>(suppExamElement.GetRawText()) ?? [];
        }
    }
    
    private object CreateSnapshot()
        {
            lock (_lock)
            {
                return new
                {
                    Committees = _committeeslist.ToList(),  // Create copies
                    Examinees = _examineeslist.ToList(),
                    ProjectDocumentation = _projectDocumentationList.ToList(),
                    ProjectPresentation = _projectPresentationList.ToList(),
                    TechConversation = _projectTechConversationList.ToList(),
                    SupplementaryExamination = _supplementaryExaminationList.ToList()
                };
            }
        }
    
    public List<AuditCommittee> GetCommitteesList()
    {
        return _committeeslist;
    }

    public List<Examinee> GetExamineesList()
    {
        return _examineeslist;
    }

    public List<ProjectDocumentation> GetProjectDocumentationList()
    {
        return _projectDocumentationList;
    }

    public List<ProjectPresentation> GetProjectPresentationList()
    {
        return _projectPresentationList;
    }

    public List<TechConversation> GetProjectTechConversationList()
    {
        return _projectTechConversationList;
    }

    public List<SupplementaryExamination> GetSupplementaryExaminationList()
    {
        return _supplementaryExaminationList;
    }

}