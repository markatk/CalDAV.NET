using System;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;

namespace CalDAV.NET.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new Client(new Uri("http://localhost:5232"), "test", "test");

            // get calendar
            var calendar = await client.GetCalendarAsync("c5b6a846-7846-9e52-adec-a0bdcbfd57bf");

            await PrintEventsAsync(calendar);

            // create
            Console.WriteLine($"Create new event in {calendar.DisplayName}");

            await calendar.CreateEventAsync("Test event", DateTime.Now);

            await PrintEventsAsync(calendar);

            // delete
            var firstEvent = (await calendar.GetEventsAsync()).FirstOrDefault();

            Console.WriteLine($"Delete first event {firstEvent}");

            await calendar.DeleteEventAsync(firstEvent);

            await PrintEventsAsync(calendar);
        }

        private static async Task PrintEventsAsync(ICalendar calendar)
        {
            Console.WriteLine($"Events in {calendar.DisplayName}:");

            var events = await calendar.GetEventsAsync();

            foreach (var calendarEvent in events)
            {
                Console.WriteLine($" - {calendarEvent.Summary} on {calendarEvent.Start} till {calendarEvent.End}");
            }
        }
    }
}
