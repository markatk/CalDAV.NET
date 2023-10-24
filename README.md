# CalDAV.NET

[![Build status](https://ci.appveyor.com/api/projects/status/le4dlvu54wvpufqh?svg=true)](https://ci.appveyor.com/project/markatk/caldav-net)
[![Nuget](https://img.shields.io/nuget/dt/CalDAV.NET)](https://www.nuget.org/packages/CalDAV.NET/)

## Description

Asynchronous CalDAV .NET client library build on .NET Standard. Heavily influenced by [WebDAVClient](https://github.com/skazantsev/WebDavClient) but was not able to use it because of different request methods required by CalDAV.

## Installation

Install CalDAV.NET via [NuGet](https://www.nuget.org/packages/CalDAV.NET/)

```
Install-Package CalDAV.NET
```

## Usage
```csharp
// Connect to server.
var serverUrl = new Uri("https://example.com/calendars");
var client = new Client(serverUrl, "Benutzername", "Passwort");

// Get calendar from server.
var calendar = await client.GetCalendarAsync("59e336cc-a919-46b6-a142-7ed1045dd135");

// Update events.
foreach (var @event in calendar.Events)
{
    if (@event.Summary != "TestSummary")
    {
        continue;
    }

    @event.Summary = "Test";

    // Save changes to server.
    var result = await calendar.SaveChangesAsync();

    if (!result)
    {
        Console.WriteLine("Unable to save changed event");
        return;
    }
}

// Create a new event.
var calendarEvent = calendar.CreateEvent("Mein Termin", DateTime.Now, DateTime.Now.AddHours(1), "Test");

if (calendarEvent == null)
{
    Console.WriteLine("Unable to create event");
    return;
}

// Save changes to server.
var result2 = await calendar.SaveChangesAsync();

if (!result2)
{
    Console.WriteLine("Unable to save created event");
    return;
}

// Delete an event.
var firstEvent = calendar.Events.FirstOrDefault();

if (firstEvent is not null)
{
    calendar.DeleteEvent(firstEvent);

    // Save changes to server.
    var result3 = await calendar.SaveChangesAsync();

    if (!result3)
    {
        Console.WriteLine("Unable to save created event");
        return;
    }
}
```

## License

MIT License

Copyright (c) 2019 MarkAtk

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
