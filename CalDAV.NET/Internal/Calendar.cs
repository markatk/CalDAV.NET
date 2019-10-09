using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using Ical.Net.Serialization;

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
        private readonly CalDAVClient _client;

        private Calendar(CalDAVClient client)
        {
            _client = client;
            _calendar = new Ical.Net.Calendar();
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync()
        {
            var events = new List<IEvent>();



            return events;
        }

        public string Serialize()
        {
            return _serializer.SerializeToString(_calendar);
        }

        public static Calendar Deserialize(Resource resource, CalDAVClient client)
        {
            var calendar = new Calendar(client);

            foreach (var property in resource.Properties)
            {
                if (property.Key.LocalName == "displayname")
                {
                    calendar.Name = property.Value;
                }
                else if (property.Key.LocalName == "owner")
                {
                    calendar.Owner = property.Value;
                }
                else if (property.Key.LocalName == "getetag")
                {
                    calendar.ETag = property.Value;
                }
                else if (property.Key.LocalName == "getlastmodified")
                {
                    calendar.LastModified = DateTime.Parse(property.Value);
                }
                else if (property.Key.LocalName == "sync-token")
                {
                    calendar.SyncToken = property.Value;
                }
            }

            return calendar;
        }
    }
}
