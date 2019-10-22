using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    [Verb("list", HelpText = "List all available calendars")]
    public class ListOptions : BaseOptions
    {
        public override async Task<int> Run()
        {
            var client = GetClient();
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
