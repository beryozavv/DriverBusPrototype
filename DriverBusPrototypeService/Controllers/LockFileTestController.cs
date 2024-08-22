using Microsoft.AspNetCore.Mvc;

namespace DriverBusPrototypeService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LockFileTestController : ControllerBase
{
    [HttpGet(Name = "LockTheFileShareRead")]
    public async Task<IActionResult> LockShareRead(string path)
    {
        var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

        using (var reader = new StreamReader(fileStream, leaveOpen: true))
        {
            var result = await reader.ReadToEndAsync();
            Console.WriteLine(result);
        }

        using (var writer = new StreamWriter(fileStream, leaveOpen: true))
        {
            writer.WriteLine(true);
        }

        fileStream.Close();
        await fileStream.DisposeAsync();

        return Ok();
    }
}