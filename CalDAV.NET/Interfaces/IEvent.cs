using System;

namespace CalDAV.NET.Interfaces
{
    public interface IEvent
    {
        string Uid { get; }
        DateTime Start { get; set; }
        DateTime End { get; set; }
        TimeSpan Duration { get; set; }
        DateTime Created { get; }
        DateTime Stamp { get; set; }
        DateTime LastModified { get; }
        string Location { get; set; }
        string Summary { get; set; }

        // TODO: Add alarm
        // TODO: Add missing properties like transparent, class, categories etc.
    }
}
