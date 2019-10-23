using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Interfaces;
using CalDAV.NET.Internal;

namespace CalDAV.NET
{
    public class Client : IClient
    {
        private readonly Regex _tagRegex = new Regex("<[^>]*(>|$)");

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
            if (string.IsNullOrEmpty(userUri))
            {
                return null;
            }

            // create body
            var prop = new XElement(Constants.DavNs + "prop");
            prop.Add(new XElement(Constants.DavNs + "resourcetype"));
            prop.Add(new XElement(Constants.DavNs + "displayname"));
            prop.Add(new XElement(Constants.ServerNs + "getctag"));
            prop.Add(new XElement(Constants.CalNs + "supported-calendar-component-set"));

            var root = new XElement(
                Constants.DavNs + "propfind",
                new XAttribute(XNamespace.Xmlns + "d", Constants.DavNs),
                new XAttribute(XNamespace.Xmlns + "c", Constants.CalNs),
                new XAttribute(XNamespace.Xmlns + "cs", Constants.ServerNs));
            root.Add(prop);

            var result = await _client
                .Propfind(userUri, root)
                .SendAsync()
                .ConfigureAwait(false);

            if (result.IsSuccessful == false)
            {
                return null;
            }

            // get all calendars by returned urls
            var calendars = new List<ICalendar>();

            foreach (var resource in result.Resources)
            {
                var calendar = await GetCalendarWithUriAsync(resource.Uri);
                if (calendar == null)
                {
                    continue;
                }

                calendars.Add(calendar);
            }

            return calendars;
        }

        public Task<ICalendar> GetCalendarAsync(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException(nameof(uid));
            }

            return GetCalendarWithUriAsync($"{Username}/{uid}");
        }

        public async Task<ICalendar> GetDefaultCalendarAsync()
        {
            var userUri = await GetUserUri();
            if (string.IsNullOrEmpty(userUri))
            {
                return null;
            }

            // create body
            var prop = new XElement(Constants.DavNs + "prop");
            prop.Add(new XElement(Constants.DavNs + "resourcetype"));
            prop.Add(new XElement(Constants.DavNs + "displayname"));
            prop.Add(new XElement(Constants.ServerNs + "getctag"));
            prop.Add(new XElement(Constants.CalNs + "supported-calendar-component-set"));

            var root = new XElement(
                Constants.DavNs + "propfind",
                new XAttribute(XNamespace.Xmlns + "d", Constants.DavNs),
                new XAttribute(XNamespace.Xmlns + "c", Constants.CalNs),
                new XAttribute(XNamespace.Xmlns + "cs", Constants.ServerNs));
            root.Add(prop);

            var result = await _client
                .Propfind(userUri, root)
                .WithHeader("Depth", "0")
                .SendAsync()
                .ConfigureAwait(false);

            if (result.IsSuccessful == false)
            {
                return null;
            }

            var resource = result.Resources.FirstOrDefault();
            if (resource == null)
            {
                return null;
            }

            return await GetCalendarWithUriAsync(resource.Uri);;
        }

        private async Task<ICalendar> GetCalendarWithUriAsync(string uri)
        {
            // create body
            var propfind = new XElement(Constants.DavNs + "propfind", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNs));
            propfind.Add(new XElement(Constants.DavNs + "allprop"));

            var result = await _client
                .Propfind(uri, propfind)
                .SendAsync()
                .ConfigureAwait(false);

            if (result.IsSuccessful == false)
            {
                return null;
            }

            var resource = result.Resources.FirstOrDefault();

            // check if resource really is a calendar
            var contentType = resource?.Properties.FirstOrDefault(x => x.Key.LocalName == "getcontenttype");
            if (contentType.HasValue == false || contentType.Value.Value != "text/calendar")
            {
                return null;
            }

            var calendar = await Calendar.Deserialize(resource, uri, _client);

            return calendar;
        }

        private async Task<string> GetUserUri()
        {
            // create body
            var prop = new XElement(Constants.DavNs + "prop");
            prop.Add(new XElement(Constants.DavNs + "current-user-principal"));

            var root = new XElement(Constants.DavNs + "propfind", new XAttribute(XNamespace.Xmlns + "d", Constants.DavNs));
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

            var resource = result.Resources.FirstOrDefault();
            if (resource == null)
            {
                return null;
            }

            foreach (var keyValue in resource.Properties)
            {
                if (keyValue.Key.LocalName == "current-user-principal")
                {
                    return _tagRegex.Replace(keyValue.Value, "");
                }
            }

            return null;
        }
    }
}
