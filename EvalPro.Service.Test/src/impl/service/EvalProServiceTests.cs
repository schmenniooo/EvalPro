using EvalProService.impl.exceptions;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;
using Xunit;
using ServiceClass = EvalProService.impl.service.EvalProService;

namespace EvalProServiceTest.impl.service;

public class EvalProServiceTests : IDisposable
{
    private readonly ServiceClass _service;
    private static readonly DateTime FixedTestDate = new(2026, 6, 15, 10, 30, 0);

    public EvalProServiceTests()
    {
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        _service = new ServiceClass();
    }

    // ===== Committee CRUD Tests =====

    [Fact]
    public void AddCommittee_ReturnsNewCommittee()
    {
        var committee = _service.AddCommittee("Test Committee", "Software Development", new List<DateTime> { FixedTestDate });

        Assert.NotNull(committee);
        Assert.Equal("Test Committee", committee.Designation);
        Assert.Equal("Software Development", committee.ApprenticeShip);
        Assert.Single(committee.TestDates);
        Assert.Contains(FixedTestDate, committee.TestDates);
    }

    [Fact]
    public void GetCommitteeById_ReturnsCommittee_WhenExists()
    {
        var created = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });
        var retrieved = _service.GetCommitteeById(created.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal("Test Committee", retrieved.Designation);
    }

    [Fact]
    public void GetCommitteeById_ReturnsNull_WhenNotExists()
    {
        Assert.Null(_service.GetCommitteeById("non-existent-id"));
    }

    [Fact]
    public void GetAllCommittees_ReturnsEmptyList_Initially()
    {
        var committees = _service.GetAllCommittees();
        Assert.NotNull(committees);
        Assert.Empty(committees);
    }

    [Fact]
    public void GetAllCommittees_ReturnsAllCommittees()
    {
        _service.AddCommittee("Committee 1", "IT", new List<DateTime> { FixedTestDate });
        _service.AddCommittee("Committee 2", "Development", new List<DateTime> { FixedTestDate.AddDays(1) });
        _service.AddCommittee("Committee 3", "Testing", new List<DateTime> { FixedTestDate.AddDays(2) });

        Assert.Equal(3, _service.GetAllCommittees().Count);
    }

    [Fact]
    public void UpdateCommittee_UpdatesDesignation()
    {
        var committee = _service.AddCommittee("Original", "IT", new List<DateTime> { FixedTestDate });
        _service.UpdateCommittee(committee.Id, designation: "Updated");

        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Designation);
        Assert.Equal("IT", updated.ApprenticeShip);
    }

    [Fact]
    public void UpdateCommittee_UpdatesApprenticeShip()
    {
        var committee = _service.AddCommittee("Test", "Original", new List<DateTime> { FixedTestDate });
        _service.UpdateCommittee(committee.Id, apprenticeShip: "Updated");

        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.ApprenticeShip);
    }

    [Fact]
    public void UpdateCommittee_UpdatesTestDates()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var newDates = new List<DateTime> { FixedTestDate.AddDays(10), FixedTestDate.AddDays(20) };
        _service.UpdateCommittee(committee.Id, testDates: newDates);

        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Equal(2, updated.TestDates.Count);
    }

    [Fact]
    public void UpdateCommittee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.UpdateCommittee("non-existent-id", designation: "Test"));
        Assert.Equal("Committee", ex.EntityType);
    }

    [Fact]
    public void RemoveCommittee_RemovesCommittee()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _service.RemoveCommittee(committee.Id);
        Assert.Null(_service.GetCommitteeById(committee.Id));
    }

    [Fact]
    public void RemoveCommittee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        var ex = Assert.Throws<EntityNotFoundException>(() => _service.RemoveCommittee("non-existent-id"));
        Assert.Equal("Committee", ex.EntityType);
    }

    // ===== Examinee CRUD Tests =====

    [Fact]
    public void AddExaminee_ReturnsNewExaminee()
    {
        var examinee = _service.AddExaminee("John Doe", "Tech Corp", "Jane Smith", "Cloud Migration");

        Assert.NotNull(examinee);
        Assert.Equal("John Doe", examinee.Name);
        Assert.Equal("Tech Corp", examinee.Company);
        Assert.Equal("Jane Smith", examinee.ContactPerson);
        Assert.Equal("Cloud Migration", examinee.ProjectTitle);
    }

    [Fact]
    public void GetExamineeById_ReturnsExaminee_WhenExists()
    {
        var created = _service.AddExaminee("John Doe", "Tech Corp", "Jane Smith", "Cloud Migration");
        var retrieved = _service.GetExamineeById(created.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
    }

    [Fact]
    public void GetExamineeById_ReturnsNull_WhenNotExists()
    {
        Assert.Null(_service.GetExamineeById("non-existent-id"));
    }

    [Fact]
    public void GetAllExaminees_ReturnsEmptyList_Initially()
    {
        Assert.Empty(_service.GetAllExaminees());
    }

    [Fact]
    public void GetAllExaminees_ReturnsAllExaminees()
    {
        _service.AddExaminee("John", "A", "A", "A");
        _service.AddExaminee("Jane", "B", "B", "B");
        Assert.Equal(2, _service.GetAllExaminees().Count);
    }

    [Fact]
    public void UpdateExaminee_UpdatesName()
    {
        var examinee = _service.AddExaminee("Original", "Company", "Contact", "Project");
        _service.UpdateExaminee(examinee.Id, name: "Updated");

        Assert.Equal("Updated", _service.GetExamineeById(examinee.Id)!.Name);
    }

    [Fact]
    public void UpdateExaminee_UpdatesCompany()
    {
        var examinee = _service.AddExaminee("Name", "Original", "Contact", "Project");
        _service.UpdateExaminee(examinee.Id, company: "Updated");

        Assert.Equal("Updated", _service.GetExamineeById(examinee.Id)!.Company);
    }

    [Fact]
    public void UpdateExaminee_UpdatesContactPerson()
    {
        var examinee = _service.AddExaminee("Name", "Company", "Original", "Project");
        _service.UpdateExaminee(examinee.Id, contactPerson: "Updated");

        Assert.Equal("Updated", _service.GetExamineeById(examinee.Id)!.ContactPerson);
    }

    [Fact]
    public void UpdateExaminee_UpdatesProjectTitle()
    {
        var examinee = _service.AddExaminee("Name", "Company", "Contact", "Original");
        _service.UpdateExaminee(examinee.Id, projectTitle: "Updated");

        Assert.Equal("Updated", _service.GetExamineeById(examinee.Id)!.ProjectTitle);
    }

    [Fact]
    public void UpdateExaminee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.UpdateExaminee("non-existent-id", name: "Test"));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void RemoveExaminee_RemovesExaminee()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        _service.RemoveExaminee(examinee.Id);
        Assert.Null(_service.GetExamineeById(examinee.Id));
    }

    [Fact]
    public void RemoveExaminee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        var ex = Assert.Throws<EntityNotFoundException>(() => _service.RemoveExaminee("non-existent-id"));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void RemoveExaminee_RemovesReferenceFromCommittee()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        _service.RemoveExaminee(examinee.Id);

        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Null(updated.Examinee);
    }

    // ===== Committee <-> Examinee Relationship Tests =====

    [Fact]
    public void AssignExamineeToCommittee_AssignsSuccessfully()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");

        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated?.Examinee);
        Assert.Equal(examinee.Id, updated.Examinee.Id);
    }

    [Fact]
    public void AssignExamineeToCommittee_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignExamineeToCommittee(committee.Id, "non-existent-examinee"));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void AssignExamineeToCommittee_ThrowsEntityNotFoundException_WhenCommitteeNotExists()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignExamineeToCommittee("non-existent-committee", examinee.Id));
        Assert.Equal("Committee", ex.EntityType);
    }

    [Fact]
    public void RemoveExamineeFromCommittee_RemovesReference()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        _service.RemoveExamineeFromCommittee(committee.Id);

        Assert.Null(_service.GetCommitteeById(committee.Id)?.Examinee);
    }

    [Fact]
    public void RemoveExamineeFromCommittee_ThrowsEntityNotFoundException_WhenCommitteeNotExists()
    {
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.RemoveExamineeFromCommittee("non-existent-committee"));
        Assert.Equal("Committee", ex.EntityType);
    }

    [Fact]
    public void GetExamineeForCommittee_ReturnsExaminee_WhenAssigned()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        var result = _service.GetExamineeForCommittee(committee.Id);
        Assert.NotNull(result);
        Assert.Equal(examinee.Id, result.Id);
    }

    [Fact]
    public void GetExamineeForCommittee_ReturnsNull_WhenNotAssigned()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        Assert.Null(_service.GetExamineeForCommittee(committee.Id));
    }

    [Fact]
    public void GetExamineeForCommittee_ReturnsNull_WhenCommitteeNotExists()
    {
        Assert.Null(_service.GetExamineeForCommittee("non-existent-committee"));
    }

    [Fact]
    public void GetCommitteeForExaminee_ReturnsCommittee_WhenAssigned()
    {
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        var result = _service.GetCommitteeForExaminee(examinee.Id);
        Assert.NotNull(result);
        Assert.Equal(committee.Id, result.Id);
    }

    [Fact]
    public void GetCommitteeForExaminee_ReturnsNull_WhenNotAssigned()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        Assert.Null(_service.GetCommitteeForExaminee(examinee.Id));
    }

    // ===== Rating Assignment Tests =====

    [Fact]
    public void AssignProjectDocumentation_AssignsSuccessfully()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        var doc = new ProjectDocumentation(
            "Good work",
            new Dictionary<string, int> { { "Quality", 8 }, { "Completeness", 9 } },
            new Dictionary<string, string> { { "Quality", "Well structured" } }
        );

        _service.AssignProjectDocumentation(examinee.Id, doc);

        var result = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(result?.ProjectDocumentation);
        Assert.Equal("Good work", result.ProjectDocumentation.FinalComment);
    }

    [Fact]
    public void AssignProjectDocumentation_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        var doc = new ProjectDocumentation("Good", new Dictionary<string, int>(), new Dictionary<string, string>());
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignProjectDocumentation("non-existent-id", doc));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void AssignProjectPresentation_AssignsSuccessfully()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        var pres = new ProjectPresentation(
            "Excellent",
            new Dictionary<string, int> { { "Clarity", 9 } },
            new Dictionary<string, string> { { "Clarity", "Very clear" } }
        );

        _service.AssignProjectPresentation(examinee.Id, pres);

        var result = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(result?.ProjectPresentation);
        Assert.Equal("Excellent", result.ProjectPresentation.FinalComment);
    }

    [Fact]
    public void AssignProjectPresentation_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        var pres = new ProjectPresentation("Good", new Dictionary<string, int>(), new Dictionary<string, string>());
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignProjectPresentation("non-existent-id", pres));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void AssignTechConversation_AssignsSuccessfully()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        var conv = new TechConversation(
            "Solid knowledge",
            new Dictionary<string, int> { { "Depth", 8 } },
            new Dictionary<string, string> { { "Depth", "Good understanding" } }
        );

        _service.AssignTechConversation(examinee.Id, conv);

        var result = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(result?.TechConversation);
        Assert.Equal("Solid knowledge", result.TechConversation.FinalComment);
    }

    [Fact]
    public void AssignTechConversation_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        var conv = new TechConversation("Good", new Dictionary<string, int>(), new Dictionary<string, string>());
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignTechConversation("non-existent-id", conv));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void AssignSupplementaryExamination_AssignsSuccessfully()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");
        var exam = new SupplementaryExamination("Networking", 85, new List<string> { "What is TCP/IP?" });

        _service.AssignSupplementaryExamination(examinee.Id, exam);

        var result = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(result?.SupplementaryExamination);
        Assert.Equal("Networking", result.SupplementaryExamination.ChosenTestArea);
        Assert.Equal(85, result.SupplementaryExamination.Points);
    }

    [Fact]
    public void AssignSupplementaryExamination_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        var exam = new SupplementaryExamination("Networking", 70, new List<string> { "Q1" });
        var ex = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignSupplementaryExamination("non-existent-id", exam));
        Assert.Equal("Examinee", ex.EntityType);
    }

    [Fact]
    public void ExamineeRatings_AreNull_ByDefault()
    {
        var examinee = _service.AddExaminee("John", "Company", "Contact", "Project");

        Assert.Null(examinee.ProjectDocumentation);
        Assert.Null(examinee.ProjectPresentation);
        Assert.Null(examinee.TechConversation);
        Assert.Null(examinee.SupplementaryExamination);
    }

    // ===== Persistence Tests =====

    [Fact]
    public void SaveConfigToJson_CreatesJsonFile()
    {
        _service.AddCommittee("Test Committee", "Software Development", new List<DateTime> { FixedTestDate });
        _service.SaveConfigToJson();

        Assert.True(File.Exists("config.json"));
        var content = File.ReadAllText("config.json");
        Assert.Contains("Test Committee", content);
    }

    [Fact]
    public void SaveConfigToJson_HandlesEmptyLists()
    {
        _service.SaveConfigToJson();

        var newService = new ServiceClass();
        Assert.Empty(newService.GetAllCommittees());
        Assert.Empty(newService.GetAllExaminees());
        newService.Dispose();
    }

    [Fact]
    public void SaveAndLoad_PreservesAuditCommitteeData()
    {
        _service.AddCommittee("Backend Team", "Application Development", new List<DateTime> { FixedTestDate, FixedTestDate.AddDays(5) });
        _service.SaveConfigToJson();

        var newService = new ServiceClass();
        var committees = newService.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Backend Team", committees[0].Designation);
        Assert.Equal(2, committees[0].TestDates.Count);
        newService.Dispose();
    }

    [Fact]
    public void SaveAndLoad_PreservesMultipleCommittees()
    {
        _service.AddCommittee("A", "IT", new List<DateTime> { FixedTestDate });
        _service.AddCommittee("B", "Dev", new List<DateTime> { FixedTestDate.AddDays(1) });
        _service.AddCommittee("C", "Test", new List<DateTime> { FixedTestDate.AddDays(2) });
        _service.SaveConfigToJson();

        var newService = new ServiceClass();
        Assert.Equal(3, newService.GetAllCommittees().Count);
        newService.Dispose();
    }

    [Fact]
    public void SaveConfigToJson_WithConcurrentSaves_CompletesSuccessfully()
    {
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        var tasks = new List<Task>();
        var errors = 0;
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try { _service.SaveConfigToJson(); }
                catch { Interlocked.Increment(ref errors); }
            }));
        }

        #pragma warning disable xUnit1031
        Task.WaitAll(tasks.ToArray());
        #pragma warning restore xUnit1031

        Assert.Equal(0, errors);
    }

    [Fact]
    public void LoadConfigFromJson_WithMalformedJson_HandlesGracefully()
    {
        File.WriteAllText("config.json", "{ not valid }");
        var newService = new ServiceClass();
        Assert.Empty(newService.GetAllCommittees());
        newService.Dispose();
    }

    [Fact]
    public void LoadConfigFromJson_WithEmptyFile_HandlesGracefully()
    {
        File.WriteAllText("config.json", "");
        var newService = new ServiceClass();
        Assert.Empty(newService.GetAllCommittees());
        newService.Dispose();
    }

    [Fact]
    public void LoadConfigFromJson_WithEmptyJson_LoadsEmptyLists()
    {
        File.WriteAllText("config.json", "{}");
        var newService = new ServiceClass();
        Assert.Empty(newService.GetAllCommittees());
        newService.Dispose();
    }

    [Fact]
    public void SaveConfigToJson_CreatesReadableJsonFormat()
    {
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _service.SaveConfigToJson();

        var content = File.ReadAllText("config.json");
        Assert.Contains("{\n", content);
        Assert.Contains("  \"Committees\"", content);
    }

    public void Dispose()
    {
        _service.Dispose();
        if (File.Exists("config.json")) File.Delete("config.json");
    }
}
