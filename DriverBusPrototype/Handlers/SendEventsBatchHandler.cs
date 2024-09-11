using DriverBusPrototype.Models;
using DriverBusPrototype.Streams;
using Microsoft.Extensions.Logging;

namespace DriverBusPrototype.Handlers;

/// <summary>
/// Todo тестовый хендлер для имитации отправки батча событий
/// </summary>
public class SendEventsBatchHandler : BaseDriverRequestHandler<SendEventsBatchRequest, DriverEventDto[], long>
{
    private readonly ILogger<SendEventsBatchHandler> _logger;
    private readonly string _testFilePath = "MaxDriverEventId.txt";

    public SendEventsBatchHandler(IInputCommunicationStream communicationStream, ILogger<SendEventsBatchHandler> logger)
        : base(communicationStream)
    {
        _logger = logger;
    }

    public override async Task<long> ExecuteRequest(DriverEventDto[] events, CancellationToken cancellationToken)
    {
        if (events.Length == 0)
        {
            if (File.Exists(_testFilePath))
            {
                using (var fs = new FileStream(_testFilePath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[sizeof(long)];
                    // ReSharper disable once MustUseReturnValue
                    fs.Read(bytes, 0, bytes.Length);
                    long longValue = BitConverter.ToInt64(bytes, 0);

                    return longValue;
                }
            }
            else
            {
                return 0;
            }
        }

        var maxId = events.Max(e => e.DriverEventId);

        _logger.LogInformation("Max eventId from events batch = {MaxId}. Events count = {Count}", maxId, events.Length);

        _logger.LogInformation("Save events to Db");
        foreach (var driverEventDto in events)
        {
            _logger.LogInformation(
                "DriverId = {Id}, EventType={Type}, EventDate={Date}, DocGuid={Guid}, FileName={File}, MarkerGuid = {Marker}",
                driverEventDto.DriverEventId, driverEventDto.EventType, driverEventDto.EventDateTimeUtc,
                driverEventDto.DocGuid, driverEventDto.FileName, driverEventDto.MarkerGuid);
        }

        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);

        using (var fs = File.Create(_testFilePath))
        {
            byte[] info = BitConverter.GetBytes(maxId);
            fs.Write(info, 0, info.Length);
        }

        return maxId;
    }
}