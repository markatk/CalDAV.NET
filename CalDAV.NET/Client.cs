using System;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.NET.Interfaces;
using CalDAV.NET.Internal;

namespace CalDAV.NET
{
    public class Client : IClient
    {
        public string Username { get; }
        public string Password { get; }
        public Uri Url { get; }

        private static readonly CalDAVClient _client = new CalDAVClient();

        public Client(Uri url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }

        public async Task<ICalendar> GetCalendarAsync(string name)
        {
            var result = await _client.PropfindAsync(new Uri($"{Url}{Username}/{name}"));

            if (result.IsSuccessful == false)
            {
                Console.WriteLine($"Unable to find calendar {name}: {result}");

                return null;
            }

            var resource = result.Resources.FirstOrDefault();

            return Calendar.Deserialize(resource, _client);
        }
    }
}
