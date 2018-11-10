using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EchoGrabber.Tests
{
    [TestClass]
    public class GrabberLibTests
    {
        #region Dynamic Data Methods
        private static IEnumerable<object[]> GetDatesFromFile()
        {
            using (var stream = new StreamReader(@"TestData\dates.txt"))
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    yield return new object[] {
                        line
                    };
                }
        }

        private static IEnumerable<object[]> GetSizesFromFile()
        {
            using (var stream = new StreamReader(@"TestData\sizes.txt"))
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    yield return new object[] {
                        line
                    };
                }
        } 

        private static IEnumerable<object[]> GetDurationsFromFile()
        {
            using (var stream = new StreamReader(@"TestData\durations.txt"))
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    yield return new object[] {
                        line
                    };
                }
        }
        #endregion

        [DataTestMethod]
        [DynamicData(nameof(GetDatesFromFile), DynamicDataSourceType.Method)]
        public void DateParserTest(string text)
        {
            var date = text.ParseDateTime();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSizesFromFile), DynamicDataSourceType.Method)]
        public void SizeParserTest(string sizeText)
        {
            var size = sizeText.ParseSize();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetDurationsFromFile), DynamicDataSourceType.Method)]
        public void DurationParserTest(string durationText)
        {
            var duration = durationText.ParseDuration();
        }

    }
}
