using System.Text.Json.Serialization;

namespace EventEase.Models;

public class SessionState
{
    public int Id { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public DateTime LastActivity { get; set; }
    public string? SessionId { get; set; }
    
    [JsonIgnore]
    public bool IsSessionExpired => LastActivity.AddHours(1) < DateTime.Now;
}