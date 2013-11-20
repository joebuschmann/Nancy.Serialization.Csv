using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nancy.Serialization.Csv.Serialization
{
    public class CsvSerializer : ISerializer
    {
        public bool CanSerialize(string contentType)
        {
            return contentType == "application/csv";
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            IEnumerable subject = model as IEnumerable;

            if (subject != null)
            {
                bool headerWritten = false;

                foreach (var item in subject)
                {
                    var properties =
                        item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead);

                    if (!headerWritten)
                    {
                        string header = properties.Aggregate("", (s, p) => string.Format("{0},{1}", s, p.Name)).Trim(',');
                        byte[] headerBuffer = Encoding.UTF8.GetBytes(header);
                        outputStream.Write(headerBuffer, 0, headerBuffer.Count());
                        headerWritten = true;
                    }

                    string row =
                        properties.Aggregate("", (s, p) => string.Format("{0},{1}", s, p.GetValue(item).ToString()))
                                  .Trim(',');

                    byte[] buffer = Encoding.UTF8.GetBytes("\n" + row);
                    outputStream.Write(buffer, 0, buffer.Count());
                }
            }
        }

        public IEnumerable<string> Extensions { get { yield break; } }
    }
}
