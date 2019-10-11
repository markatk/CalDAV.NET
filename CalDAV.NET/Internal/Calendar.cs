using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Interfaces;
using Ical.Net.Serialization;

namespace CalDAV.NET.Internal
{
    internal class Calendar : ICalendar
    {
        private static readonly CalendarSerializer _serializer = new CalendarSerializer();

        public string Uri { get; set; }

        public string DisplayName { get; private set; }

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
            // create body
            var query = new XElement(Constants.CalNs + "calendar-query", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNs), new XAttribute(XNamespace.Xmlns + "c", Constants.CalNs));

            var prop = new XElement(Constants.DavNs + "prop");
            prop.Add(new XElement(Constants.DavNs + "getetag"));
            prop.Add(new XElement(Constants.CalNs + "calendar-data"));
            query.Add(prop);

            var filter = new XElement(Constants.CalNs + "filter");
            filter.Add(new XElement(Constants.CalNs + "comp-filter", new XAttribute("name", "VCALENDAR")));
            query.Add(filter);

            var result = await _client
                .Report(Uri, query)
                .SendAsync()
                .ConfigureAwait(false);

            // parse events
            return result.Resources
                .SelectMany(x => x.Properties)
                .Where(x => x.Key.LocalName == "calendar-data")
                .SelectMany(x => Ical.Net.Calendar.Load<Ical.Net.Calendar>(x.Value))
                .SelectMany(x => x.Events)
                .Select(internalEvent => new Event(internalEvent));
        }

        public async Task<IEvent> CreateEventAsync(string summary, DateTime start, DateTime end = default, string location = null)
        {
            var internalEvent = _calendar.Create<Ical.Net.CalendarComponents.CalendarEvent>();

            var calendarEvent = new Event(internalEvent)
            {
                Start = start,
                End = end != default ? end : start.AddHours(1),
                Summary = summary,
                Location = location
            };

            var result = await _client
                .Put(GetEventUrl(calendarEvent), calendarEvent.Serialize())
                .SendAsync()
                .ConfigureAwait(false);

            return result.IsSuccessful ? calendarEvent : null;
        }

        public async Task<bool> UpdateEventAsync(IEvent calendarEvent)
        {
            var internalEvent = calendarEvent as Event;
            if (internalEvent == null)
            {
                return false;
            }

            var result = await _client
                .Put(GetEventUrl(calendarEvent), internalEvent.Serialize())
                .SendAsync()
                .ConfigureAwait(false);

            return result.IsSuccessful;
        }

        public async Task<bool> DeleteEventAsync(IEvent calendarEvent)
        {
            var result = await _client
                .Delete(GetEventUrl(calendarEvent))
                .SendAsync()
                .ConfigureAwait(false);

            return result.IsSuccessful;
        }

        internal string Serialize()
        {
            return _serializer.SerializeToString(_calendar);
        }

        internal static Calendar Deserialize(Resource resource, CalDAVClient client)
        {
            var calendar = new Calendar(client);

            foreach (var property in resource.Properties)
            {
                switch (property.Key.LocalName)
                {
                    case "displayname":
                        calendar.DisplayName = property.Value;
                        break;

                    case "owner":
                        calendar.Owner = property.Value;
                        break;

                    case "getetag":
                        calendar.ETag = property.Value;
                        break;

                    case "getlastmodified":
                        calendar.LastModified = DateTime.Parse(property.Value);
                        break;

                    case "sync-token":
                        calendar.SyncToken = property.Value;
                        break;
                }
            }

            return calendar;
        }

        private string GetEventUrl(IEvent calendarEvent)
        {
            return $"{Uri}/{calendarEvent.Uid}.ics";
        }
    }
}
