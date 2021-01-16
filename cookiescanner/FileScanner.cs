using System.Collections.Generic;

namespace cookiescanner
{
    public class FileScanner
    {
        private string _filePath;
        private LinesScannerBase _linesScanner; 

        public FileScanner(string filePath, string searchType = "binary")
        {
            _filePath = filePath;
            if (searchType == "binary")
                _linesScanner = new BinarySearch();
            else
                _linesScanner = new LinearSearch();
        }

        /// <summary>
        /// Parse the file.
        /// Complexity O(long N) for searcType = binary
        /// </summary>
        public List<string> Parse(string searchDate)
        {
            List<string> result = new List<string>();
                      
            int max = 1;
            Dictionary<string, int> cookieCountDict = new Dictionary<string, int>();
            Dictionary<int, List<string>> countCookieListDict = new Dictionary<int, List<string>>();

            var lines = _linesScanner.Search(searchDate, _filePath);

            foreach (string line in lines)
            {
                string[] tokens = _linesScanner.ParseLine(line);
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
    }
}
