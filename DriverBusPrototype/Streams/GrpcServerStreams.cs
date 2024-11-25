using Crosstech.Dss;
using Grpc.Core;

namespace DriverBusPrototype.Streams;

public class GrpcServerStreams : FilterDriver.FilterDriverBase
{
    private readonly IGrpcStreamProvider _grpcStreamProvider;
    private TaskCompletionSource? _finishCommandsTaskSource;

    public GrpcServerStreams(IGrpcStreamProvider grpcStreamProvider)
    {
        _grpcStreamProvider = grpcStreamProvider;
    }

    public override async Task SendCommand(IAsyncStreamReader<CommandResult> commandResultStream,
        IServerStreamWriter<Command> commandStream, ServerCallContext context)
    {
        _finishCommandsTaskSource = new TaskCompletionSource();
        
        _grpcStreamProvider.SendCommandStreams.CommandStream.Value = commandStream;
        _grpcStreamProvider.SendCommandStreams.CommandResultStream.Value = commandResultStream;
        _grpcStreamProvider.SendCommandStreams.ServerCallContext.Value = context;
        _grpcStreamProvider.SendCommandStreams.FinishTaskSource.Value = _finishCommandsTaskSource;

        await _finishCommandsTaskSource.Task;
    }

    public override Task GetRequest(IAsyncStreamReader<BusRequest> requestStream,
        IServerStreamWriter<BusResponse> responseStream, ServerCallContext context)
    {
        return base.GetRequest(requestStream, responseStream, context);
    }
}

public class GrpcStreamProvider : IGrpcStreamProvider
{
    public SendCommandStreams SendCommandStreams { get; }
}

public interface IGrpcStreamProvider
{
    SendCommandStreams SendCommandStreams { get; }
}

public class SendCommandStreams
{
    public AtomicReference<IAsyncStreamReader<CommandResult>> CommandResultStream { get; } = new();
    public AtomicReference<IServerStreamWriter<Command>> CommandStream { get; } = new();
    public AtomicReference<ServerCallContext> ServerCallContext { get; } = new();
    public AtomicReference<TaskCompletionSource> FinishTaskSource { get; } = new();
}

public class AtomicReference<T> where T : class
{
    private T? _value;

    public AtomicReference(T initialValue)
    {
        _value = initialValue;
    }

    public AtomicReference()
    {
    }

    public T? Value
    {
        get => _value;
        set => Interlocked.Exchange(ref _value, value);
    }

    public bool CompareAndSet(T expectedValue, T newValue)
    {
        return Interlocked.CompareExchange(ref _value, newValue, expectedValue) == expectedValue;
    }
}