using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    [Verb("delete", HelpText = "Delete an event from a calendar")]
    public class DeleteOptions : EventOptions
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

            // delete event with given name
            var calendarEvent = calendar.Events.FirstOrDefault(x => x.Uid == Event);
            if (calendarEvent == null)
            {
                Console.WriteLine($"No event found with uid {Event}");

                return 0;
            }

            calendar.DeleteEvent(calendarEvent);

            var result = await calendar.SaveChangesAsync();
            if (result == false)
            {
                Console.WriteLine("Unable to delete event");

                return 1;
            }

            return 0;
        }
    }
}
