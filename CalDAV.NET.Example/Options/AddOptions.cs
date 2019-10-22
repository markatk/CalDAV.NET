using System;
using System.Net.Http;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    [Verb("add", HelpText = "Add a new event to a calendar")]
    public class AddOptions : CalendarOptions
    {
        [Value(0, MetaName = "summary", Required = true)]
        public string Summary { get; set; }

        [Value(1, MetaName = "start", Required = true)]
        public DateTime Start { get; set; }

        [Value(2, MetaName = "end")]
        public DateTime End { get; set; }

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

            // add event
            var calendarEvent = calendar.CreateEvent(Summary, Start);
            if (calendarEvent == null)
            {
                Console.WriteLine("Unable to create event");

                return 1;
            }

            var result = await calendar.SaveChangesAsync();
            if (result == false)
            {
                Console.WriteLine("Unable to save new event");

                return 1;
            }

            Console.WriteLine("Event created");

            return 0;
        }
    }
}
