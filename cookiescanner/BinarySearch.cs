
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cookiescanner
{
    public class BinarySearch : LinesScannerBase
    {

        /// <summary>
        /// Retrieve the lines that match serchDate using a binary search algorithm
        /// </summary>
        public override List<string> Search(string searchDate, string file)
        {
            List<string> result = new List<string>();
            IEnumerable<string> lines = File.ReadLines(file);

            int min = 1;
            int max = lines.Count() - 1;
            int seek = -1;

            //binary search 
            while (min <= max)
            {
                int middle = (min + max) / 2;
                string dateMiddle = GetDateForLine(lines, middle);
                if (dateMiddle == searchDate)
                {
                    seek = middle;
                    break;
                }
                else if (DateTime.Parse(dateMiddle) < DateTime.Parse(searchDate))
                    max = middle - 1;
                else
                    min = middle + 1;
            }

            if (seek == -1) return result;
            
            //go back to find the first 
            while (seek > 1 && GetDateForLine(lines, seek - 1) == searchDate)
                seek--;
            

            foreach (var line in lines.Skip(seek))
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
