using EventEase.Models;
using Microsoft.AspNetCore.Components;

namespace EventEase.Services;

public class EventState
{
    private List<Event>? _events;
    private Dictionary<int, Event> _eventCache = new();

    public async Task<IEnumerable<Event>> GetEventsAsync()
    {
        if (_events == null)
        {
            // Simulate API call delay
            await Task.Delay(100);
            _events = GetSampleEvents();
            // Cache all events
            foreach (var evt in _events)
            {
                _eventCache[evt.Id] = evt;
            }
        }
        return _events;
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        if (_eventCache.TryGetValue(id, out var cachedEvent))
        {
            return cachedEvent;
        }

        var events = await GetEventsAsync();
        return events.FirstOrDefault(e => e.Id == id);
    }

    private List<Event> GetSampleEvents()
    {
        // Move sample data here
        return new List<Event>
        {
            new Event 
            { 
                Id = 1,
                Name = "Tech Conference 2025",
                Description = "Annual technology conference featuring the latest innovations",
                Date = new DateTime(2025, 6, 15),
                Location = "Seattle Convention Center",
                ImageUrl = "https://picsum.photos/400/300?random=1"
            },
            new Event 
            { 
                Id = 2, 
                Name = "Summer Music Festival 2025",
                Description = "Join us for an incredible three-day music festival featuring top artists from around the world.",
                Date = new DateTime(2025, 7, 4),
                Location = "Central Park Amphitheater",
                ImageUrl = "https://picsum.photos/400/300?random=2"
            }
        };
    }
}