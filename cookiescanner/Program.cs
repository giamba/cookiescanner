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
            //var lines = File.ReadLines("C:\\Dev\\cookiescanner\\cookiescanner\\testfiles\\test1.csv");


            FileScanner fp = new FileScanner("C:\\Dev\\cookiescanner\\cookiescanner\\testfiles\\test2.csv");


            var lines = fp.Parse(opts.Date);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }


            Console.ReadKey();
            

            //handle options
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }

    }
}
