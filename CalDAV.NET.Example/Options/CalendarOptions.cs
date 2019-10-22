using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    public class CalendarOptions : BaseOptions
    {
        [Option('c', "calendar", Required = true, HelpText = "Calendar to work with")]
        public string Calendar { get; set; }

        public Task<ICalendar> GetCalendarAsync()
        {
            return GetClient().GetCalendarAsync(Calendar);
        }
    }
}
