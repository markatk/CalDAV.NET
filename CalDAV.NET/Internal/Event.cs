using System;
using CalDAV.NET.Interfaces;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace CalDAV.NET.Internal
{
    internal class Event : IEvent
    {
        private static readonly CalendarSerializer _calendarSerializer = new CalendarSerializer();

        public string Uid => _calendarEvent.Uid;
        public DateTime Created => _calendarEvent.Created.Value;
        public DateTime LastModified => _calendarEvent.LastModified.Value;

        public DateTime Start
        {
            get => _calendarEvent.DtStart.Value;
            set => _calendarEvent.DtStart = SetDateTime(_calendarEvent.DtStart, value);
        }

        public DateTime End
        {
            get => _calendarEvent.DtEnd.Value;
            set => _calendarEvent.DtEnd = SetDateTime(_calendarEvent.DtEnd, value);
        }

        public TimeSpan Duration
        {
            get => _calendarEvent.Duration;
            set => _calendarEvent.Duration = value;
        }

        public DateTime Stamp
        {
            get => _calendarEvent.DtStamp.Value;
            set => _calendarEvent.DtStamp = SetDateTime(_calendarEvent.DtStamp, value);
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

        public override string ToString()
        {
            var content = Summary;

            if (string.IsNullOrEmpty(Location) == false)
            {
                content += $" at {Location}";
            }

            return $"{content} on {Start} till {End}";
        }

        internal string Serialize()
        {
            // TODO: Use real calendar as base
            var calendar = new Ical.Net.Calendar();
            calendar.Events.Add(_calendarEvent);

            return _calendarSerializer.SerializeToString(calendar);
        }

        private IDateTime SetDateTime(IDateTime target, DateTime value)
        {
            if (target == null)
            {
                return new CalDateTime(value);
            }

            target.Value = value;

            return target;
        }
    }
}
