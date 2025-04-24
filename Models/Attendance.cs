using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class Attendance
{
    public int Id { get; set; }
    
    public int EventId { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    public AttendanceStatus Status { get; set; }
    
    public DateTime RegisteredAt { get; set; } = DateTime.Now;
    
    public string? Notes { get; set; }
}

public enum AttendanceStatus
{
    Registered,
    Attended,
    Cancelled,
    NoShow
}