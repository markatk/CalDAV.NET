using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface ICalendar
    {
        /// <summary>
        /// Get the uid of the calendar.
        /// </summary>
        string Uid { get; }

        /// <summary>
        /// Get the display name of the calendar.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Get the description of the calendar.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get the owner of the calendar.
        /// </summary>
        string Owner { get; }

        /// <summary>
        /// Get date of last modification of the calendar.
        /// </summary>
        DateTime LastModified { get; }

        /// <summary>
        /// Get the product id of the calendar.
        /// </summary>
        string ProductId { get; }

        /// <summary>
        /// Get the scale of the calendar.
        /// </summary>
        string Scale { get; }

        /// <summary>
        /// Get the method of the calendar.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Get the color of the calendar.
        /// </summary>
        string Color { get; }

        /// <summary>
        /// Get all events in the calendar.
        /// </summary>
        ///
        /// The events may not represent the state on the server since local changes may not be synchronized.
        /// With <see cref="LocalChanges" /> a calendar can be checked whether it contains any changes.
        ///
        /// To synchronize the calendar use <see cref="SaveChangesAsync" />.
        IReadOnlyCollection<IEvent> Events { get; }

        /// <summary>
        /// Check if the calendar has local changes which are not synchronized yet.
        /// </summary>
        ///
        /// To synchronize local changes with the server use <see cref="SaveChangesAsync" />.
        bool LocalChanges { get; }

        /// <summary>
        /// Create a new event in the calendar.
        /// </summary>
        ///
        /// If no end is set the event will default to one hour duration.
        ///
        /// Attention: The event will not be synchronized until <see cref="SaveChangesAsync" /> is called.
        ///
        /// <param name="summary">Summary text of the event</param>
        /// <param name="start">Start date and time of the event</param>
        /// <param name="end">End date and time of the event</param>
        /// <param name="location">Location of the event</param>
        /// <returns>New created event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="summary" /> is null</exception>
        IEvent CreateEvent(string summary, DateTime start, DateTime end = default(DateTime), string location = null);

        /// <summary>
        /// Delete a given event from the calendar.
        /// </summary>
        ///
        /// Attention: The event will not be synchronized (deleted on the server) until <see cref="SaveChangesAsync" /> is called.
        ///
        /// <param name="calendarEvent">Event to delete from the calendar</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="calendarEvent"/> is null</exception>
        void DeleteEvent(IEvent calendarEvent);

        /// <summary>
        /// Save all local changes to the server.
        /// </summary>
        /// <returns>True if all changes could be applied, otherwise false.</returns>
        Task<IEnumerable<SaveChangesStatus>> SaveChangesAsync();
    }
}
