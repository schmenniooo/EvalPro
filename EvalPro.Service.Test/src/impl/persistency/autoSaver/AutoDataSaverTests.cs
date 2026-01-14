using EvalProService.impl.model.entities;
using EvalProService.impl.persistency;
using EvalProService.impl.persistency.autoSaver;
using Xunit;

namespace EvalProServiceTest.impl.persistency.autoSaver;

public class AutoDataSaverTests : IDisposable
{
    private readonly ServiceData _serviceData;
    private readonly AutoDataSaver _autoSaver;

    public AutoDataSaverTests()
    {
        // Clean up any existing test file
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        _serviceData = new ServiceData();
        _autoSaver = new AutoDataSaver(_serviceData);
    }

    [Fact]
    public void Constructor_InitializesSuccessfully()
    {
        // Arrange & Act - Constructor called in setup

        // Assert - No exception thrown
        Assert.NotNull(_autoSaver);
    }

    [Fact]
    public void StartAutoSaveTimer_SavesDataPeriodically()
    {
        // Arrange - Add test data
        var committee = new AuditCommittee(
            designation: "Auto-saved Committee",
            apprenticeShip: "Software Development",
            testDates: new List<DateTime> { DateTime.Now }
        );
        _serviceData.CommitteesList.Add(committee);

        // Act - Start timer and wait for at least one save cycle (500ms)
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(700); // Wait longer than timer duration

        // Assert - File should exist
        Assert.True(File.Exists("config.json"));

        // Verify content by loading
        var loadedData = new ServiceData();
        Assert.Single(loadedData.CommitteesList);
        Assert.Equal("Auto-saved Committee", loadedData.CommitteesList[0].Designation);
    }

    [Fact]
    public void Dispose_SavesDataBeforeCleanup()
    {
        // Arrange - Add data but don't start timer
        var committee = new AuditCommittee(
            designation: "Final Committee",
            apprenticeShip: "IT",
            testDates: new List<DateTime> { DateTime.Now }
        );
        _serviceData.CommitteesList.Add(committee);

        // Delete any existing file to ensure Dispose creates it
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        // Act - Dispose should save data
        _autoSaver.Dispose();

        // Assert - File should exist with correct data
        Assert.True(File.Exists("config.json"));

        var loadedData = new ServiceData();
        Assert.Single(loadedData.CommitteesList);
        Assert.Equal("Final Committee", loadedData.CommitteesList[0].Designation);
    }

    [Fact]
    public void StartAutoSaveTimer_HandlesMultipleSaveCycles()
    {
        // Arrange - Add initial data
        var committee = new AuditCommittee(
            designation: "Initial Committee",
            apprenticeShip: "Development",
            testDates: new List<DateTime> { DateTime.Now }
        );
        _serviceData.CommitteesList.Add(committee);

        // Act - Start timer and wait for multiple cycles
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(700); // First save

        // Add more data
        var committee2 = new AuditCommittee(
            designation: "Second Committee",
            apprenticeShip: "Testing",
            testDates: new List<DateTime> { DateTime.Now.AddDays(1) }
        );
        _serviceData.CommitteesList.Add(committee2);
        Thread.Sleep(700); // Second save

        // Assert - Both committees should be saved
        var loadedData = new ServiceData();
        Assert.Equal(2, loadedData.CommitteesList.Count);
    }

    [Fact]
    public void AutoSaver_HandlesConcurrentSaves()
    {
        // Arrange
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { DateTime.Now });
        _serviceData.CommitteesList.Add(committee);

        // Act - Start timer (which saves every 500ms)
        _autoSaver.StartAutoSaveTimer();

        // Simulate concurrent modifications
        for (int i = 0; i < 3; i++)
        {
            var newCommittee = new AuditCommittee(
                designation: $"Committee {i}",
                apprenticeShip: "IT",
                testDates: new List<DateTime> { DateTime.Now }
            );
            _serviceData.CommitteesList.Add(newCommittee);
            Thread.Sleep(200); // Add while timer is running
        }

        Thread.Sleep(700); // Wait for final save

        // Assert - Should complete without exceptions
        var loadedData = new ServiceData();
        Assert.Equal(4, loadedData.CommitteesList.Count); // 1 initial + 3 added
    }

    public void Dispose()
    {
        // Cleanup
        _autoSaver?.Dispose();

        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }
    }
}
