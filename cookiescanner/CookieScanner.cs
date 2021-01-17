using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cookiescanner
{
    public class CookieScanner
    {
        private string _filePath;

        public CookieScanner(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Reurns the most active cookies
        /// </summary>
        /// <param name="searchDate">Date to search. Like: 2018-12-09</param>
        /// <returns>List of cookies</returns>
        public Tuple<List<string>, ScannerError> Scan(string searchDate)
        {
            Dictionary<string, int> cookieCountDict = new Dictionary<string, int>();
            Dictionary<int, List<string>> countCookieListDict = new Dictionary<int, List<string>>();
            int max = 1;

            try
            {
                var lines = SearchLines(searchDate);

                foreach (string line in lines)
                {
                    (string cookie, string date) = ParseLine(line);

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

                if (max == 1)
                    return new Tuple<List<string>, ScannerError>(new List<string>(), null);

                return new Tuple<List<string>, ScannerError>(countCookieListDict[max], null);
            }
            catch (ParseLineException ex)
            {
                return new Tuple<List<string>, ScannerError>(null, new ScannerError(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return new Tuple<List<string>, ScannerError>(null, new ScannerError("### Generic error", ex));
            }
        }

        /// <summary>
        /// Search for lines that match searchDate. Binary search, complexity O(log N)
        /// </summary>
        /// <param name="searchDate">Date to search. Like: 2018-12-09</param>
        /// <returns>List of lines that match the criteria</returns>
        public List<string> SearchLines(string searchDate)
        {
            List<string> result = new List<string>();
            IEnumerable<string> lines = File.ReadLines(_filePath);

            int index = GetIndex(lines, searchDate);

            if (index == -1) 
                return result;

            string currentDate = GetDate(lines, index);
            while (currentDate != null && currentDate == searchDate)
            {
                result.Add(GetLine(lines, index));
                currentDate = GetDate(lines, ++index);
            }

            return result;
        }

        /// <summary>
        /// Get the index of the first row that match searchDate
        /// </summary>
        /// <param name="lines">Lines as IEnumerable<string></param>
        /// <param name="searchDate">Date to match</param>
        /// <returns>Index of the line that match searchDate</returns>
        private int GetIndex(IEnumerable<string> lines, string searchDate)
        {
            int min = 1;
            int max = lines.Count() - 1;
            int index = -1;

            //binary search 
            while (min <= max)
            {
                int middle = (min + max) / 2;
                string dateMiddle = GetDate(lines, middle);
                if (dateMiddle == searchDate)
                {
                    index = middle;
                    break;
                }
                else if (DateTime.Parse(dateMiddle) < DateTime.Parse(searchDate))
                    max = middle - 1;
                else
                    min = middle + 1;
            }

            if (index == -1) 
                return index;

            //go back to find the first line
            while (index > 1 && GetDate(lines, index - 1) == searchDate)
                index--;

            return index;
        }

        /// <summary>
        /// Parse a single line
        /// </summary>
        /// <param name="line">Line to parse. Like: AtY0laUfhglK3lC7,2018-12-09T14:19:00+00:00</param>
        /// <returns>Return the cookie and the date of the line. Like: ("AtY0laUfhglK3lC7", "2018-12-09") </returns>
        private Tuple<string, string> ParseLine(string line)
        {
            try
            {
                string[] tokens = line.Split(',');
                return Tuple.Create<string, string>(tokens[0], tokens[1].Substring(0, tokens[1].IndexOf('T')));
            }
            catch (Exception ex)
            {
                string message = $"### Error parsing line: {line}";
                throw new ParseLineException(message, ex);
            }
        }

        /// <summary>
        /// Returns the date with index lineNum
        /// </summary>
        /// <param name="lines">Lines as IEnumerable<string></param>
        /// <param name="lineNum">Number of the line to retrieve</param>
        /// <returns>Return the parsed date of the line. Like: "2018-12-09"</returns>
        private string GetDate(IEnumerable<string> lines, int lineNum)
        {
            if (lines.Skip(lineNum).Take(1).FirstOrDefault() == null)
                return null;

            (string _, string date) = ParseLine(lines.Skip(lineNum).Take(1).First());
            return date;
        }

        /// <summary>
        /// Returns the line with index lineNum
        /// </summary>
        /// <param name="lines">Lines as IEnumerable<string></param>
        /// <param name="lineNum">Number of the line to retrieve</param>
        /// <returns>Return the parsed date of the line. Like: "2018-12-09"</returns>
        private string GetLine(IEnumerable<string> lines, int lineNum)
        {
            return lines.Skip(lineNum).Take(1).First();
        }
    }

    public class ScannerError
    {
        public ScannerError(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; }
        public Exception Exception { get; }
    }
    public class ParseLineException : Exception
    {
        public ParseLineException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}