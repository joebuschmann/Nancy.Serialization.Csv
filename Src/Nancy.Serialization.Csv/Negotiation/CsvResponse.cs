using System;
using System.IO;

namespace Nancy.Serialization.Csv.Negotiation
{
    public class CsvResponse<TModel> : Response
    {
        public CsvResponse(TModel model, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("CSV Serializer not set");
            }

            this.Contents = GetContents(model, serializer);
            this.ContentType = "application/csv";
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize("application/csv", model, stream);
        }
    }

    public class CsvResponse : CsvResponse<object>
    {
        public CsvResponse(object model, ISerializer serializer)
            : base(model, serializer)
        {
        }
    }
}
