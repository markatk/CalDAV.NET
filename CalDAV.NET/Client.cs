using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CalDAV.NET.Internal;
using WebDav;

namespace CalDAV.NET
{
    public class Client : IClient
    {
        public string Username { get; }
        public string Password { get; }
        public Uri Url { get; }

        private readonly IWebDavClient _client;

        public Client(Uri url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;

            _client = new WebDavClient(new WebDavClientParams
            {
                BaseAddress = Url,
                Credentials = new NetworkCredential(username, password)
            });
        }

        public async Task<ICalendar> GetCalendarAsync(string name)
        {
            var result = await _client.Propfind($"{Username}/{name}", new PropfindParameters
            {

            });

            if (result.IsSuccessful == false)
            {
                Console.WriteLine($"Unable to find calendar {name}: {result}");

                return null;
            }

            var resource = result.Resources.FirstOrDefault();

            return Calendar.Deserialize(resource);
        }
    }
}
