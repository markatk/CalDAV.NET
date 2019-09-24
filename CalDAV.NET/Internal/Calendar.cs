using System;
using CalDAV.NET.Interfaces;
using Ical.Net.Serialization;
using WebDav;

namespace CalDAV.NET.Internal
{
    internal class Calendar : ICalendar
    {
        private static readonly CalendarSerializer _serializer = new CalendarSerializer();

        public string Name
        {
            get => _calendar.Name;
            private set => _calendar.Name = value;
        }
        public string Owner { get; private set; }
        public DateTime LastModified { get; private set; }
        public string Color { get; private set; }

        private string ETag { get; set; }
        private string SyncToken { get; set; }

        private readonly Ical.Net.Calendar _calendar;

        private Calendar()
        {
            _calendar = new Ical.Net.Calendar();
        }

        public string Serialize()
        {
            return _serializer.SerializeToString(_calendar);
        }

        public static Calendar Deserialize(WebDavResource resource)
        {
            var calendar = new Calendar();

            foreach (var property in resource.Properties)
            {
                if (property.Name.LocalName == "displayname")
                {
                    calendar.Name = property.Value;
                }
                else if (property.Name.LocalName == "owner")
                {
                    calendar.Owner = property.Value;
                }
                else if (property.Name.LocalName == "getetag")
                {
                    calendar.ETag = property.Value;
                }
                else if (property.Name.LocalName == "getlastmodified")
                {
                    calendar.LastModified = DateTime.Parse(property.Value);
                }
                else if (property.Name.LocalName == "sync-token")
                {
                    calendar.SyncToken = property.Value;
                }
            }

            return calendar;
        }
    }
}
