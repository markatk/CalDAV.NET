using System;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface IClient
    {
        string Username { get; }
        string Password { get; }
        Uri Uri { get; }

        Task<ICalendar> GetCalendarAsync(string name);
    }
}
