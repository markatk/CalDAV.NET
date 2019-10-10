using System;
using System.Collections.Generic;
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
            _client.SetAuthorization(Username, Password);
        }

        public async Task<IEnumerable<ICalendar>> GetCalendarsAsync()
        {
            var userUri = await GetUserUri();

            return null;
        }

        public async Task<ICalendar> GetCalendarAsync(string name)
        {
            // create body
            var propfind = new XElement(Constants.DavNS + "propfind", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNS));
            propfind.Add(new XElement(Constants.DavNS + "allprop"));

            var result = await _client
                .Propfind($"{Username}/{name}", propfind)
                .SendAsync()
                .ConfigureAwait(false);

            if (result.IsSuccessful == false)
            {
                return null;
            }

            var resource = result.Resources.FirstOrDefault();

            var calendar = Calendar.Deserialize(resource, _client);
            calendar.Name = name;
            calendar.Username = Username;

            return calendar;
        }

        private async Task<string> GetUserUri()
        {
            // create body
            var prop = new XElement(Constants.DavNS + "prop");
            prop.Add(new XElement(Constants.DavNS + "current-user-principal"));

            var root = new XElement(Constants.DavNS + "propfind", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNS));
            root.Add(prop);

            var result = await _client
                .Propfind("", root)
                .WithHeader("Depth", "0")
                .SendAsync()
                .ConfigureAwait(false);

            if (result.IsSuccessful == false)
            {
                return null;
            }

            return null;
        }
    }
}
