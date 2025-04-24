using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Event date is required")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage = "Event date must be in the future")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string Location { get; set; } = string.Empty;

    [Url(ErrorMessage = "Invalid image URL format")]
    public string ImageUrl { get; set; } = string.Empty;
}