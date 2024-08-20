namespace DriverBusPrototype.DriverCommands.Models;

/// <summary>
/// 
/// </summary>
public class ParamsJson
{
    /// <summary>
    /// 
    /// </summary>
    public bool IsDriverEnabled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int EventsBatchSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public TrustedApp[] TrustedApps { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public FileFormat[] FileFormats { get; set; }
}

public class TrustedApp
{
    public string AppName { get; set; }
    public string Hash { get; set; }
}