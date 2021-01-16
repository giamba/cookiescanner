using System.Collections.Generic;
using System.IO;
using Xunit;

namespace cookiescanner.tests
{
    public class FileScannerTest
    {
    
        [Theory]
        [InlineData("test1.csv", "2999-12-09", 0, null)]
        [InlineData("test1.csv", "2018-12-09", 1, "AtY0laUfhglK3lC7")]
        [InlineData("test2.csv", "2018-12-08", 2, "SAZuXPGUrfbcn5UA,fbcn5UAVanZf6UtG")]
        [InlineData("test3.csv", "2018-12-08", 0, null)]
        public void TestingFileScanner_ResultsAreCorrect(string fileName, string searchDate, int expectedCount, string expectedCookies)
        {
            //Arrange
            FileScanner fp = new FileScanner(GetFilePath(fileName));

            //Act
            List<string> result = fp.Parse(searchDate);

            //Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.Equal(expectedCookies, ResultForTest(result));
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
