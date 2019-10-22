using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    public abstract class CalendarOptions : BaseOptions
    {
        [Option('c', "calendar", SetName = "calendar", HelpText = "Calendar to work with")]
        public string Calendar { get; set; }

        [Option('d', "default", SetName = "calendar", HelpText = "Use default calendar")]
        public bool Default { get; set; }

        protected Task<ICalendar> GetCalendarAsync()
        {
            var client = GetClient();

            return Default ? client.GetDefaultCalendarAsync() : client.GetCalendarAsync(Calendar);
        }
    }
}
