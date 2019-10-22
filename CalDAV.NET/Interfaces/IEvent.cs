using System;

namespace CalDAV.NET.Interfaces
{
    public interface IEvent
    {
        /// <summary>
        /// Get the uid of the event.
        /// </summary>
        string Uid { get; }

        /// <summary>
        /// Get or set the stamp of the event.
        /// </summary>
        DateTime Stamp { get; set; }

        /// <summary>
        /// Get or set the start date of the event.
        /// </summary>
        DateTime Start { get; set; }

        /// <summary>
        /// Get or set the end date of the event.
        /// </summary>
        DateTime End { get; set; }

        /// <summary>
        /// Get or set the duration of the event.
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Get the creation date of the event.
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// Get the last modification date of the event.
        /// </summary>
        DateTime LastModified { get; }

        /// <summary>
        /// Get or set the location of the event.
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// Get or set the summary of the event.
        /// </summary>
        string Summary { get; set; }

        // TODO: Add alarm
        // TODO: Add missing properties like transparent, class, categories etc.
    }
}
