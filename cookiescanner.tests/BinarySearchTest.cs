using System.Collections.Generic;
using System.IO;
using Xunit;

namespace cookiescanner.tests
{
    public class BinarySearchTest
    {
    
        [Theory]
        [InlineData("test1.csv", "2999-12-09", 0)]
        [InlineData("test1.csv", "2018-12-09", 4)]
        [InlineData("test2.csv", "2018-12-10", 1)]
        [InlineData("test1.csv", "2018-12-07", 1)]
        [InlineData("test3.csv", "2018-12-08", 0)]
        
        public void TestingBinarySearch_ResultsAreCorrect(string fileName, string searchDate, int expectedCount)
        {
            //Arrange
            BinarySearch bs = new BinarySearch();

            //Act
            List<string> result = bs.Search(searchDate, GetFilePath(fileName));

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
    }
}
