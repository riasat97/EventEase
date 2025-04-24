using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is DateTime date && date > DateTime.Now;
    }
}