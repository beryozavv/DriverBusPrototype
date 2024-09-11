using System.IO.Pipes;

namespace DriverBusPrototype.Streams;

internal class ThreadSafetyNamedPipeStream : IDisposable
{
    private readonly NamedPipeServerStream _pipeServerStream;

    private int _isDisposed;
    private readonly SemaphoreSlim _readSemaphore = new(1, 1);
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    public bool IsConnected => _pipeServerStream.IsConnected;

    public ThreadSafetyNamedPipeStream(string pipeName)
    {
        _pipeServerStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1,
            PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
    }

    public Task WaitForConnectionAsync(CancellationToken cancellationToken = default)
    {
        return _pipeServerStream.WaitForConnectionAsync(cancellationToken);
    }

    public async Task<string> ReadLineAsync()
    {
        CheckDisposed();
        await _readSemaphore.WaitAsync();
        try
        {
            CheckDisposed();

            using (var reader = new StreamReader(_pipeServerStream, leaveOpen: true))
            {
                var readLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(readLine))
                {
                    readLine = await reader.ReadLineAsync(); //после второго чтения стает notConnected
                    if (string.IsNullOrEmpty(readLine))
                    {
                        throw new CommunicationStreamException("read null");
                    }
                }

                return readLine;
            }
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

            using (var writer = new StreamWriter(_pipeServerStream, leaveOpen: true))
            {
                writer.AutoFlush = true;
                await writer.WriteLineAsync(value);
            }
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    private void CheckDisposed()
    {
        if (_isDisposed == 1)
            throw new ObjectDisposedException(nameof(ThreadSafetyNamedPipeStream));
    }

    public async Task DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
            return;

        await _readSemaphore.WaitAsync();
        await _writeSemaphore.WaitAsync();
        try
        {
            await _pipeServerStream.DisposeAsync();
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