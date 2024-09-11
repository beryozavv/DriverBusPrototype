using System.Text.Json;
using DriverBusPrototype.Models;
using DriverBusPrototype.Streams;
using MediatR;

namespace DriverBusPrototype.Handlers;

/// <summary>
/// Базовый обработчик. Сериализует ответ и отправляет его в драйвер в виде CommandResult
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TBody"></typeparam>
public abstract class BaseDriverRequestHandler<T, TBody, TResponse> : IRequestHandler<T> where T : BaseDriverRequest<TBody>
{
    private readonly IInputCommunicationStream _communicationStream;

    public BaseDriverRequestHandler(IInputCommunicationStream communicationStream)
    {
        _communicationStream = communicationStream;
    }

    public async Task<Unit> Handle(T request, CancellationToken cancellationToken)
    {
        CommandResult result;
        try
        {
            var response = await ExecuteRequest(request.RequestBody, cancellationToken); // todo polly?
            var responseJson = JsonSerializer.Serialize(response, BusJsonOptions.GetOptions());
            result = new CommandResult
                { Id = request.Id, CommandType = request.CommandType, IsSuccess = true, Result = responseJson };
        }
        catch (Exception ex)
        {
            result = new CommandResult
            {
                Id = request.Id, CommandType = request.CommandType, IsSuccess = false, ErrorCode = ex.HResult,
                ErrorMessage = ex.Message
            };
        }

        await _communicationStream.WriteAsync(result);

        return Unit.Value;
    }

    public abstract Task<TResponse> ExecuteRequest(TBody request, CancellationToken cancellationToken);
}