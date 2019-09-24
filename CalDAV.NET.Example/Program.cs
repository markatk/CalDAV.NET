using System;
using System.Threading.Tasks;

namespace CalDAV.NET.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client(new Uri("http://localhost:5232"), "test", "test");

            var calendar = await client.GetCalendarAsync("c5b6a846-7846-9e52-adec-a0bdcbfd57bf");
        }
    }
}
