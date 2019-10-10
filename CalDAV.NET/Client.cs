using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Interfaces;
using CalDAV.NET.Internal;

namespace CalDAV.NET
{
    public class Client : IClient
    {
        public string Username { get; }
        public string Password { get; }
        public Uri Uri { get; }

        private static readonly CalDAVClient _client = new CalDAVClient();

        public Client(Uri uri, string username, string password)
        {
            Uri = uri;
            Username = username;
            Password = password;

            _client.BaseUri = uri;
        }

        public async Task<ICalendar> GetCalendarAsync(string name)
        {
            // create body
            var propfind = new XElement(Constants.DavNS + "propfind", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNS));
            propfind.Add(new XElement(Constants.DavNS + "allprop"));

            var result = await _client.PropfindAsync($"{Username}/{name}", propfind);

            if (result.IsSuccessful == false)
            {
                Console.WriteLine($"Unable to find calendar {name}: {result}");

                return null;
            }

            var resource = result.Resources.FirstOrDefault();

            var calendar = Calendar.Deserialize(resource, _client);
            calendar.Name = name;
            calendar.Username = Username;

            return calendar;
        }
    }
}
