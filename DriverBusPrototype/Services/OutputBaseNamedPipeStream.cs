using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Services;

internal class OutputBaseNamedPipeStream : BaseNamedPipeStream, IOutputCommunicationStream
{
    public OutputBaseNamedPipeStream(ILogger<OutputBaseNamedPipeStream> logger, IOptions<DriverBusSettings> settings) 
        : base(logger, settings.Value.OutputPipeName)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
    }
}