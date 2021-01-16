using System.Collections.Generic;
using System.IO;
using Xunit;

namespace cookiescanner.tests
{
    public class FileScannerTest
    {
        [Theory]
        [InlineData("test1.csv", "2999-12-09", 1, null)]
        [InlineData("test1.csv", "2018-12-09", 1, "AtY0laUfhglK3lC7")]
        [InlineData("test2.csv", "2018-12-08", 2, "SAZuXPGUrfbcn5UA,fbcn5UAVanZf6UtG")]
        [InlineData("test3.csv", "2018-12-08", 0, null)]
        public void WhenTestingScan_ResultsAreCorrect(string fileName, string searchDate, int expectedCount, string expectedCookies)
        {
            //Arrange
            FileScanner fileScanner = new FileScanner(GetFilePath(fileName));

            //Act
            List<string> result = fileScanner.Scan(searchDate);

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
            FileScanner fileScanner = new FileScanner(GetFilePath(fileName));

            //Act
            List<string> result = fileScanner.SearchLines(searchDate);

            //Assert
            Assert.Equal(expectedCount, result.Count);
        }

        /// <summary>
        /// Returns the file's path 
        /// </summary>
        private string GetFilePath(string fileName)
        {
            string currPath = Directory.GetCurrentDirectory();
            currPath = currPath.Remove(currPath.LastIndexOf('\\'));
            currPath = currPath.Remove(currPath.LastIndexOf('\\'));
            currPath = currPath.Remove(currPath.LastIndexOf('\\'));

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
