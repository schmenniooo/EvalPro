using EvalProService.impl.model.entities;
using EvalProService.impl.model.events;
using EvalProService.impl.persistency.autoSaver;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ServiceClass = EvalProService.impl.service.EvalProService;

namespace EvalProServiceTest.impl.persistency.autoSaver;

public class AutoDataSaverTests : IDisposable
{
    private readonly ServiceClass _service;
    private readonly AutoDataSaver _autoSaver;
    private static readonly DateTime FixedTestDate = new(2026, 6, 15, 10, 30, 0);
    private const int SafeWaitTime = 800; // Wait time to ensure at least one timer tick

    public AutoDataSaverTests()
    {
        // Clean up any existing test file
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        _service = new ServiceClass();
        _autoSaver = new AutoDataSaver(_service.SaveConfigToJson, NullLogger<ServiceClass>.Instance);
    }

    [Fact]
    public void Constructor_InitializesSuccessfully()
    {
        // Assert - No exception thrown, instance created
        Assert.NotNull(_autoSaver);
        Assert.NotNull(_service);
    }

    [Fact]
    public void StartAutoSaveTimer_SavesDataPeriodically()
    {
        // Arrange - Add test data with fixed date
        _service.AddCommittee("Auto-saved Committee", "Software Development", new List<DateTime> { FixedTestDate });

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
        var loadedService = new ServiceClass();
        var committees = loadedService.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Auto-saved Committee", committees[0].Designation);
        loadedService.Dispose();
    }

    [Fact]
    public void StartAutoSaveTimer_CalledMultipleTimes_DoesNotCrash()
    {
        // Arrange
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Act - Start timer multiple times
        _autoSaver.StartAutoSaveTimer();
        _autoSaver.StartAutoSaveTimer(); // Second call
        _autoSaver.StartAutoSaveTimer(); // Third call

        Thread.Sleep(SafeWaitTime);

        // Assert - Should not crash, file should exist
        Assert.True(File.Exists("config.json"));
        var loadedService = new ServiceClass();
        Assert.Single(loadedService.GetAllCommittees());
        loadedService.Dispose();
    }

    [Fact]
    public void Dispose_SavesDataBeforeCleanup()
    {
        // Arrange - Add data but don't start timer
        _service.AddCommittee("Final Committee", "IT", new List<DateTime> { FixedTestDate });

        // Delete any existing file to ensure Dispose creates it
        if (File.Exists("config.json"))
        {
            File.Delete("config.json");
        }

        // Act - Dispose should save data
        _autoSaver.Dispose();

        // Assert - File should exist with correct data
        Assert.True(File.Exists("config.json"));

        var loadedService = new ServiceClass();
        var committees = loadedService.GetAllCommittees();
        Assert.Single(committees);
        Assert.Equal("Final Committee", committees[0].Designation);
        loadedService.Dispose();
    }

    [Fact]
    public void StartAutoSaveTimer_HandlesMultipleSaveCycles()
    {
        // Arrange - Add initial data
        _service.AddCommittee("Initial Committee", "Development", new List<DateTime> { FixedTestDate });

        // Act - Start timer and wait for first save
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime);

        // Verify first save
        var firstLoad = new ServiceClass();
        Assert.Single(firstLoad.GetAllCommittees());
        firstLoad.Dispose();

        // Add more data
        _service.AddCommittee("Second Committee", "Testing", new List<DateTime> { FixedTestDate.AddDays(1) });
        Thread.Sleep(SafeWaitTime); // Wait for second save

