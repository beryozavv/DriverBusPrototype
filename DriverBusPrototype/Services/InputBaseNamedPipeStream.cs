using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Services;

internal class InputBaseNamedPipeStream : BaseNamedPipeStream, IInputCommunicationStream
{
    public InputBaseNamedPipeStream(ILogger<InputBaseNamedPipeStream> logger, IOptions<DriverBusSettings> settings) 
        : base(logger, settings.Value.InputPipeName)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
    }
}