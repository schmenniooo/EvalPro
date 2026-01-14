using EvalProService.impl.model.entities;
using EvalProService.impl.persistency;
using Xunit;

namespace EvalProServiceTest.impl.persistency;

public class ServiceDataTests : IDisposable
{
    private readonly ServiceData _serviceData;

    public ServiceDataTests()
    {
        // Clean up any existing test file before test runs
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        _serviceData = new ServiceData();
    }

    [Fact]
    public void Constructor_InitializesAllLists()
    {
        // Assert - Check that all lists are initialized and not null
        Assert.NotNull(_serviceData.CommitteesList);
        Assert.NotNull(_serviceData.ExamineesList);
        Assert.NotNull(_serviceData.ProjectDocumentationList);
        Assert.NotNull(_serviceData.ProjectPresentationList);
        Assert.NotNull(_serviceData.ProjectTechConversationList);
        Assert.NotNull(_serviceData.SupplementaryExaminationList);
    }

    [Fact]
    public void SaveConfigToJson_CreatesJsonFile()
    {
        // Arrange - Create test committee
        var testCommittee = new AuditCommittee(
            designation: "Test Committee",
            apprenticeShip: "Software Development",
            testDates: new List<DateTime> { DateTime.Now }
        );
        _serviceData.CommitteesList.Add(testCommittee);

        // Act - Save to JSON
        _serviceData.SaveConfigToJson();

        // Assert - Check file was created
        Assert.True(File.Exists("config.json"));

        // Verify file has content
        var fileContent = File.ReadAllText("config.json");
        Assert.Contains("Test Committee", fileContent);
    }

    [Fact]
    public void SaveConfigToJson_HandlesEmptyLists()
    {
        // Act - Save with empty lists
        _serviceData.SaveConfigToJson();

        // Assert - File should exist
        Assert.True(File.Exists("config.json"));

        // Load and verify empty lists are preserved
        var newServiceData = new ServiceData();
        Assert.Empty(newServiceData.CommitteesList);
        Assert.Empty(newServiceData.ExamineesList);
    }

    [Fact]
    public void SaveAndLoad_PreservesAuditCommitteeData()
    {
        // Arrange - Create test data
        var testDate = new DateTime(2026, 6, 15);
        var testCommittee = new AuditCommittee(
            designation: "Backend Team",
            apprenticeShip: "Application Development",
            testDates: new List<DateTime> { testDate }
        );
        _serviceData.CommitteesList.Add(testCommittee);

        // Act - Save and create new instance to load
        _serviceData.SaveConfigToJson();
        var newServiceData = new ServiceData();

        // Assert - Check data was loaded correctly
        Assert.Single(newServiceData.CommitteesList);
        Assert.Equal("Backend Team", newServiceData.CommitteesList[0].Designation);
        Assert.Equal("Application Development", newServiceData.CommitteesList[0].ApprenticeShip);
        Assert.Single(newServiceData.CommitteesList[0].TestDates);
    }

    [Fact]
    public void SaveConfigToJson_IsThreadSafe()
    {
        // Arrange - Add test data
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { DateTime.Now });
        _serviceData.CommitteesList.Add(committee);

        // Act - Call save multiple times rapidly (simulating concurrent access)
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() => _serviceData.SaveConfigToJson()));
        }

        // Wait for all tasks to complete
        // Disabling warning that tests shouldn't be stopped
        #pragma warning disable xUnit1031
        
        Task.WaitAll(tasks.ToArray());
        
        #pragma warning restore xUnit1031

        // Assert - Should complete without exceptions
        Assert.True(File.Exists("config.json"));
    }

    public void Dispose()
    {
        // Cleanup - Remove test files
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }
    }
}
