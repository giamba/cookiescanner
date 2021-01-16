using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace cookiescanner.tests
{
    public class CookieScannerTest
    {
        [Theory]
        [InlineData("test1.csv", "2999-12-09", 0, null)]
        [InlineData("test1.csv", "2018-12-09", 1, "AtY0laUfhglK3lC7")]
        [InlineData("test2.csv", "2018-12-08", 2, "SAZuXPGUrfbcn5UA,fbcn5UAVanZf6UtG")]
        [InlineData("test3.csv", "2018-12-08", 0, null)]
        public void WhenTestingScan_ResultsAreCorrect(string fileName, string searchDate, int expectedCount, string expectedCookies)
        {
            //Arrange
            CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

            //Act
            List<string> result = cookieScanner.Scan(searchDate);

            //Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.Equal(expectedCookies, ResultForTest(result));
        }


        [Theory]
        [InlineData("test1.csv", "2999-12-09", 0)]
        [InlineData("test1.csv", "2018-12-09", 4)]
        [InlineData("test2.csv", "2018-12-10", 1)]
        [InlineData("test1.csv", "2018-12-07", 1)]
        [InlineData("test3.csv", "2018-12-08", 0)]

        public void WhenTestingSearchLines_ResultsAreCorrect(string fileName, string searchDate, int expectedCount)
        {
            //Arrange
            CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

            //Act
            List<string> result = cookieScanner.SearchLines(searchDate);

            //Assert
            Assert.Equal(expectedCount, result.Count);
        }

        /// <summary>
        /// Returns the file's path 
        /// </summary>
        private string GetFilePath(string fileName)
        {
            char dirSeparator = '/';
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dirSeparator = '\\';
            }

            string currPath = Directory.GetCurrentDirectory();
            currPath = currPath.Remove(currPath.LastIndexOf(dirSeparator));
            currPath = currPath.Remove(currPath.LastIndexOf(dirSeparator));
            currPath = currPath.Remove(currPath.LastIndexOf(dirSeparator));

            return Path.Combine(new string[] { currPath, "testfiles", fileName });
        }

        /// <summary>
        /// Method to facilitate assertions
        /// Convert a list of string in a comma separeted string. Null if list is empty.
        /// </summary>
        private string ResultForTest(List<string> result)
        {
            string resultForTest = null;

            if (result.Count > 0)
                return string.Join(",", result);

            return resultForTest;
        }
    }
}
