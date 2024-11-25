using Crosstech.Dss;
using Grpc.Core;

namespace DriverBusPrototype.Streams;

internal class ThreadSafetyOutGrpcStream : IDisposable
{
    private readonly IGrpcStreamProvider _grpcStreamProvider;

    private int _isDisposed;
    private readonly SemaphoreSlim _readSemaphore = new(1, 1);
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    public bool IsConnected => _grpcStreamProvider.SendCommandStreams.ServerCallContext.Value?.Status.StatusCode ==
                               StatusCode.OK;

    public ThreadSafetyOutGrpcStream(IGrpcStreamProvider grpcStreamProvider)
    {
        _grpcStreamProvider = grpcStreamProvider;
    }

    public async Task<string> ReadLineAsync()
    {
        CheckDisposed();
        await _readSemaphore.WaitAsync();
        try
        {
            CheckDisposed();


            //  throw new CommunicationStreamException("read null");

            await _grpcStreamProvider.SendCommandStreams.CommandResultStream.Value.MoveNext();


            return _grpcStreamProvider.SendCommandStreams.CommandResultStream.Value?.Current.Result;
        }
        finally
        {
            _readSemaphore.Release();
        }
    }

    public async Task WriteLineAsync(string? value)
    {
        CheckDisposed();
        await _writeSemaphore.WaitAsync();
        try
        {
            CheckDisposed();

            await _grpcStreamProvider.SendCommandStreams.CommandStream.Value.WriteAsync(new Command
                { Command_ = value });
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    private void CheckDisposed()
    {
        if (_isDisposed == 1)
            throw new ObjectDisposedException(nameof(ThreadSafetyOutGrpcStream));
    }

    public async Task DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
            return;

        await _readSemaphore.WaitAsync();
        await _writeSemaphore.WaitAsync();
        try
        {
            _grpcStreamProvider.SendCommandStreams.FinishTaskSource.Value?.SetCanceled();
        }
        finally
        {
            _readSemaphore.Release();
            _writeSemaphore.Release();
        }
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }
}