using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    public abstract class CalendarOptions : BaseOptions
    {
        [Option('c', "calendar", Required = true, HelpText = "Calendar to work with")]
        public string Calendar { get; set; }

        protected Task<ICalendar> GetCalendarAsync()
        {
            return GetClient().GetCalendarAsync(Calendar);
        }
    }
}
