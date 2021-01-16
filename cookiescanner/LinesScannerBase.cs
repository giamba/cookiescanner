using System.Collections.Generic;
using System.Linq;

namespace cookiescanner
{
    public  abstract class LinesScannerBase
    {
        public abstract List<string> Search(string searchDate, string file);

        public string GetDateForLine(IEnumerable<string> lines, int lineNum)
        {
            return ParseLine(lines.Skip(lineNum).Take(1).First())[1];
        }

        public string[] ParseLine(string line)
        {
            string[] tokens = line.Split(',');
            return new string[] { tokens[0], tokens[1].Substring(0, tokens[1].IndexOf('T')) };
        }
    }
}
