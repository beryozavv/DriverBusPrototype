using System.Security.Cryptography;
using System.Text;
using DriverBusPrototype.Agent;
using Microsoft.AspNetCore.Mvc;

namespace DriverBusPrototypeService.Controllers;

[ApiController]
[Route("api")]
public class ClientApiController : ControllerBase
{
    private readonly ILogger<ClientApiController> _logger;
    private readonly string _testFilePath = "MaxDriverEventId.txt";

    public ClientApiController(ILogger<ClientApiController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("SaveEvents")]
    public async Task<ActionResult<long>> SaveEvents(DriverEventDto[] events)
    {
        if (events.Length == 0)
        {
            
            if (System.IO.File.Exists(_testFilePath))
            {
                using (var fs = new FileStream(_testFilePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[sizeof(long)];
                    fs.Read(bytes, 0, bytes.Length);
                    long longValue = BitConverter.ToInt64(bytes, 0);

                    return Ok(longValue);
                }
            }
            else
            {
                return 0;
            }
        }

        var maxId = events.Max(e => e.DriverId);

        _logger.LogInformation("Max eventId from events batch = {maxId}. Events count = {count}", maxId, events.Length);

        _logger.LogInformation("Save events to Db");
        foreach (var driverEventDto in events)
        {
            _logger.LogInformation(
                "DriverId = {0}, EventType={1}, EventDate={2}, DocGuid={3}, FileName={4}, MarkerGuid = {5}",
                driverEventDto.DriverId, driverEventDto.EventType, driverEventDto.EventDateTimeUtc,
                driverEventDto.DocGuid, driverEventDto.FileName, driverEventDto.MarkerGuid);
        }

        await Task.Delay(TimeSpan.FromMilliseconds(200));
        
        using (var fs = System.IO.File.Create(_testFilePath))
        {
            byte[] info = BitConverter.GetBytes(maxId);
            fs.Write(info, 0, info.Length);
        }

        return Ok(maxId);
    }

    [HttpGet]
    [Route("Encryption/{documentGuid}/encryptionKey")]
    public async Task<ActionResult<string>> GetEncryptionKey(Guid documentGuid)
    {
        _logger.LogInformation("Get Encryption Key from server by documentGuid={0}",documentGuid);
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        
        byte[] guidBytes = documentGuid.ToByteArray();

        // Compute the hash using SHA-256
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(guidBytes);

            // Convert the hash to a hexadecimal string
            StringBuilder hashString = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashString.Append(b.ToString("x2"));
            }

            return Ok(hashString.ToString());
        }
    }
}