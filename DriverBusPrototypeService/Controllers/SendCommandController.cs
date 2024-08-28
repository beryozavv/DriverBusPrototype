using DriverBusPrototype.DriverCommands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriverBusPrototypeService.Controllers;

[AllowAnonymous]
[ApiController]
[Route("command")]
public class SendCommandController : ControllerBase
{
    private readonly ICommandExecutor _commandExecutor;

    public SendCommandController(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    [HttpPost]
    [Route("params")]
    public async Task<IActionResult> ParamsCommand()
    {
        var paramsCommand = TestCommandsHelper.PrepareParamsCommand();

        await _commandExecutor.ExecuteCommandAsync(paramsCommand);

        return Ok();
    }
    
    [HttpPost]
    [Route("permissions")]
    public async Task<IActionResult> PermissionsCommand()
    {
        var permissionsCommand = TestCommandsHelper.PreparePermissionsCommand();

        await _commandExecutor.ExecuteCommandAsync(permissionsCommand);

        return Ok();
    }
    
    [HttpPost]
    [Route("both")]
    public async Task<IActionResult> BothCommands()
    {
        var permissionsCommand = TestCommandsHelper.PreparePermissionsCommand();
        var paramsCommand = TestCommandsHelper.PrepareParamsCommand();

        var permissionsTask = _commandExecutor.ExecuteCommandAsync(permissionsCommand);
        var paramsTask = _commandExecutor.ExecuteCommandAsync(paramsCommand);

        await Task.WhenAll(paramsTask, permissionsTask);

        return Ok();
    }
}