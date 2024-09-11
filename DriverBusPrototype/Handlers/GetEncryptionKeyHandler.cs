using System.Security.Cryptography;
using System.Text;
using DriverBusPrototype.Streams;
using Microsoft.Extensions.Logging;

namespace DriverBusPrototype.Handlers;

/// <summary>
/// Todo тестовый хендлер для имитации получения ключа шифрования
/// </summary>
public class GetEncryptionKeyHandler : BaseDriverRequestHandler<GetEncryptionKeyRequest, Guid, string>
{
    private readonly ILogger<GetEncryptionKeyHandler> _logger;

    public GetEncryptionKeyHandler(IInputCommunicationStream communicationStream,
        ILogger<GetEncryptionKeyHandler> logger) : base(communicationStream)
    {
        _logger = logger;
    }

    public override async Task<string> ExecuteRequest(Guid documentGuid,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Encryption Key from server by documentGuid={Guid}", documentGuid);
        
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);

        var guidBytes = documentGuid.ToByteArray();

        // Compute the hash using SHA-256
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(guidBytes);

            // Convert the hash to a hexadecimal string
            var hashString = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hashString.Append(b.ToString("x2"));
            }

            return hashString.ToString();
        }
    }
}