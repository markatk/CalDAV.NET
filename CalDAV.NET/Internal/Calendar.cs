using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Enums;
using CalDAV.NET.Interfaces;

namespace CalDAV.NET.Internal
{
    internal class Calendar : ICalendar
    {
        private static readonly Regex _hrefRegex = new Regex("<[^>]*(>|$)");

        public string Uri { get; set; }

        public string Uid { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public string Owner { get; private set; }
        public DateTime LastModified { get; private set; }
        public string ProductId { get; private set; }
        public string Scale { get; private set; }
        public string Method { get; private set; }
        public string Color { get; private set; }

        public IReadOnlyCollection<IEvent> Events => _events.Where(x => x.Status != EventState.Deleted).Select(x => x as IEvent).ToList();
        public bool LocalChanges => _events.Any(x => x.Status != EventState.None);

        private string ETag { get; set; }
        private string SyncToken { get; set; }

        private readonly Ical.Net.Calendar _calendar;
        private readonly CalDAVClient _client;
        private List<Event> _events;

        private Calendar(CalDAVClient client)
        {
            _client = client;
            _calendar = new Ical.Net.Calendar();
            _events = new List<Event>();
        }

        public IEvent CreateEvent(string summary, DateTime start, DateTime end = default, string location = null)
        {
            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            var internalEvent = _calendar.Create<Ical.Net.CalendarComponents.CalendarEvent>();

            var calendarEvent = new Event(internalEvent)
            {
                Start = start,
                End = end != default ? end : start.AddHours(1),
                Summary = summary,
                Location = location,
                Status = EventState.Created
            };

            _events.Add(calendarEvent);

            return calendarEvent;
        }

        public void DeleteEvent(IEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                throw new ArgumentNullException(nameof(calendarEvent));
            }

            var internalEvent = calendarEvent as Event;
            if (internalEvent == null)
            {
                throw new ArgumentException(nameof(calendarEvent));
            }

            if (_events.Contains(internalEvent) == false)
            {
                throw new ArgumentException(nameof(calendarEvent));
            }

            internalEvent.Status = EventState.Deleted;
        }

        public async Task<IEnumerable<SaveChangesStatus>> SaveChangesAsync()
        {
            // TODO: Update calendar itself
            var status = new List<SaveChangesStatus>();

            foreach (var calendarEvent in _events)
            {
                switch (calendarEvent.Status)
                {
                    case EventState.Changed:
                        if (await UpdateEventAsync(calendarEvent) == false)
                        {
                            status.Add(new SaveChangesStatus(Error.UpdatingEventFailed, calendarEvent));
                        }

                        break;

                    case EventState.Deleted:
                        if (await DeleteEventAsync(calendarEvent) == false)
                        {
                            status.Add(new SaveChangesStatus(Error.DeletingEventFailed, calendarEvent));
                        }

                        break;

                    case EventState.Created:
                        if (await CreateEventAsync(calendarEvent) == false)
                        {
                            status.Add(new SaveChangesStatus(Error.CreatingEventFailed, calendarEvent));
                        }

                        break;
                }
            }

            return status;
        }

        internal static async Task<Calendar> Deserialize(Resource resource, string uri, CalDAVClient client)
        {
            var calendar = new Calendar(client)
            {
                Uri = uri
            };

            foreach (var property in resource.Properties)
            {
                switch (property.Key.LocalName)
                {
                    case "displayname":
                        calendar.DisplayName = property.Value;

                        break;

                    case "owner":
                        calendar.Owner = property.Value.Contains("href") ? _hrefRegex.Replace(property.Value, "") : property.Value;

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

                    case "calendar-color":
                        calendar.Color = property.Value;

                        break;

                    case "calendar-description":
                        calendar.Description = property.Value;

                        break;
                }
            }

            calendar.Uid = uri
                .Replace(calendar.Owner, "")
                .Replace("/", "");

            // fetch events
            var events = await calendar.GetEventsAsync().ConfigureAwait(false);
            calendar._events = events.ToList();

            return calendar;
        }

        private async Task<bool> UpdateEventAsync(IEvent calendarEvent)
        {
            var internalEvent = calendarEvent as Event;
            if (internalEvent == null)
            {
                return false;
            }

            var result = await _client
                .Put(GetEventUrl(calendarEvent), internalEvent.Serialize(_calendar))
                .SendAsync()
                .ConfigureAwait(false);

            return result.IsSuccessful;
        }

        private async Task<bool> DeleteEventAsync(IEvent calendarEvent)
        {
            var result = await _client
                .Delete(GetEventUrl(calendarEvent))
                .SendAsync()
                .ConfigureAwait(false);

            return result.IsSuccessful;
        }

        private async Task<IEnumerable<Event>> GetEventsAsync()
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

        private async Task<bool> CreateEventAsync(Event calendarEvent)
        {
            var result = await _client
                .Put(GetEventUrl(calendarEvent), calendarEvent.Serialize(_calendar))
                .SendAsync()
                .ConfigureAwait(false);

            return result.IsSuccessful;
        }

        private string GetEventUrl(IEvent calendarEvent)
        {
            return $"{Uri}/{calendarEvent.Uid}.ics";
        }
    }
}
