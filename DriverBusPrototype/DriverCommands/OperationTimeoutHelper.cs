namespace DriverBusPrototype.DriverCommands;

public static class OperationTimeoutHelper
{
    public static T OperationWithTimeout<T>(Func<T> operation, TimeSpan timeout)
    {
        var task = Task.Run(operation);
        if (!task.Wait(timeout))
        {
            throw new CommandTimeoutException("Timeout in operation", timeout);
        }

        return task.Result;
    }

    public static void OperationWithTimeout(Action operation, TimeSpan timeout)
    {
        var task = Task.Run(operation);
        if (!task.Wait(timeout))
        {
            throw new CommandTimeoutException("Timeout in operation", timeout);
        }
    }
}