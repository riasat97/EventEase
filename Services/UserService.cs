using EventEase.Models;
using System.Text.Json;
using Microsoft.JSInterop;

namespace EventEase.Services;

public class UserService
{
    private List<User> _users = new();
    private readonly string STORAGE_KEY = "registered_users";
    private readonly ILogger<UserService> _logger;
    private readonly IJSRuntime _jsRuntime;

    public UserService(ILogger<UserService> logger, IJSRuntime jsRuntime)
    {
        _logger = logger;
        _jsRuntime = jsRuntime;
        LoadUsers();
    }

    public async Task<(bool success, string message)> RegisterUserAsync(User user)
    {
        try
        {
            // Check if username is taken
            if (_users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Username is already taken");
            }

            // Check if email is taken
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Email address is already registered");
            }

            // Assign an ID and add user
            user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            user.RegistrationDate = DateTime.Now;
            
            _users.Add(user);
            await SaveUsers();

            return (true, "Registration successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return (false, "An error occurred during registration. Please try again.");
        }
    }

    public async Task<User?> ValidateUserAsync(string usernameOrEmail, string password)
    {
        try
        {
            // Simulate delay for API call
            await Task.Delay(100);
            
            var user = _users.FirstOrDefault(u => 
                (u.Username.Equals(usernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                 u.Email.Equals(usernameOrEmail, StringComparison.OrdinalIgnoreCase)) &&
                u.Password.Equals(password));

            if (user != null)
            {
                _logger.LogInformation($"User {user.Username} logged in successfully");
                return user;
            }

            _logger.LogWarning($"Failed login attempt for username/email: {usernameOrEmail}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user validation");
            throw;
        }
    }

    private async Task SaveUsers()
    {
        try
        {
            var json = JsonSerializer.Serialize(_users);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving users to local storage");
            throw;
        }
    }

    private async void LoadUsers()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users from local storage");
            _users = new();
        }
    }
}