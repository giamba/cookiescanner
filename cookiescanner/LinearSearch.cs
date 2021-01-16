
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cookiescanner
{
    public class LinearSearch : LinesScannerBase
    {
        public override List<string> Search(string searchDate, string file)
        {
            List<string> result = new List<string>();

            var lines = File.ReadLines(file);
            lines = lines.Skip(1); //skip header

            foreach (string line in lines)
            {
                string[] tokens = ParseLine(line);
                if (searchDate == tokens[1])
                    result.Add(line);
                else
                    break;
            }

            return result;
        }
    }
}
