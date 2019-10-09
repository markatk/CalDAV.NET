using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalDAV.NET.Interfaces
{
    public interface ICalendar
    {
        Task<IEnumerable<IEvent>> GetEventsAsync();
    }
}
