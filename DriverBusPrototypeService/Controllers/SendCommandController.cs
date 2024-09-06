using DriverBusPrototype;
using DriverBusPrototype.Models;
using DriverBusPrototype.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriverBusPrototypeService.Controllers;

[AllowAnonymous]
[ApiController]
[Route("command")]
public class SendCommandController : ControllerBase
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ILogger<SendCommandController> _logger;

    public SendCommandController(ICommandExecutor commandExecutor, ILogger<SendCommandController> logger)
    {
        _commandExecutor = commandExecutor;
        _logger = logger;
    }

    [HttpPost]
    [Route("params")]
    public async Task<ActionResult<CommandResult>> ParamsCommand(CancellationToken cancellationToken)
    {
        var paramsCommand = TestCommandsHelper.PrepareParamsCommand();

        var result = await _commandExecutor.ExecuteCommandAsync(paramsCommand, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("permissions")]
    public async Task<ActionResult<CommandResult>> PermissionsCommand(CancellationToken cancellationToken)
    {
        var permissionsCommand = TestCommandsHelper.PreparePermissionsCommand();

        var result = await _commandExecutor.ExecuteCommandAsync(permissionsCommand, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("both")]
    public async Task<ActionResult<CommandResult>> BothCommands(CancellationToken cancellationToken)
    {
        var permissionsCommand = TestCommandsHelper.PreparePermissionsCommand();
        var paramsCommand = TestCommandsHelper.PrepareParamsCommand();

        var permissionsTask = _commandExecutor.ExecuteCommandAsync(permissionsCommand, cancellationToken);
        var paramsTask = _commandExecutor.ExecuteCommandAsync(paramsCommand, cancellationToken);

        var results = await Task.WhenAll(paramsTask, permissionsTask);
        foreach (var commandResult in results)
        {
            _logger.LogInformation("command result: {Id}, {IsSuccess}, {ErrorCode}, {ErrorMsg} ", commandResult.Id,
                commandResult.IsSuccess, commandResult.ErrorCode, commandResult.ErrorMessage);
        }

        return Ok(results[0]);
    }

    [HttpPost]
    [Route("any")]
    public async Task<ActionResult<CommandResult>> AnyCommand(Command command, CancellationToken cancellationToken)
    {
        var result = await _commandExecutor.ExecuteCommandAsync(command, cancellationToken);

        return Ok(result);
    }
}