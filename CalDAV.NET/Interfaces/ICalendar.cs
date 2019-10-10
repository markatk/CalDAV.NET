using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface ICalendar
    {
        string Name { get; }
        string DisplayName { get; }
        string Owner { get; }
        DateTime LastModified { get; }
        string Color { get; }

        Task<IEnumerable<IEvent>> GetEventsAsync();
    }
}
