using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface ICalendar
    {
        string DisplayName { get; }
        string Owner { get; }
        DateTime LastModified { get; }
        string Color { get; }

        Task<IEnumerable<IEvent>> GetEventsAsync();
        Task<IEvent> CreateEventAsync(string summary, DateTime start, DateTime end = default(DateTime), string location = null);
        Task<bool> DeleteEventAsync(IEvent calendarEvent);
    }
}
