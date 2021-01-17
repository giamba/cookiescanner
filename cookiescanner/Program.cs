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
            [Option('f', "file", Required = true, HelpText = "Input Cookie file.")]
            public string File { get; set; }

            [Option('d', "date", Required = true, HelpText = "Date to search.")]
            public string Date { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
           .WithParsed(Run);
        }

        static void Run(Options opts)
        {   
            CookieScanner cookieScanner = new CookieScanner(opts.File);

            (List<string> lines, ScannerError error) = cookieScanner.Scan(opts.Date);
            if(error != null)
                Console.WriteLine($"{error.Message}. \n \t Exception: {error.Exception}");

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
