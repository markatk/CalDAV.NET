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

        public string Name
        {
            get => _calendar.Name;
            internal set => _calendar.Name = value;
        }

        public string DisplayName { get; private set; }

        public string Owner { get; private set; }
        public DateTime LastModified { get; private set; }
        public string Color { get; private set; }

        internal string Username { get; set; }

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
            var query = new XElement(Constants.CalNS + "calendar-query", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNS), new XAttribute(XNamespace.Xmlns + "c", Constants.CalNS));

            var prop = new XElement(Constants.DavNS + "prop");
            prop.Add(new XElement(Constants.DavNS + "getetag"));
            prop.Add(new XElement(Constants.CalNS + "calendar-data"));
            query.Add(prop);

            var filter = new XElement(Constants.CalNS + "filter");
            filter.Add(new XElement(Constants.CalNS + "comp-filter", new XAttribute("name", "VCALENDAR")));
            query.Add(filter);

            var result = await _client
                .Report(GetCalendarUrl(), query)
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

        private string GetCalendarUrl()
        {
            return $"{Username}/{Name}";
        }

        private string GetEventUrl(IEvent calendarEvent)
        {
            return $"{Username}/{Name}/{calendarEvent.Uid}.ics";
        }
    }
}