        // Assert - Both committees should be saved
        var loadedService = new ServiceClass();
        var committees = loadedService.GetAllCommittees();
        Assert.Equal(2, committees.Count);
        Assert.Equal("Initial Committee", committees[0].Designation);
        Assert.Equal("Second Committee", committees[1].Designation);
        loadedService.Dispose();
    }

    [Fact]
    public void AutoSaver_WithConcurrentModifications_SavesCorrectly()
    {
        // Arrange
        _service.AddCommittee("Initial", "IT", new List<DateTime> { FixedTestDate });

        // Act - Start timer
        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime); // Let initial save complete

        // Add items concurrently with timer
        for (int i = 0; i < 3; i++)
        {
            _service.AddCommittee($"Committee {i}", "IT", new List<DateTime> { FixedTestDate.AddDays(i) });
            Thread.Sleep(250); // Add items faster than timer interval
        }

        Thread.Sleep(SafeWaitTime); // Wait for final save

        // Assert - All items should be saved
        var loadedService = new ServiceClass();
        Assert.Equal(4, loadedService.GetAllCommittees().Count); // 1 initial + 3 added
        loadedService.Dispose();
    }

    [Fact]
    public void AutoSaver_WhenDisposed_StopsTimer()
    {
        // Arrange
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        _autoSaver.StartAutoSaveTimer();
        Thread.Sleep(SafeWaitTime); // Let it save once

        // Act - Dispose should stop timer
        _autoSaver.Dispose();

        // Wait longer than timer interval
        Thread.Sleep(SafeWaitTime * 2);

        // Assert - Should not crash, file should exist
        Assert.True(File.Exists("config.json"));
    }

    [Fact]
    public void AutoSaver_SaveErrorInTimer_DoesNotCrashTimer()
    {
        // Arrange
        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Act - Start timer
        _autoSaver.StartAutoSaveTimer();

        // Let timer run through multiple cycles
        Thread.Sleep(SafeWaitTime * 3);

        // Assert - Timer should still be running, file should exist
        Assert.True(File.Exists("config.json"));

        // Verify data is still being saved
        var loadedService = new ServiceClass();
        Assert.Single(loadedService.GetAllCommittees());
        loadedService.Dispose();
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

        var loadedService = new ServiceClass();
        Assert.Empty(loadedService.GetAllCommittees());
        Assert.Empty(loadedService.GetAllExaminees());
        loadedService.Dispose();
    }

    [Fact]
    public void OnSaveError_WhenTimerSaveFails_RaisesEventWithIsCriticalFalse()
    {
        // Arrange
        AutoSaveErrorEventArgs? receivedArgs = null;
        _autoSaver.OnSaveError += (_, args) => receivedArgs = args;

        _service.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Make the config file read-only to trigger a save error
        _service.SaveConfigToJson(); // Create the file first
        File.SetAttributes("config.json", FileAttributes.ReadOnly);

        try
        {
            // Act - Start timer which will try to save and fail
            _autoSaver.StartAutoSaveTimer();
            Thread.Sleep(SafeWaitTime);

            // Assert - Event should have been raised with IsCritical = false
            Assert.NotNull(receivedArgs);
            Assert.False(receivedArgs.IsCritical);
            Assert.NotNull(receivedArgs.Exception);
            Assert.True(receivedArgs.Timestamp <= DateTime.Now);
        }
        finally
        {
            // Cleanup - Remove read-only attribute
            File.SetAttributes("config.json", FileAttributes.Normal);
        }
    }

    [Fact]
    public void OnSaveError_WhenDisposeSaveFails_RaisesEventWithIsCriticalTrue()
    {
        // Arrange
        AutoSaveErrorEventArgs? receivedArgs = null;
        var localService = new ServiceClass();
        var localAutoSaver = new AutoDataSaver(localService.SaveConfigToJson, NullLogger<ServiceClass>.Instance);
        localAutoSaver.OnSaveError += (_, args) => receivedArgs = args;

        localService.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        // Make the config file read-only to trigger a save error
        localService.SaveConfigToJson(); // Create the file first
        File.SetAttributes("config.json", FileAttributes.ReadOnly);

        try
        {
            // Act - Dispose will try to save and fail
            localAutoSaver.Dispose();

            // Assert - Event should have been raised with IsCritical = true
            Assert.NotNull(receivedArgs);
            Assert.True(receivedArgs.IsCritical);
            Assert.NotNull(receivedArgs.Exception);
        }
        finally
        {
            // Cleanup - Remove read-only attribute
            File.SetAttributes("config.json", FileAttributes.Normal);
            localService.Dispose();
        }
    }

    [Fact]
    public void OnSaveError_EventContainsTimestamp()
    {
        // Arrange
        AutoSaveErrorEventArgs? receivedArgs = null;
        var localService = new ServiceClass();
        var localAutoSaver = new AutoDataSaver(localService.SaveConfigToJson, NullLogger<ServiceClass>.Instance);
        localAutoSaver.OnSaveError += (_, args) => receivedArgs = args;

        localService.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        localService.SaveConfigToJson();
        File.SetAttributes("config.json", FileAttributes.ReadOnly);

        var beforeDispose = DateTime.Now;

        try
        {
            // Act
            localAutoSaver.Dispose();

            // Assert
            Assert.NotNull(receivedArgs);
            Assert.True(receivedArgs.Timestamp >= beforeDispose);
            Assert.True(receivedArgs.Timestamp <= DateTime.Now);
        }
        finally
        {
            File.SetAttributes("config.json", FileAttributes.Normal);
            localService.Dispose();
        }
    }

    [Fact]
    public void OnSaveError_WhenNoSubscribers_DoesNotThrow()
    {
        // Arrange - Don't subscribe to the event
        var localService = new ServiceClass();
        var localAutoSaver = new AutoDataSaver(localService.SaveConfigToJson, NullLogger<ServiceClass>.Instance);

        localService.AddCommittee("Test", "IT", new List<DateTime> { FixedTestDate });

        localService.SaveConfigToJson();
        File.SetAttributes("config.json", FileAttributes.ReadOnly);

        try
        {
            // Act & Assert - Should not throw even without subscribers
            var exception = Record.Exception(() => localAutoSaver.Dispose());
            Assert.Null(exception);
        }
        finally
        {
            File.SetAttributes("config.json", FileAttributes.Normal);
            localService.Dispose();
        }
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

        try
        {
            _service?.Dispose();
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
