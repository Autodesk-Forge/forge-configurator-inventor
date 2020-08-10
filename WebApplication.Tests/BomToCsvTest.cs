using System;
using Shared;
using WebApplication.Utilities;
using Xunit;

namespace WebApplication.Tests
{
    public class BomToCsvTest
    {
        [Fact]
        public void EmptyBom()
        {
            var bom = new ExtractedBOM();
            Assert.Throws<ApplicationException>(() => bom.ToCSV());
        }

        [Fact]
        public void ColumnsOnly()
        {
            var bom = new ExtractedBOM
            {
                Columns = new [] { new Column { Label = "a" }, new Column { Label = "b", Numeric = true }}
            };

            string[] lines = BomToCsvLines(bom);
            Assert.Single(lines);
            Assert.Equal("a,b", lines[0]);
        }

        [Fact]
        public void DataOnly()
        {
            var bom = new ExtractedBOM
            { 
                Data = new []
                {
                    new object[]{ "foo" }
                }
            };
            Assert.Throws<ApplicationException>(() => bom.ToCSV());
        }

        [Fact]
        public void SingleRow()
        {
            var bom = new ExtractedBOM
            {
                Columns = new [] { new Column { Label = "a" }, new Column { Label = "b" }},
                Data = new []
                {
                    new object[]{ "foo", "bar" }
                }
            };

            string[] lines = BomToCsvLines(bom);
            Assert.Equal(new[] { "a,b", "foo,bar" }, lines);
        }

        [Fact]
        public void SingleColumn()
        {
            var bom = new ExtractedBOM
            {
                Columns = new [] { new Column { Label = "a" }},
                Data = new []
                {
                    new object[]{ "foo" },
                    new object[]{ "bar" }
                }
            };

            string[] lines = BomToCsvLines(bom);
            Assert.Equal(new[] { "a", "foo", "bar" }, lines);
        }

        [Fact]
        public void MultipleRowAndColumns()
        {
            var bom = new ExtractedBOM
            {
                Columns = new [] { new Column { Label = "a" }, new Column { Label = "b" }, new Column { Label = "c", Numeric = true }},
                Data = new []
                {
                    new object[] { "foo", "bar", 1 },
                    new object[] { "foo 2", "bar 2", 2 },
                    new object[] { "foo 3", "bar 3", 3 }
                }
            };

            string[] lines = BomToCsvLines(bom);
            Assert.Equal(new[] { "a,b,c", "foo,bar,1" , "foo 2,bar 2,2" , "foo 3,bar 3,3" }, lines);
        }

        [Fact]
        public void EncodedHeaderAndRows()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DifferentNumberOfColumnAndDataTest()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void NumericData()
        {
            var bom = new ExtractedBOM
            {
                Columns = new [] { new Column { Label = "a", Numeric = true }, new Column { Label = "b", Numeric = true }, new Column { Label = "c", Numeric = true }},
                Data = new []
                {
                    new object[] { 1,2,3 },
                    new object[] { 4,5,6 }
                }
            };

            string[] lines = BomToCsvLines(bom);
            Assert.Equal(new[] { "a,b,c", "1,2,3", "4,5,6" }, lines);
        }

        private static string[] BomToCsvLines(ExtractedBOM bom)
        {
            string[] lines = bom.ToCSV().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }
    }
}
