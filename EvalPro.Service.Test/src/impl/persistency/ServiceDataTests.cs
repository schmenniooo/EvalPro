using EvalProService.impl.model.entities;
using EvalProService.impl.persistency;
using Xunit;

namespace EvalProServiceTest.impl.persistency;

public class ServiceDataTests : IDisposable
{
    private readonly ServiceData _serviceData;
    private static readonly DateTime FixedTestDate = new(2026, 6, 15, 10, 30, 0);

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
        // Assert - Check that all lists are initialized and accessible (should return empty lists, not null)
        Assert.NotNull(_serviceData.GetAllCommittees());
        Assert.NotNull(_serviceData.GetAllExaminees());
        Assert.Empty(_serviceData.GetAllCommittees());
        Assert.Empty(_serviceData.GetAllExaminees());
    }

    [Fact]
    public void SaveConfigToJson_CreatesJsonFile()
    {
        // Arrange - Create test committee with fixed date
        var testCommittee = new AuditCommittee(
            designation: "Test Committee",
            apprenticeShip: "Software Development",
            testDates: new List<DateTime> { FixedTestDate }
        );
        _serviceData.AddCommittee(testCommittee);

        // Act - Save to JSON
        _serviceData.SaveConfigToJson();

        // Assert - Check file was created
        Assert.True(File.Exists("config.json"));

        // Verify file has content
        var fileContent = File.ReadAllText("config.json");
        Assert.Contains("Test Committee", fileContent);
        Assert.Contains("Software Development", fileContent);
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
        Assert.Empty(newServiceData.GetAllCommittees());
        Assert.Empty(newServiceData.GetAllExaminees());
    }

    [Fact]
    public void SaveAndLoad_PreservesAuditCommitteeData()
    {
        // Arrange - Create test data with fixed dates
        var testDate1 = new DateTime(2026, 6, 15);
        var testDate2 = new DateTime(2026, 6, 20);
        var testCommittee = new AuditCommittee(
            designation: "Backend Team",
            apprenticeShip: "Application Development",
            testDates: new List<DateTime> { testDate1, testDate2 }
        );
        _serviceData.AddCommittee(testCommittee);

        // Act - Save and create new instance to load
        _serviceData.SaveConfigToJson();
        var newServiceData = new ServiceData();

        // Assert - Check data was loaded correctly
        var committees = newServiceData.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Backend Team", committees[0].Designation);
        Assert.Equal("Application Development", committees[0].ApprenticeShip);
        Assert.Equal(2, committees[0].TestDates.Count);
        Assert.Contains(testDate1, committees[0].TestDates);
        Assert.Contains(testDate2, committees[0].TestDates);
    }

    [Fact]
    public void SaveAndLoad_PreservesMultipleCommittees()
    {
        // Arrange - Create multiple committees
        var committee1 = new AuditCommittee("Team A", "IT", new List<DateTime> { FixedTestDate });
        var committee2 = new AuditCommittee("Team B", "Development", new List<DateTime> { FixedTestDate.AddDays(1) });
        var committee3 = new AuditCommittee("Team C", "Testing", new List<DateTime> { FixedTestDate.AddDays(2) });

        _serviceData.AddCommittee(committee1);
        _serviceData.AddCommittee(committee2);
        _serviceData.AddCommittee(committee3);

        // Act
        _serviceData.SaveConfigToJson();
        var newServiceData = new ServiceData();

        // Assert
        var committees = newServiceData.GetAllCommittees();
        Assert.Equal(3, committees.Count);
        Assert.Equal("Team A", committees[0].Designation);
        Assert.Equal("Team B", committees[1].Designation);
        Assert.Equal("Team C", committees[2].Designation);
    }

    [Fact]
    public void SaveConfigToJson_WithConcurrentSaves_CompletesSuccessfully()
    {
        // Arrange - Add test data with fixed date
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _serviceData.AddCommittee(committee);

        // Act - Call save multiple times concurrently
        var tasks = new List<Task>();
        var exceptionsCaught = 0;

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    _serviceData.SaveConfigToJson();
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

        // Assert - Should complete without exceptions and file should be valid
        Assert.Equal(0, exceptionsCaught);
        Assert.True(File.Exists("config.json"));

        // Verify the saved data is still valid
        var loadedData = new ServiceData();
        Assert.Single(loadedData.GetAllCommittees());
    }

    [Fact]
    public void LoadConfigFromJson_WithMalformedJson_HandlesGracefully()
    {
        // Arrange - Write malformed JSON
        File.WriteAllText("config.json", "{ this is not valid JSON }");

        // Act - Should not throw, just keep empty lists
        var serviceData = new ServiceData();

        // Assert - Should have empty lists (error was caught and logged)
        Assert.Empty(serviceData.GetAllCommittees());
        Assert.Empty(serviceData.GetAllExaminees());
    }

    [Fact]
    public void LoadConfigFromJson_WithEmptyFile_HandlesGracefully()
    {
        // Arrange - Create empty file
        File.WriteAllText("config.json", "");

        // Act - Should not throw, just keep empty lists
        var serviceData = new ServiceData();

        // Assert - Should have empty lists (error was caught and logged)
        Assert.Empty(serviceData.GetAllCommittees());
        Assert.Empty(serviceData.GetAllExaminees());
    }

    [Fact]
    public void LoadConfigFromJson_WithValidButEmptyJson_LoadsEmptyLists()
    {
        // Arrange - Write valid empty JSON object
        File.WriteAllText("config.json", "{}");

        // Act
        var serviceData = new ServiceData();

        // Assert - All lists should be empty
        Assert.Empty(serviceData.GetAllCommittees());
        Assert.Empty(serviceData.GetAllExaminees());
    }

    [Fact]
    public void SaveConfigToJson_CreatesReadableJsonFormat()
    {
        // Arrange
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _serviceData.AddCommittee(committee);

        // Act
        _serviceData.SaveConfigToJson();

        // Assert - JSON should be indented and readable
        var jsonContent = File.ReadAllText("config.json");
        Assert.Contains("{\n", jsonContent); // Check for formatting
        Assert.Contains("  \"Committees\"", jsonContent); // Check for indentation
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
