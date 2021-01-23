using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace cookiescannerparallel
{
    public class CookieScannerParallel
    {
        private string _filePath;
        private FileStream _fileStream;
        private StreamReader _streamReader;

        public long LinesCount { get; private set; } = 0;
        public long HeaderLength { get; private set; }
        public long LineLength { get; private set; }


        public CookieScannerParallel(string filePath)
        {
            _filePath = filePath;
            _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            _streamReader = new StreamReader(_fileStream);
            
            (HeaderLength, LineLength) = GetHeaderAndLineLength();
            if(HeaderLength != 0 || LineLength != 0)
                LinesCount = (_fileStream.Length - HeaderLength) / LineLength;
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

            var cores = Environment.ProcessorCount; // 8

          



            //LinesCount 



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
            finally
            {
                _streamReader.Close();
                _fileStream.Close();
            }
        }


        public List<Tuple<long, long, long>> GetChunk(long chunkNum, long numLines)
        {
            long chunkSize = numLines / chunkNum;
            List<Tuple<long, long, long>> result = new List<Tuple<long, long, long>>();

            for (long i = 0, start = 0, end = chunkSize; i < chunkNum; i++, start += chunkSize + 1, end = start + chunkSize)
            {
                if (end > numLines - 1)
                    end = numLines - 1;

                long size = end - start + 1;
                result.Add(new Tuple<long, long, long>(start, end, size));

            }
            return result;
        }


        /// <summary>
        /// Search for lines that match searchDate. Binary search, complexity O(log N)
        /// </summary>
        /// <param name="searchDate">Date to search. Like: 2018-12-09</param>
        /// <returns>List of lines that match the criteria</returns>
        public List<string> SearchLines(string searchDate)
        {
            List<string> result = new List<string>();

            long index = GetIndex(searchDate);

            if (index == -1)
                return result;

            string currentDate = GetDate(index);
            while (currentDate != null && currentDate == searchDate)
            {
                result.Add(GetLine(index));
                currentDate = GetDate(++index);
            }

            return result;
        }

        public List<string> SearchLinesParallel(string searchDate)
        {
            List<string> result = new List<string>();

            long index = GetIndex(searchDate);

            if (index == -1)
                return result;

            string currentDate = GetDate(index);
            while (currentDate != null && currentDate == searchDate)
            {
                result.Add(GetLine(index));
                currentDate = GetDate(++index);
            }

            return result;
        }


        /// <summary>
        /// Get the index of the first row that match searchDate
        /// </summary>
        /// <param name="lines">Lines as IEnumerable<string></param>
        /// <param name="searchDate">Date to match</param>
        /// <returns>Index of the line that match searchDate</returns>
        private long GetIndex(string searchDate)
        {
            if (LinesCount == 0)
                return -1;

            long min = 0;
            long max = LinesCount;
            long index = -1;

            //binary search 
            while (min <= max)
            {
                long middle = (min + max) / 2;
                string dateMiddle = GetDate(middle);
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
            while (index >= 1 && GetDate(index - 1) == searchDate)
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
        public  string GetDate(long lineNum)
        {
            if (lineNum >= LinesCount || lineNum < 0)
                return null;

            long offset = (HeaderLength) + (lineNum * LineLength) + GetEndLineCharactersCount() + 1;

            _fileStream.Seek(offset, SeekOrigin.Begin);
            _streamReader = new StreamReader(_fileStream);

            var line = _streamReader.ReadLine();

            (string _, string date) = ParseLine(line);
            return date;
        }

        /// <summary>
        /// Returns the line with index lineNum
        /// </summary>
        /// <param name="lines">Lines as IEnumerable<string></param>
        /// <param name="lineNum">Number of the line to retrieve</param>
        /// <returns>Return the parsed date of the line. Like: "2018-12-09"</returns>
        private string GetLine(long lineNum)
        {
            long offset = HeaderLength + (LineLength * lineNum) + GetEndLineCharactersCount() + 1;
            _fileStream.Seek(offset, SeekOrigin.Begin);
            _streamReader = new StreamReader(_fileStream);
            return _streamReader.ReadLine();
        }

        public Tuple<int, int> GetHeaderAndLineLength()
        {
            //Read first 2 lines
            int i = 0;
            List<string> firstTwoLines = new List<string>();

            _fileStream.Seek(0, SeekOrigin.Begin);
            _streamReader = new StreamReader(_fileStream);
            while (i < 2)
            {
                firstTwoLines.Add(_streamReader.ReadLine());
                i++;
            }

            if (firstTwoLines[0] == null || firstTwoLines[1] == null)
            {
                return new Tuple<int, int>(0, 0);
            }

            int headerByteCount = firstTwoLines[0].Length + GetEndLineCharactersCount();
            int lineByteCount = firstTwoLines[1].Length + GetEndLineCharactersCount();

            return new Tuple<int, int>(headerByteCount, lineByteCount);
        }

        /// <summary>
        /// Get the end of line characters 
        /// </summary>
        /// <returns>Retur the number of end of line characters</returns>
        private int GetEndLineCharactersCount()
        {
            // Windows \r\n = > 2
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return 2;

            //Mac \r, Unix \n
            else
                return 1;
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
