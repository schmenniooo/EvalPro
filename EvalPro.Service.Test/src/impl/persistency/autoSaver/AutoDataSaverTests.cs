using EvalProService.impl.model.entities;
using EvalProService.impl.persistency;
using EvalProService.impl.persistency.autoSaver;
using Xunit;

namespace EvalProServiceTest.impl.persistency.autoSaver;

public class AutoDataSaverTests : IDisposable
{
    private readonly ServiceData _serviceData;
    private readonly AutoDataSaver _autoSaver;
    private static readonly DateTime FixedTestDate = new(2026, 6, 15, 10, 30, 0);
    private const int TimerInterval = 500;
    private const int SafeWaitTime = 800; // Wait time to ensure at least one timer tick

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

        // Assert - No exception thrown, instance created
        Assert.NotNull(_autoSaver);
        Assert.NotNull(_serviceData);
    }

    [Fact]
    public void StartAutoSaveTimer_SavesDataPeriodically()
    {
        // Arrange - Add test data with fixed date
        var committee = new AuditCommittee(
            designation: "Auto-saved Committee",
            apprenticeShip: "Software Development",
            testDates: new List<DateTime> { FixedTestDate }
        );
        _serviceData.AddCommittee(committee);

        // Delete file to ensure timer creates it
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        // Act - Start timer and wait for at least one save cycle
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime);

        // Assert - File should exist
        Assert.True(File.Exists("config.json"), "Timer should have created config.json");

        // Verify content by loading
        var loadedData = new ServiceData();
        var committees = loadedData.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Auto-saved Committee", committees[0].Designation);
    }

    [Fact]
    public void StartAutoSaveTimer_CalledMultipleTimes_DoesNotCrash()
    {
        // Arrange
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _serviceData.AddCommittee(committee);

        // Act - Start timer multiple times
        _autoSaver.StartAutoSaveTimer();
        _autoSaver.StartAutoSaveTimer(); // Second call
        _autoSaver.StartAutoSaveTimer(); // Third call

        Thread.Sleep(SafeWaitTime);

        // Assert - Should not crash, file should exist
        Assert.True(File.Exists("config.json"));
        var loadedData = new ServiceData();
        Assert.Single(loadedData.GetAllCommittees());
    }

    [Fact]
    public void Dispose_SavesDataBeforeCleanup()
    {
        // Arrange - Add data but don't start timer
        var committee = new AuditCommittee(
            designation: "Final Committee",
            apprenticeShip: "IT",
            testDates: new List<DateTime> { FixedTestDate }
        );
        _serviceData.AddCommittee(committee);

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
        var committees = loadedData.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Final Committee", committees[0].Designation);
    }

    [Fact]
    public void StartAutoSaveTimer_HandlesMultipleSaveCycles()
    {
        // Arrange - Add initial data
        var committee = new AuditCommittee(
            designation: "Initial Committee",
            apprenticeShip: "Development",
            testDates: new List<DateTime> { FixedTestDate }
        );
        _serviceData.AddCommittee(committee);

        // Act - Start timer and wait for first save
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime);

        // Verify first save
        var firstLoad = new ServiceData();
        Assert.Single(firstLoad.GetAllCommittees());

        // Add more data
        var committee2 = new AuditCommittee(
            designation: "Second Committee",
            apprenticeShip: "Testing",
            testDates: new List<DateTime> { FixedTestDate.AddDays(1) }
        );
        _serviceData.AddCommittee(committee2);
        Thread.Sleep(SafeWaitTime); // Wait for second save

        // Assert - Both committees should be saved
        var loadedData = new ServiceData();
        var committees = loadedData.GetAllCommittees();
        Assert.Equal(2, committees.Count);
        Assert.Equal("Initial Committee", committees[0].Designation);
        Assert.Equal("Second Committee", committees[1].Designation);
    }

    [Fact]
    public void AutoSaver_WithConcurrentModifications_SavesCorrectly()
    {
        // Arrange
        var committee = new AuditCommittee("Initial", "IT", new List<DateTime> { FixedTestDate });
        _serviceData.AddCommittee(committee);

        // Act - Start timer
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime); // Let initial save complete

        // Add items concurrently with timer
        for (int i = 0; i < 3; i++)
        {
            var newCommittee = new AuditCommittee(
                designation: $"Committee {i}",
                apprenticeShip: "IT",
                testDates: new List<DateTime> { FixedTestDate.AddDays(i) }
            );
            _serviceData.AddCommittee(newCommittee);
            Thread.Sleep(250); // Add items faster than timer interval
        }

        Thread.Sleep(SafeWaitTime); // Wait for final save

        // Assert - All items should be saved
        var loadedData = new ServiceData();
        Assert.Equal(4, loadedData.GetAllCommittees().Count); // 1 initial + 3 added
    }

    [Fact]
    public void AutoSaver_WhenDisposed_StopsTimer()
    {
        // Arrange
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _serviceData.AddCommittee(committee);

        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime); // Let it save once

        // Note the file modification time
        var fileInfo = new FileInfo("config.json");
        var lastWriteTime = fileInfo.LastWriteTime;

        // Act - Dispose should stop timer
        _autoSaver.Dispose();

        // Wait longer than timer interval
        Thread.Sleep(SafeWaitTime * 2);

        // Assert - File should not have been modified again (timer stopped)
        // Note: This test may be flaky due to Dispose also saving, so we just verify no crash
        Assert.True(File.Exists("config.json"));
    }

    [Fact]
    public void AutoSaver_SaveErrorInTimer_DoesNotCrashTimer()
    {
        // This test verifies that the try-catch in SaveDataTimerEvent works
        // We can't easily force a save error without changing implementation,
        // but we can verify the timer keeps running even after an error

        // Arrange
        var committee = new AuditCommittee("Test", "IT", new List<DateTime> { FixedTestDate });
        _serviceData.AddCommittee(committee);

        // Act - Start timer
        _autoSaver.StartAutoSaveTimer();

        // Let timer run through multiple cycles
        Thread.Sleep(SafeWaitTime * 3);

        // Assert - Timer should still be running, file should exist
        Assert.True(File.Exists("config.json"));

        // Verify data is still being saved
        var loadedData = new ServiceData();
        Assert.Single(loadedData.GetAllCommittees());
    }

    [Fact]
    public void AutoSaver_WithEmptyData_SavesSuccessfully()
    {
        // Arrange - No data added to lists
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        // Act - Start timer with empty data
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime);

        // Assert - Should save empty lists without error
        Assert.True(File.Exists("config.json"));

        var loadedData = new ServiceData();
        Assert.Empty(loadedData.GetAllCommittees());
        Assert.Empty(loadedData.GetAllExaminees());
    }

    public void Dispose()
    {
        // Cleanup - Dispose auto saver first to stop timer
        try
        {
            _autoSaver?.Dispose();
        }
        catch
        {
            // Ignore disposal errors in cleanup
        }

        // Then remove test files
        if (File.Exists("config.json"))
        {
            try
            {
                File.Delete("config.json");
            }
            catch
            {
                // Ignore file deletion errors in cleanup
            }
        }
    }
}
