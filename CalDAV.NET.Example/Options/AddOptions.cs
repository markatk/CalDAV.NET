using System;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    [Verb("add", HelpText = "Add a new event to a calendar")]
    public class AddOptions : CalendarOptions
    {
        [Option('s', "summary", Required = true, HelpText = "Summary of the new event")]
        public string Summary { get; set; }

        [Option('S', "start", Required = true, HelpText = "Start date and time of the event")]
        public DateTime Start { get; set; }
    }
}
