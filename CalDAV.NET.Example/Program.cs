using System;
using System.Threading.Tasks;

namespace CalDAV.NET.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new Client(new Uri("http://localhost:5232"), "test", "test");

            var calendar = await client.GetCalendarAsync("c5b6a846-7846-9e52-adec-a0bdcbfd57bf");
            var events = await calendar.GetEventsAsync();

            Console.WriteLine($"Create new event in {calendar.DisplayName}");

            await calendar.CreateEventAsync("Test event", DateTime.Now);

            Console.WriteLine($"Events in {calendar.DisplayName}:");

            foreach (var calendarEvent in events)
            {
                Console.WriteLine($" - {calendarEvent.Summary} on {calendarEvent.Start} till {calendarEvent.End}");
            }
        }
    }
}
