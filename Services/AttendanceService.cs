using System.Text.Json;
using EventEase.Models;
using Microsoft.JSInterop;

namespace EventEase.Services;

public class AttendanceService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AttendanceService> _logger;
    private readonly SessionService _sessionService;
    private readonly string STORAGE_KEY = "event_attendance";
    private List<Attendance> _attendanceRecords = new();

    public event Action? OnAttendanceChanged;

    public AttendanceService(
        IJSRuntime jsRuntime,
        ILogger<AttendanceService> logger,
        SessionService sessionService)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
        _sessionService = sessionService;
        LoadAttendance();
    }

    public async Task<bool> RegisterForEventAsync(int eventId, string? notes = null)
    {
        try
        {
            var currentUser = _sessionService.GetCurrentSession();
            if (currentUser == null || !_sessionService.IsAuthenticated)
            {
                _logger.LogWarning("Attempted to register without authentication");
                return false;
            }

            // Check if user is already registered
            var existingRecord = _attendanceRecords
                .FirstOrDefault(a => a.EventId == eventId && a.UserId == currentUser.Id);

            if (existingRecord != null)
            {
                _logger.LogWarning($"User {currentUser.Id} already registered for event {eventId}");
                return false;
            }

            var attendance = new Attendance
            {
                Id = _attendanceRecords.Count > 0 ? _attendanceRecords.Max(a => a.Id) + 1 : 1,
                EventId = eventId,
                UserId = currentUser.Id,
                Status = AttendanceStatus.Registered,
                RegisteredAt = DateTime.Now,
                Notes = notes
            };

            _attendanceRecords.Add(attendance);
            await SaveAttendanceAsync();
            
            OnAttendanceChanged?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering for event");
            return false;
        }
    }

    public async Task<bool> UpdateAttendanceStatusAsync(int eventId, AttendanceStatus status)
    {
        try
        {
            var currentUser = _sessionService.GetCurrentSession();
            if (currentUser == null || !_sessionService.IsAuthenticated)
            {
                return false;
            }

            var record = _attendanceRecords
                .FirstOrDefault(a => a.EventId == eventId && a.UserId == currentUser.Id);

            if (record == null)
            {
                return false;
            }

            record.Status = status;
            await SaveAttendanceAsync();
            
            OnAttendanceChanged?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attendance status");
            return false;
        }
    }

    public IEnumerable<Attendance> GetEventAttendance(int eventId)
    {
        return _attendanceRecords.Where(a => a.EventId == eventId).ToList();
    }

    public AttendanceStatus? GetUserAttendanceStatus(int eventId)
    {
        var currentUser = _sessionService.GetCurrentSession();
        if (currentUser == null) return null;

        var record = _attendanceRecords
            .FirstOrDefault(a => a.EventId == eventId && a.UserId == currentUser.Id);

        return record?.Status;
    }

    private async Task SaveAttendanceAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_attendanceRecords);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving attendance records");
            throw;
        }
    }

    private async void LoadAttendance()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                _attendanceRecords = JsonSerializer.Deserialize<List<Attendance>>(json) ?? new();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading attendance records");
            _attendanceRecords = new();
        }
    }
}