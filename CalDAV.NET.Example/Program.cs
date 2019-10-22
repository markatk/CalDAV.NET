using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalDAV.NET.Example.Options;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await Parser.Default
                .ParseArguments<FetchOptions, AddOptions, ListOptions>(args)
                .MapResult(
                    (FetchOptions x) => RunFetch(x),
                    (AddOptions x) => RunAdd(x),
                    (ListOptions x) => RunList(x),
                    errs => Task.FromResult(1));
        }

        private static async Task<int> RunFetch(FetchOptions options)
        {
            ICalendar calendar;

            try
            {
                calendar = await options.GetCalendarAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);

                return 1;
            }

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

        private static async Task<int> RunAdd(AddOptions options)
        {
            ICalendar calendar;

            try
            {
                calendar = await options.GetCalendarAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);

                return 1;
            }

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

            var result = await calendar.SaveChangesAsync();
            if (result == false)
            {
                Console.WriteLine("Unable to save new event");

                return 1;
            }

            Console.WriteLine("Event created");

            return 0;
        }

        private static async Task<int> RunList(ListOptions options)
        {
            var client = options.GetClient();
            if (client == null)
            {
                Console.WriteLine("Unable to connect to host");

                return 1;
            }

            IEnumerable<ICalendar> result;

            try
            {
                result = await client.GetCalendarsAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);

                return 1;
            }

            if (result == null)
            {
                Console.WriteLine("Unable to get calendars");

                return 1;
            }

            // list all calendars
            var calendars = result.ToList();

            Console.WriteLine($"Calendars ({calendars.Count()}):");

            foreach (var calendar in calendars)
            {
                Console.WriteLine($"- {calendar.DisplayName}");
            }

            return 0;
        }
    }
}
