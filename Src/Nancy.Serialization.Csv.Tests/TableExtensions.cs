using System.IO;
using System.Linq;
using System.Text;

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
    }
}
