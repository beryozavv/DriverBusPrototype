using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Streams;

internal class InputCommunicationStream : BaseCommunicationStream, IInputCommunicationStream
{
    public InputCommunicationStream(ILogger<InputCommunicationStream> logger, IOptions<DriverBusSettings> settings) 
        : base(logger, settings.Value.InputPipeName)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
    }
}