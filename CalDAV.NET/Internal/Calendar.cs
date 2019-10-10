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

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            document.Add(query);

            var result = await _client.ReportAsync($"{Username}/{Name}", document);

            // parse events
            return result.Resources
                .SelectMany(x => x.Properties)
                .Where(x => x.Key.LocalName == "calendar-data")
                .SelectMany(x => Ical.Net.Calendar.Load<Ical.Net.Calendar>(x.Value))
                .SelectMany(x => x.Events)
                .Select(internalEvent => new Event(internalEvent));
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
                    calendar.DisplayName = property.Value;
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
