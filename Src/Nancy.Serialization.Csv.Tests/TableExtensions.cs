using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nancy;

// ReSharper disable CheckNamespace
namespace TechTalk.SpecFlow
// ReSharper restore CheckNamespace
{
    internal static class TableExtensions
    {
        internal static Stream ConvertToCsv(this Table table)
        {
            var stream = new MemoryStream();

            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 255, true))
            {
                var headerRow = table.Header.Aggregate((s1, s2) => string.Concat(s1, ",", s2));
                streamWriter.WriteLine(headerRow);

                foreach (var row in table.Rows)
                {
                    var dataRow = row.Values.Aggregate((s1, s2) => string.Concat(s1, ",", s2));
                    streamWriter.WriteLine(dataRow);
                }

                streamWriter.Flush();
            }

            stream.Position = 0;

            return stream;
        }

        internal static void CompareToDynamicSet(this Table table, IEnumerable<DynamicDictionary> items)
        {
            var header = table.Header;
            var list = new List<DynamicDictionary>(items);

            if (table.Rows.Count != list.Count)
                Assert.Fail("Row count does not equal the item count in the list.");

            for (int i = 0; i < table.Rows.Count; i++)
            {
                TableRow row = table.Rows[i];
                DynamicDictionary dynamicDictionary = list[i];

                foreach (var headerRow in header)
                {
                    var expectedValue = row[headerRow];
                    var actualValue = dynamicDictionary[headerRow.Replace(" ", "")].ToString();

                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
            }
        }
    }
}
