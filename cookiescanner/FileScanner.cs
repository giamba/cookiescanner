using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cookiescanner
{
    public class FileScanner
    {
            private string _filePath;
        
        public FileScanner(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Reurns the most active cookie
        /// </summary>
        /// <param name="searchDate">Date to search. Like: 2018-12-09</param>
        /// <returns>List of cookies</returns>
        public List<string> Scan(string searchDate)
        {
            List<string> result = new List<string>();
                      
            int max = 1;
            Dictionary<string, int> cookieCountDict = new Dictionary<string, int>();
            Dictionary<int, List<string>> countCookieListDict = new Dictionary<int, List<string>>();

            var lines = SearchLines(searchDate);

            foreach (string line in lines)
            {
                string[] tokens = ParseLine(line);
                string cookie = tokens[0];
            
                if (!cookieCountDict.ContainsKey(cookie))
                        cookieCountDict[cookie] = 1;
                else
                {
                    cookieCountDict[cookie] += 1;
           
                    if (!countCookieListDict.ContainsKey(cookieCountDict[cookie]))
                        countCookieListDict[cookieCountDict[cookie]] = new List<string> { cookie };
                    else
                        countCookieListDict[cookieCountDict[cookie]].Add(cookie);

                    if (cookieCountDict[cookie] > max)
                        max = cookieCountDict[cookie];
                }
            }

            if (max == 1) return result;

            return countCookieListDict[max];
        }

        /// <summary>
        /// Seach for lines that match searchDate. Binary search, complexity O(long N)
        /// </summary>
        /// <param name="searchDate">Date to search. Like: 2018-12-09</param>
        /// <returns>List of lines that match the criteria</returns>
        public List<string> SearchLines(string searchDate)
        {
            List<string> result = new List<string>();
            IEnumerable<string> lines = File.ReadLines(_filePath);

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

            //go back to find the first line
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

        /// <summary>
        /// Parse a single line
        /// </summary>
        /// <param name="line">Line to parse. Like: AtY0laUfhglK3lC7,2018-12-09T14:19:00+00:00</param>
        /// <returns>Return the cookie and the date of the line. Like: ["AtY0laUfhglK3lC7", "2018-12-09"] </returns>
        private string[] ParseLine(string line)
        {
            string[] tokens = line.Split(',');
            return new string[] { tokens[0], tokens[1].Substring(0, tokens[1].IndexOf('T')) };
        }

        /// <summary>
        /// Returns a strid date associated at the line lineNum
        /// </summary>
        /// <param name="lines">List of lines.</param>
        /// <param name="lineNum">Number of the line to retrieve</param>
        /// <returns>Return the parsed date of the line. Like: "2018-12-09"</returns>
        private string GetDateForLine(IEnumerable<string> lines, int lineNum)
        {
            return ParseLine(lines.Skip(lineNum).Take(1).First())[1];
        }
    }
}
