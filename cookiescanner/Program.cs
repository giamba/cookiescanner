using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;

namespace cookiescanner
{
    class Program
    {
        public class Options
        {
            //[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            //public bool Verbose { get; set; }
            [Option('f', "file", Required = true, HelpText = "Cookie input file.")]
            public string File { get; set; }

            [Option('d', "Date", Required = true, HelpText = "Date")]
            public string Date { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
           .WithParsed(Run)
           .WithNotParsed(HandleParseError);
        }

        static void Run(Options opts)
        {
            string currDir = Directory.GetCurrentDirectory();
            
            CookieScanner cookieScanner = new CookieScanner(opts.File);

            var lines = cookieScanner.Scan(opts.Date);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
        
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }
}
