using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface IClient
    {
        /// <summary>
        /// Get the username to authenticate with.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Get the password to authenticate with.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Get the uri of the server to connect to.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Get all calendars available for the current user (or unauthenticated).
        /// </summary>
        /// <returns>Collection of available calendars</returns>
        Task<IEnumerable<ICalendar>> GetCalendarsAsync();

        /// <summary>
        /// Get a calendar by given uid.
        /// </summary>
        /// <param name="uid">Uid of the calendar to get</param>
        /// <returns>Calendar or null if none was found</returns>
        /// <exception cref="ArgumentNullException">Thrown if uid is null or empty</exception>
        Task<ICalendar> GetCalendarAsync(string uid);

        /// <summary>
        /// Get the default calendar for the given user.
        /// </summary>
        /// <returns>Calendar or null if none was found</returns>
        Task<ICalendar> GetDefaultCalendarAsync();
    }
}
