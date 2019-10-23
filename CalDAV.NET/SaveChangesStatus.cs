using CalDAV.NET.Interfaces;
using CalDAV.NET.Enums;

namespace CalDAV.NET
{
    public class SaveChangesStatus
    {
        public string Message { get; }
        public Error Error { get; }
        public IEvent Event { get; }

        internal SaveChangesStatus(Error error, IEvent calendarEvent)
        {
            // TODO: Add internal error
            Error = error;
            Event = calendarEvent;
            Message = GetMessage(error);
        }

        private string GetMessage(Error error)
        {
            switch (error)
            {
                case Error.CreatingEventFailed:
                    return "Calendar event could not be created";

                case Error.DeletingEventFailed:
                    return "Calendar event could not be deleted";

                case Error.UpdatingEventFailed:
                    return "Calendar event could not be updated";

                default:
                    return "";
            }
        }
}
}
