using System;
using System.Threading.Tasks;
using CalDAV.NET.Example.Options;
using CommandLine;

namespace CalDAV.NET.Example
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var res = await Parser.Default
                .ParseArguments<FetchOptions, AddOptions, ListOptions, DeleteOptions>(args)
                .MapResult(
                    (FetchOptions x) => x.Run(),
                    (AddOptions x) => x.Run(),
                    (ListOptions x) => x.Run(),
                    (DeleteOptions x) => x.Run(),
                    errs => Task.FromResult(1));
            Console.ReadLine();
            return res;
        }
    }
}
