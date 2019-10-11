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
        IReadOnlyCollection<IEvent> Events { get; }

        IEvent CreateEvent(string summary, DateTime start, DateTime end = default(DateTime), string location = null);
        void DeleteEvent(IEvent calendarEvent);

        Task<bool> SaveChangesAsync();
    }
}
