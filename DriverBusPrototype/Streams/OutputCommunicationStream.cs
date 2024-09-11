using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Streams;

internal class OutputCommunicationStream : BaseCommunicationStream, IOutputCommunicationStream
{
    public OutputCommunicationStream(ILogger<OutputCommunicationStream> logger, IOptions<DriverBusSettings> settings) 
        : base(logger, settings.Value.OutputPipeName)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
    }
}