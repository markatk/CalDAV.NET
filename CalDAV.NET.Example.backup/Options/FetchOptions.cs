using System;
using System.Net.Http;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    [Verb("fetch", HelpText = "Fetch all events from a calendar")]
    public class FetchOptions : CalendarOptions
    {
        public override async Task<int> Run()
        {
            ICalendar calendar;

            try
            {
                calendar = await GetCalendarAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);

                return 1;
            }

            if (calendar == null)
            {
                Console.WriteLine($"Calendar {Calendar} not found");

                return 1;
            }

            // list all events
            Console.WriteLine($"{calendar.DisplayName} ({calendar.Events.Count}):");

            foreach (var calendarEvent in calendar.Events)
            {
                Console.WriteLine($"- {calendarEvent.Summary} ({calendarEvent.Uid}) at {calendarEvent.Start}");
            }

            return 0;
        }
    }
}
