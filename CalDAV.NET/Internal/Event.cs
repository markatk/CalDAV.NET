using System;
using CalDAV.NET.Interfaces;
using Ical.Net.CalendarComponents;

namespace CalDAV.NET.Internal
{
    internal class Event : IEvent
    {
        public string Uid => _calendarEvent.Uid;
        public DateTime Created => _calendarEvent.Created.Value;
        public DateTime LastModified => _calendarEvent.LastModified.Value;

        public DateTime Start
        {
            get => _calendarEvent.DtStart.Value;
            set => _calendarEvent.DtStart.Value = value;
        }

        public DateTime End
        {
            get => _calendarEvent.DtEnd.Value;
            set => _calendarEvent.DtEnd.Value = value;
        }

        public TimeSpan Duration
        {
            get => _calendarEvent.Duration;
            set => _calendarEvent.Duration = value;
        }

        public DateTime Stamp
        {
            get => _calendarEvent.DtStamp.Value;
            set => _calendarEvent.DtStamp.Value = value;
        }

        public string Location
        {
            get => _calendarEvent.Location;
            set => _calendarEvent.Location = value;
        }

        public string Summary
        {
            get => _calendarEvent.Summary;
            set => _calendarEvent.Summary = value;
        }

        private readonly CalendarEvent _calendarEvent;

        public Event(CalendarEvent calendarEvent)
        {
            _calendarEvent = calendarEvent;
        }
    }
}
