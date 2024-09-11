using DriverBusPrototype.Handlers;
using DriverBusPrototype.Models;
using DriverBusPrototype.Streams;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DriverBusPrototype.Services;

internal class RequestExecutor : IRequestExecutor
{
    private readonly IInputCommunicationStream _inputStream;
    private readonly IMediator _mediator;
    private readonly ILogger<RequestExecutor> _logger;

    public RequestExecutor(IInputCommunicationStream inputStream, IMediator mediator, ILogger<RequestExecutor> logger)
    {
        _inputStream = inputStream;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task ExecuteRequests(CancellationToken stoppingToken)
    {
        await _inputStream.ConnectAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ReadAndExecuteRequest();
        }

        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task ReadAndExecuteRequest()
    {
        try
        {
            var command = await _inputStream.ReadAsync<Command>();

            var request = MapCommandToRequest(command);

            await _mediator.Send(request);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "readCommandError");
        }
    }

    private BaseDriverRequest MapCommandToRequest(Command command)
    {
        switch (command.Type)
        {
            case CommandType.GetEncryptionKey:
                return new GetEncryptionKeyRequest
                {
                    Id = command.Id,
                    CommandType = command.Type,
                    RequestBody = GetEncryptionKeyRequest.DeserializeRequestBody(command.Parameters)
                };
            case CommandType.SendEventsBatch:
                return new SendEventsBatchRequest
                {
                    Id = command.Id,
                    CommandType = command.Type,
                    RequestBody = SendEventsBatchRequest.DeserializeRequestBody(command.Parameters)!
                };
            case CommandType.Default:
            case CommandType.SetParams:
            case CommandType.SetPermissions:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}