using System.Text.Json;
using EventEase.Models;
using Microsoft.JSInterop;

namespace EventEase.Services;

public class SessionService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<SessionService> _logger;
    private readonly string STORAGE_KEY = "user_session";
    private SessionState? _currentSession;
    public event Action<SessionState?>? OnSessionChanged;

    public SessionService(IJSRuntime jsRuntime, ILogger<SessionService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var storedSession = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(storedSession))
            {
                _currentSession = JsonSerializer.Deserialize<SessionState>(storedSession);
                if (_currentSession?.IsSessionExpired == true)
                {
                    await EndSessionAsync();
                }
                else
                {
                    _currentSession!.LastActivity = DateTime.Now;
                    await SaveSessionAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing session");
        }
    }

    public async Task StartSessionAsync(User user)
    {
        _currentSession = new SessionState
        {
            Id = user.Id,  // Add user Id to session
            IsAuthenticated = true,
            Username = user.Username,
            FullName = user.FullName,
            LastActivity = DateTime.Now,
            SessionId = Guid.NewGuid().ToString()
        };

        await SaveSessionAsync();
    }

    public async Task EndSessionAsync()
    {
        _currentSession = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", STORAGE_KEY);
        NotifySessionChanged();
    }

    public async Task UpdateLastActivityAsync()
    {
        if (_currentSession != null)
        {
            _currentSession.LastActivity = DateTime.Now;
            await SaveSessionAsync();
        }
    }

    private async Task SaveSessionAsync()
    {
        if (_currentSession != null)
        {
            var sessionJson = JsonSerializer.Serialize(_currentSession);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, sessionJson);
            NotifySessionChanged();
        }
    }

    private void NotifySessionChanged() => OnSessionChanged?.Invoke(_currentSession);

    public SessionState? GetCurrentSession() => _currentSession;

    public bool IsAuthenticated => _currentSession?.IsAuthenticated == true && !_currentSession.IsSessionExpired;
}