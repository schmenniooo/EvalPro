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
        // Act
        var committee = _service.AddCommittee("Test Committee", "Software Development", new List<DateTime> { FixedTestDate });

        // Assert
        Assert.NotNull(committee);
        Assert.Equal("Test Committee", committee.Designation);
        Assert.Equal("Software Development", committee.ApprenticeShip);
        Assert.Single(committee.TestDates);
        Assert.Contains(FixedTestDate, committee.TestDates);
    }

    [Fact]
    public void GetCommitteeById_ReturnsCommittee_WhenExists()
    {
        // Arrange
        var created = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });

        // Act
        var retrieved = _service.GetCommitteeById(created.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal("Test Committee", retrieved.Designation);
    }

    [Fact]
    public void GetCommitteeById_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = _service.GetCommitteeById("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllCommittees_ReturnsEmptyList_Initially()
    {
        // Act
        var committees = _service.GetAllCommittees();

        // Assert
        Assert.NotNull(committees);
        Assert.Empty(committees);
    }

    [Fact]
    public void GetAllCommittees_ReturnsAllCommittees()
    {
        // Arrange
        _service.AddCommittee("Committee 1", "IT", new List<DateTime> { FixedTestDate });
        _service.AddCommittee("Committee 2", "Development", new List<DateTime> { FixedTestDate.AddDays(1) });
        _service.AddCommittee("Committee 3", "Testing", new List<DateTime> { FixedTestDate.AddDays(2) });

        // Act
        var committees = _service.GetAllCommittees();

        // Assert
        Assert.Equal(3, committees.Count);
    }

    [Fact]
    public void UpdateCommittee_UpdatesDesignation()
    {
        // Arrange
        var committee = _service.AddCommittee("Original", "IT", new List<DateTime> { FixedTestDate });

        // Act
        _service.UpdateCommittee(committee.Id, designation: "Updated");

        // Assert
        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Designation);
        Assert.Equal("IT", updated.ApprenticeShip);
    }

    [Fact]
    public void UpdateCommittee_UpdatesApprenticeShip()
    {
        // Arrange
        var committee = _service.AddCommittee("Test", "Original", new List<DateTime> { FixedTestDate });

        // Act
        _service.UpdateCommittee(committee.Id, apprenticeShip: "Updated");

        // Assert
        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.ApprenticeShip);
    }

    [Fact]
    public void UpdateCommittee_UpdatesTestDates()
    {
        // Arrange
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        var newDates = new List<DateTime> { FixedTestDate.AddDays(10), FixedTestDate.AddDays(20) };

        // Act
        _service.UpdateCommittee(committee.Id, testDates: newDates);

        // Assert
        var updated = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updated);
        Assert.Equal(2, updated.TestDates.Count);
        Assert.Contains(FixedTestDate.AddDays(10), updated.TestDates);
        Assert.Contains(FixedTestDate.AddDays(20), updated.TestDates);
    }

    [Fact]
    public void UpdateCommittee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.UpdateCommittee("non-existent-id", designation: "Test"));

        Assert.Equal("Committee", exception.EntityType);
        Assert.Equal("non-existent-id", exception.EntityId);
    }

    [Fact]
    public void RemoveCommittee_RemovesCommittee()
    {
        // Arrange
        var committee = _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Act
        _service.RemoveCommittee(committee.Id);

        // Assert
        var result = _service.GetCommitteeById(committee.Id);
        Assert.Null(result);
    }

    [Fact]
    public void RemoveCommittee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.RemoveCommittee("non-existent-id"));

        Assert.Equal("Committee", exception.EntityType);
        Assert.Equal("non-existent-id", exception.EntityId);
    }

    // ===== Examinee CRUD Tests =====

    [Fact]
    public void AddExaminee_ReturnsNewExaminee()
    {
        // Act
        var examinee = _service.AddExaminee("John Doe", "Tech Corp", "Jane Smith", "Cloud Migration");

        // Assert
        Assert.NotNull(examinee);
        Assert.Equal("John Doe", examinee.Name);
        Assert.Equal("Tech Corp", examinee.Company);
        Assert.Equal("Jane Smith", examinee.ContactPerson);
        Assert.Equal("Cloud Migration", examinee.ProjectTitle);
    }

    [Fact]
    public void GetExamineeById_ReturnsExaminee_WhenExists()
    {
        // Arrange
        var created = _service.AddExaminee("John Doe", "Tech Corp", "Jane Smith", "Cloud Migration");

        // Act
        var retrieved = _service.GetExamineeById(created.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal("John Doe", retrieved.Name);
    }

    [Fact]
    public void GetExamineeById_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = _service.GetExamineeById("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllExaminees_ReturnsEmptyList_Initially()
    {
        // Act
        var examinees = _service.GetAllExaminees();

        // Assert
        Assert.NotNull(examinees);
        Assert.Empty(examinees);
    }

    [Fact]
    public void GetAllExaminees_ReturnsAllExaminees()
    {
        // Arrange
        _service.AddExaminee("John Doe", "Company A", "Contact A", "Project A");
        _service.AddExaminee("Jane Doe", "Company B", "Contact B", "Project B");

        // Act
        var examinees = _service.GetAllExaminees();

        // Assert
        Assert.Equal(2, examinees.Count);
    }

    [Fact]
    public void UpdateExaminee_UpdatesName()
    {
        // Arrange
        var examinee = _service.AddExaminee("Original Name", "Company", "Contact", "Project");

        // Act
        _service.UpdateExaminee(examinee.Id, name: "Updated Name");

        // Assert
        var updated = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
    }

    [Fact]
    public void UpdateExaminee_UpdatesCompany()
    {
        // Arrange
        var examinee = _service.AddExaminee("Name", "Original Company", "Contact", "Project");

        // Act
        _service.UpdateExaminee(examinee.Id, company: "Updated Company");

        // Assert
        var updated = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Company", updated.Company);
    }

    [Fact]
    public void UpdateExaminee_UpdatesContactPerson()
    {
        // Arrange
        var examinee = _service.AddExaminee("Name", "Company", "Original Contact", "Project");

        // Act
        _service.UpdateExaminee(examinee.Id, contactPerson: "Updated Contact");

        // Assert
        var updated = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Contact", updated.ContactPerson);
    }

    [Fact]
    public void UpdateExaminee_UpdatesProjectTitle()
    {
        // Arrange
        var examinee = _service.AddExaminee("Name", "Company", "Contact", "Original Project");

        // Act
        _service.UpdateExaminee(examinee.Id, projectTitle: "Updated Project");

        // Assert
        var updated = _service.GetExamineeById(examinee.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Project", updated.ProjectTitle);
    }

    [Fact]
    public void UpdateExaminee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.UpdateExaminee("non-existent-id", name: "Test"));

        Assert.Equal("Examinee", exception.EntityType);
        Assert.Equal("non-existent-id", exception.EntityId);
    }

    [Fact]
    public void RemoveExaminee_RemovesExaminee()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        _service.RemoveExaminee(examinee.Id);

        // Assert
        var result = _service.GetExamineeById(examinee.Id);
        Assert.Null(result);
    }

    [Fact]
    public void RemoveExaminee_ThrowsEntityNotFoundException_WhenNotExists()
    {
        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.RemoveExaminee("non-existent-id"));

        Assert.Equal("Examinee", exception.EntityType);
        Assert.Equal("non-existent-id", exception.EntityId);
    }

    [Fact]
    public void RemoveExaminee_RemovesReferenceFromCommittee()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        // Act
        _service.RemoveExaminee(examinee.Id);

        // Assert
        var updatedCommittee = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updatedCommittee);
        Assert.Null(updatedCommittee.ExamineeId);
    }

    // ===== Committee <-> Examinee Relationship Tests =====

    [Fact]
    public void AssignExamineeToCommittee_AssignsSuccessfully()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        // Assert
        var updatedCommittee = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updatedCommittee);
        Assert.Equal(examinee.Id, updatedCommittee.ExamineeId);
    }

    [Fact]
    public void AssignExamineeToCommittee_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });

        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignExamineeToCommittee(committee.Id, "non-existent-examinee"));

        Assert.Equal("Examinee", exception.EntityType);
    }

    [Fact]
    public void AssignExamineeToCommittee_ThrowsEntityNotFoundException_WhenCommitteeNotExists()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignExamineeToCommittee("non-existent-committee", examinee.Id));

        Assert.Equal("Committee", exception.EntityType);
    }

    [Fact]
    public void RemoveExamineeFromCommittee_RemovesReference()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        // Act
        _service.RemoveExamineeFromCommittee(committee.Id);

        // Assert
        var updatedCommittee = _service.GetCommitteeById(committee.Id);
        Assert.NotNull(updatedCommittee);
        Assert.Null(updatedCommittee.ExamineeId);
    }

    [Fact]
    public void RemoveExamineeFromCommittee_ThrowsEntityNotFoundException_WhenCommitteeNotExists()
    {
        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.RemoveExamineeFromCommittee("non-existent-committee"));

        Assert.Equal("Committee", exception.EntityType);
    }

    [Fact]
    public void GetExamineeForCommittee_ReturnsExaminee_WhenAssigned()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        // Act
        var result = _service.GetExamineeForCommittee(committee.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(examinee.Id, result.Id);
    }

    [Fact]
    public void GetExamineeForCommittee_ReturnsNull_WhenNoExamineeAssigned()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });

        // Act
        var result = _service.GetExamineeForCommittee(committee.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetExamineeForCommittee_ReturnsNull_WhenCommitteeNotExists()
    {
        // Act
        var result = _service.GetExamineeForCommittee("non-existent-committee");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCommitteeForExaminee_ReturnsCommittee_WhenAssigned()
    {
        // Arrange
        var committee = _service.AddCommittee("Test Committee", "IT", new List<DateTime> { FixedTestDate });
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        _service.AssignExamineeToCommittee(committee.Id, examinee.Id);

        // Act
        var result = _service.GetCommitteeForExaminee(examinee.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(committee.Id, result.Id);
    }

    [Fact]
    public void GetCommitteeForExaminee_ReturnsNull_WhenNotAssigned()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        var result = _service.GetCommitteeForExaminee(examinee.Id);

        // Assert
        Assert.Null(result);
    }

    // ===== Examinee <-> Rating Relationship Tests =====

    [Fact]
    public void AssignProjectDocumentation_AssignsSuccessfully()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        var documentation = new ProjectDocumentation(
            "Good work",
            new Dictionary<string, int> { { "Quality", 8 }, { "Completeness", 9 } },
            new Dictionary<string, string> { { "Quality", "Well structured" } }
        );

        // Act
        _service.AssignProjectDocumentation(examinee.Id, documentation);

        // Assert
        var result = _service.GetProjectDocumentationForExaminee(examinee.Id);
        Assert.NotNull(result);
        Assert.Equal("Good work", result.FinalComment);
    }

    [Fact]
    public void AssignProjectDocumentation_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        // Arrange
        var documentation = new ProjectDocumentation(
            "Good work",
            new Dictionary<string, int>(),
            new Dictionary<string, string>()
        );

        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignProjectDocumentation("non-existent-id", documentation));

        Assert.Equal("Examinee", exception.EntityType);
    }

    [Fact]
    public void GetProjectDocumentationForExaminee_ReturnsNull_WhenNotAssigned()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        var result = _service.GetProjectDocumentationForExaminee(examinee.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetProjectDocumentationForExaminee_ReturnsNull_WhenExamineeNotExists()
    {
        // Act
        var result = _service.GetProjectDocumentationForExaminee("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AssignProjectPresentation_AssignsSuccessfully()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        var presentation = new ProjectPresentation(
            "Excellent presentation",
            new Dictionary<string, int> { { "Clarity", 9 } },
            new Dictionary<string, string> { { "Clarity", "Very clear" } }
        );

        // Act
        _service.AssignProjectPresentation(examinee.Id, presentation);

        // Assert
        var result = _service.GetProjectPresentationForExaminee(examinee.Id);
        Assert.NotNull(result);
        Assert.Equal("Excellent presentation", result.FinalComment);
    }

    [Fact]
    public void AssignProjectPresentation_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        // Arrange
        var presentation = new ProjectPresentation(
            "Good",
            new Dictionary<string, int>(),
            new Dictionary<string, string>()
        );

        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignProjectPresentation("non-existent-id", presentation));

        Assert.Equal("Examinee", exception.EntityType);
    }

    [Fact]
    public void GetProjectPresentationForExaminee_ReturnsNull_WhenNotAssigned()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        var result = _service.GetProjectPresentationForExaminee(examinee.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AssignTechConversation_AssignsSuccessfully()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        var conversation = new TechConversation(
            "Solid technical knowledge",
            new Dictionary<string, int> { { "Depth", 8 } },
            new Dictionary<string, string> { { "Depth", "Good understanding" } }
        );

        // Act
        _service.AssignTechConversation(examinee.Id, conversation);

        // Assert
        var result = _service.GetTechConversationForExaminee(examinee.Id);
        Assert.NotNull(result);
        Assert.Equal("Solid technical knowledge", result.FinalComment);
    }

    [Fact]
    public void AssignTechConversation_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        // Arrange
        var conversation = new TechConversation(
            "Good",
            new Dictionary<string, int>(),
            new Dictionary<string, string>()
        );

        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignTechConversation("non-existent-id", conversation));

        Assert.Equal("Examinee", exception.EntityType);
    }

    [Fact]
    public void GetTechConversationForExaminee_ReturnsNull_WhenNotAssigned()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        var result = _service.GetTechConversationForExaminee(examinee.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AssignSupplementaryExamination_AssignsSuccessfully()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");
        var exam = new SupplementaryExamination(
            "Networking",
            85,
            new List<string> { "What is TCP/IP?", "Explain DNS" }
        );

        // Act
        _service.AssignSupplementaryExamination(examinee.Id, exam);

        // Assert
        var result = _service.GetSupplementaryExaminationForExaminee(examinee.Id);
        Assert.NotNull(result);
        Assert.Equal("Networking", result.ChosenTestArea);
        Assert.Equal(85, result.Points);
    }

    [Fact]
    public void AssignSupplementaryExamination_ThrowsEntityNotFoundException_WhenExamineeNotExists()
    {
        // Arrange
        var exam = new SupplementaryExamination(
            "Networking",
            70,
            new List<string> { "Question 1" }
        );

        // Act & Assert
        var exception = Assert.Throws<EntityNotFoundException>(() =>
            _service.AssignSupplementaryExamination("non-existent-id", exam));

        Assert.Equal("Examinee", exception.EntityType);
    }

    [Fact]
    public void GetSupplementaryExaminationForExaminee_ReturnsNull_WhenNotAssigned()
    {
        // Arrange
        var examinee = _service.AddExaminee("John Doe", "Company", "Contact", "Project");

        // Act
        var result = _service.GetSupplementaryExaminationForExaminee(examinee.Id);

        // Assert
        Assert.Null(result);
    }

    // ===== Persistence Tests =====

    [Fact]
    public void SaveConfigToJson_CreatesJsonFile()
    {
        // Arrange
        _service.AddCommittee("Test Committee", "Software Development", new List<DateTime> { FixedTestDate });

        // Act
        _service.SaveConfigToJson();

        // Assert
        Assert.True(File.Exists("config.json"));
        var fileContent = File.ReadAllText("config.json");
        Assert.Contains("Test Committee", fileContent);
        Assert.Contains("Software Development", fileContent);
    }

    [Fact]
    public void SaveConfigToJson_HandlesEmptyLists()
    {
        // Act
        _service.SaveConfigToJson();

        // Assert
        Assert.True(File.Exists("config.json"));
        var newService = new ServiceClass();
        Assert.Empty(newService.GetAllCommittees());
        Assert.Empty(newService.GetAllExaminees());
        newService.Dispose();
    }

    [Fact]
    public void SaveAndLoad_PreservesAuditCommitteeData()
    {
        // Arrange
        var testDate1 = new DateTime(2026, 6, 15);
        var testDate2 = new DateTime(2026, 6, 20);
        _service.AddCommittee("Backend Team", "Application Development", new List<DateTime> { testDate1, testDate2 });

        // Act
        _service.SaveConfigToJson();
        var newService = new ServiceClass();

        // Assert
        var committees = newService.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Backend Team", committees[0].Designation);
        Assert.Equal("Application Development", committees[0].ApprenticeShip);
        Assert.Equal(2, committees[0].TestDates.Count);
        Assert.Contains(testDate1, committees[0].TestDates);
        Assert.Contains(testDate2, committees[0].TestDates);
        newService.Dispose();
    }

    [Fact]
    public void SaveAndLoad_PreservesMultipleCommittees()
    {
        // Arrange
        _service.AddCommittee("Team A", "IT", new List<DateTime> { FixedTestDate });
        _service.AddCommittee("Team B", "Development", new List<DateTime> { FixedTestDate.AddDays(1) });
        _service.AddCommittee("Team C", "Testing", new List<DateTime> { FixedTestDate.AddDays(2) });

        // Act
        _service.SaveConfigToJson();
        var newService = new ServiceClass();

        // Assert
        var committees = newService.GetAllCommittees();
        Assert.Equal(3, committees.Count);
        Assert.Equal("Team A", committees[0].Designation);
        Assert.Equal("Team B", committees[1].Designation);
        Assert.Equal("Team C", committees[2].Designation);
        newService.Dispose();
    }

    [Fact]
    public void SaveConfigToJson_WithConcurrentSaves_CompletesSuccessfully()
    {
        // Arrange
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Act - Call save multiple times concurrently
        var tasks = new List<Task>();
        var exceptionsCaught = 0;

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    _service.SaveConfigToJson();
                }
                catch
                {
                    Interlocked.Increment(ref exceptionsCaught);
                }
            }));
        }

        #pragma warning disable xUnit1031
        Task.WaitAll(tasks.ToArray());
        #pragma warning restore xUnit1031

        // Assert
        Assert.Equal(0, exceptionsCaught);
        Assert.True(File.Exists("config.json"));

        var loadedService = new ServiceClass();
        Assert.Single(loadedService.GetAllCommittees());
        loadedService.Dispose();
    }

    [Fact]
    public void LoadConfigFromJson_WithMalformedJson_HandlesGracefully()
    {
        // Arrange - Write malformed JSON
        File.WriteAllText("config.json", "{ this is not valid JSON }");

        // Act - Should not throw, just keep empty lists
        var newService = new ServiceClass();

        // Assert
        Assert.Empty(newService.GetAllCommittees());
        Assert.Empty(newService.GetAllExaminees());
        newService.Dispose();
    }

    [Fact]
    public void LoadConfigFromJson_WithEmptyFile_HandlesGracefully()
    {
        // Arrange
        File.WriteAllText("config.json", "");

        // Act - Should not throw, just keep empty lists
        var newService = new ServiceClass();

        // Assert
        Assert.Empty(newService.GetAllCommittees());
        Assert.Empty(newService.GetAllExaminees());
        newService.Dispose();
    }

    [Fact]
    public void LoadConfigFromJson_WithValidButEmptyJson_LoadsEmptyLists()
    {
        // Arrange
        File.WriteAllText("config.json", "{}");

        // Act
        var newService = new ServiceClass();

        // Assert
        Assert.Empty(newService.GetAllCommittees());
        Assert.Empty(newService.GetAllExaminees());
        newService.Dispose();
    }

    [Fact]
    public void SaveConfigToJson_CreatesReadableJsonFormat()
    {
        // Arrange
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Act
        _service.SaveConfigToJson();

        // Assert - JSON should be indented and readable
        var jsonContent = File.ReadAllText("config.json");
        Assert.Contains("{\n", jsonContent);
        Assert.Contains("  \"Committees\"", jsonContent);
    }

    public void Dispose()
    {
        _service.Dispose();

        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }
    }
}

