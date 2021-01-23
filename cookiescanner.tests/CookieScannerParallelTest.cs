using cookiescannerparallel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace cookiescannerparallel.tests
{
    public class CookieScannerParallelTest
    {
        //[Theory]
        //[InlineData("test1.csv", 18, 44, 8)]
        //[InlineData("test2.csv", 18, 44, 11)]
        //public void WhenTesting_GetHeaderAndLineLength_and_LinesCount_ResultIsCorrect(string fileName, long expectedHeaderLength, long expectedLineLength, long expectedLinesCount)
        //{
        //    //Arrange
        //    CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

        //    //Assert
        //    Assert.Equal(expectedHeaderLength, cookieScanner.HeaderLength);
        //    Assert.Equal(expectedLineLength, cookieScanner.LineLength);
        //    Assert.Equal(expectedLinesCount, cookieScanner.LinesCount);
        //}


        //[Theory]
        //[InlineData("test1.csv", 0, "2018-12-09")]
        //[InlineData("test1.csv", 1, "2018-12-09")]
        //[InlineData("test1.csv", 6, "2018-12-08")]
        //[InlineData("test1.csv", 7, "2018-12-07")]
        //public void WhenTesting_GetDate_ResultIsCorrect(string fileName, long index, string expectedDate)
        //{
        //    //Arrange
        //    CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

        //    string date = cookieScanner.GetDate(index);
        //    //Assert
        //    Assert.Equal(expectedDate, date);
        //}


        //[Theory]
        //[InlineData("test1.csv", "2999-12-09", 0)]
        //[InlineData("test1.csv", "2018-12-07", 1)]
        //[InlineData("test1.csv", "2018-12-09", 4)]
        //[InlineData("test2.csv", "2018-12-10", 1)]
        //[InlineData("test3.csv", "2018-12-08", 0)]
        //public void WhenTesting_SearchLines_ResultsAreCorrect(string fileName, string searchDate, int expectedCount)
        //{
        //    //Arrange
        //    CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

        //    //Act
        //    List<string> result = cookieScanner.SearchLines(searchDate);

        //    //Assert
        //    Assert.Equal(expectedCount, result.Count);
        //}

        //[Theory]
        //[InlineData("test1.csv", "2999-12-09", 0)]
        //[InlineData("test1.csv", "2018-12-07", 1)]
        //[InlineData("test1.csv", "2018-12-09", 4)]
        //[InlineData("test2.csv", "2018-12-10", 1)]
        //[InlineData("test3.csv", "2018-12-08", 0)]
        //public void WhenTesting_SearchLinesParalles_ResultsAreCorrect(string fileName, string searchDate, int expectedCount)
        //{
        //    //Arrange
        //    CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

        //    //Act
        //    List<string> result = cookieScanner.SearchLines(searchDate);

        //    //Assert
        //    Assert.Equal(expectedCount, result.Count);
        //}




        [Theory]
        //[InlineData("test1.csv", "2999-12-09", 0, null)]
        //[InlineData("test3.csv", "2018-12-08", 0, null)]
        [InlineData("test1.csv", "2018-12-09", 1, "AtY0laUfhglK3lC7")]
        //[InlineData("test2.csv", "2018-12-08", 2, "SAZuXPGUrfbcn5UA,fbcn5UAVanZf6UtG")]
        public void WhenTesting_Scan_ResultsAreCorrect(string fileName, string searchDate, int expectedCount, string expectedCookies)
        {
            //Arrange
            CookieScannerParallel cookieScanner = new CookieScannerParallel(GetFilePath(fileName));

            //Act
            (List<string> result, ScannerError error) = cookieScanner.Scan(searchDate);

            //Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.Equal(expectedCookies, ResultForTest(result));
        }

        [Theory]
        [InlineData("test1.csv", 3, 8, 3, "(0, 2, 3),(3, 5, 3),(6, 7, 2)")]
        [InlineData("test1.csv", 2, 8, 2, "(0, 4, 5),(5, 7, 3)")]
        [InlineData("test1.csv", 2, 9, 2, "(0, 4, 5),(5, 8, 4)")]
        [InlineData("test1.csv", 2, 10, 2, "(0, 5, 6),(6, 9, 4)")]
        [InlineData("test1.csv", 4, 13, 4, "(0, 3, 4),(4, 7, 4),(8, 11, 4),(12, 12, 1)")]
        public void WhenTesting_GetChunk_ResultsAreCorrect(string fileName, long chunkNumbers, long numOfLines, long expectedChunk, string expectedResult)
        {
            //Arrange
            CookieScannerParallel cookieScanner = new CookieScannerParallel(GetFilePath(fileName));

            //Act
            List<Tuple<long, long, long>> result = cookieScanner.GetChunk(chunkNumbers, numOfLines);

            //Assert
            Assert.Equal(expectedChunk, result.Count);
            Assert.Equal(expectedResult, ResultForTest(result));
        }



        //[Theory]
        //[InlineData("test4.csv", "2018-12-09", "### Error parsing line: AtY0laUfhglK3lC7,2018-12-09XXXXXXXXXXXX14:19:00+00:00")]
        //public void WhenTesting_ScannerError_ErrorsAreCorrect(string fileName, string searchDate, string expectedMessage)
        //{
        //    //Arrange
        //    CookieScanner cookieScanner = new CookieScanner(GetFilePath(fileName));

        //    //Act
        //    (List<string> result, ScannerError error) = cookieScanner.Scan(searchDate);

        //    //Assert
        //    Assert.Null(result);
        //    Assert.Equal(expectedMessage, error.Message);
        //}

        /// <summary>
        /// Returns the file's path 
        /// </summary>
        private string GetFilePath(string fileName)
        {
            char dirSeparator = '/';
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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

        private string ResultForTest(List<Tuple<long, long, long>> result)
        {
            string resultForTest = null;

            if (result.Count > 0)
                return string.Join(",", result);

            return resultForTest;
        }
    }
}
