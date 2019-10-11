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

            // get all calendars
            Console.WriteLine("Calendars:");

            var calendars = await client.GetCalendarsAsync();

            foreach (var cal in calendars)
            {
                Console.WriteLine($" - {cal.DisplayName}, Events: {cal.Events.Count()}");
            }

            // get calendar
            var calendar = await client.GetCalendarAsync("c5b6a846-7846-9e52-adec-a0bdcbfd57bf");

            PrintEvents(calendar);

            // create
            Console.WriteLine($"Create new event in {calendar.DisplayName}");

            var newEvent = calendar.CreateEvent("Test event", DateTime.Now);

            PrintEvents(calendar);

            // update
            newEvent.Start = DateTime.Now.AddDays(2);
            newEvent.End = newEvent.Start.AddDays(1);

            PrintEvents(calendar);

            // delete
            var firstEvent = calendar.Events.FirstOrDefault();

            Console.WriteLine($"Delete first event {firstEvent}");

            calendar.DeleteEvent(firstEvent);

            PrintEvents(calendar);

            // save changes to remote
            await calendar.SaveChangesAsync();
        }

        private static void PrintEvents(ICalendar calendar)
        {
            Console.WriteLine($"Events in {calendar.DisplayName}:");

            foreach (var calendarEvent in calendar.Events)
            {
                Console.WriteLine($" - {calendarEvent.Summary} on {calendarEvent.Start} till {calendarEvent.End}");
            }
        }
    }
}
