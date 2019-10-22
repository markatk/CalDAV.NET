using System;
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
    }
}
