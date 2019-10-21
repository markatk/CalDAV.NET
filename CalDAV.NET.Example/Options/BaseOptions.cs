using System;
using CalDAV.NET.Interfaces;
using CommandLine;

namespace CalDAV.NET.Example.Options
{
    public class BaseOptions
    {
        [Option('h', "host", Required = true, HelpText = "Host to connect to")]
        public string Host { get; set; }

        [Option('u', "username", HelpText = "Username to authenticate with")]
        public string Username { get; set; }

        [Option('p', "password", HelpText = "Password to authenticate with")]
        public string Password { get; set; }

        public IClient GetClient()
        {
            return new Client(new Uri(Host), Username, Password);
        }
    }
}
