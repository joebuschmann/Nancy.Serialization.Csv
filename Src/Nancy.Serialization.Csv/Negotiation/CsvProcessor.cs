using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.Responses.Negotiation;

namespace Nancy.Serialization.Csv.Negotiation
{
    public class CsvProcessor : IResponseProcessor
    {
        private static readonly Tuple<string, MediaRange>[] _extensionMappings = new[]
            {Tuple.Create("csv", MediaRange.FromString("application/csv"))};

        private readonly ISerializer _serializer;

        public CsvProcessor(IEnumerable<ISerializer> serializers)
        {
            _serializer = serializers.FirstOrDefault(s => s.CanSerialize("application/csv"));
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (requestedMediaRange.Matches("application/csv") || requestedMediaRange.Matches("text/csv"))
            {
                return new ProcessorMatch()
                    {
                        ModelResult = MatchResult.DontCare,
                        RequestedContentTypeResult = MatchResult.ExactMatch
                    };
            }

            return new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.NoMatch
                };
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new CsvResponse(model, _serializer);
        }

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings { get { return _extensionMappings; } }
    }
}
