using System;
using CalDAV.NET.Example.Options;
using CommandLine;

namespace CalDAV.NET.Example
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments<FetchOptions, AddOptions>(args)
                .MapResult(
                    (FetchOptions x) => RunFetch(x),
                    (AddOptions x) => RunAdd(x),
                    errs => 1);
        }

        private static int RunFetch(FetchOptions options)
        {
            var calendar = options.GetCalendar();
            if (calendar == null)
            {
                Console.WriteLine($"Calendar {options.Calendar} not found");

                return 1;
            }

            // list all events
            Console.WriteLine($"{calendar.DisplayName} ({calendar.Events.Count}):");

            foreach (var calendarEvent in calendar.Events)
            {
                Console.WriteLine($"- {calendarEvent.Summary} at {calendarEvent.Start}");
            }

            return 0;
        }

        private static int RunAdd(AddOptions options)
        {
            var calendar = options.GetCalendar();
            if (calendar == null)
            {
                Console.WriteLine($"Calendar {options.Calendar} not found");

                return 1;
            }

            // add event
            var calendarEvent = calendar.CreateEvent(options.Summary, options.Start);
            if (calendarEvent == null)
            {
                Console.WriteLine("Unable to create event");

                return 1;
            }

            var result = calendar.SaveChangesAsync().Result;
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
