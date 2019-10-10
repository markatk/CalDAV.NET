using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface IClient
    {
        string Username { get; }
        string Password { get; }
        Uri Uri { get; }

        Task<IEnumerable<ICalendar>> GetCalendarsAsync();
        Task<ICalendar> GetCalendarAsync(string name);
    }
}
