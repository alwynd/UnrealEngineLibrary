
namespace QueryUELibrary;

/// <summary>
/// Logging.
/// </summary>
public static class Logging
{

    /// <summary>
    /// Info to console.
    /// </summary>
    public static void Info(string msg)
    {
        Log("Info", $"{msg}");
    }        

    /// <summary>
    /// Error to console.
    /// </summary>
    public static void Error(Exception ex, string msg)
    {
        Log("Error", $"{msg} - {ex}");
    }        
        
    /// <summary>
    /// Verbose to console.
    /// </summary>
    public static void Debug(string msg)
    {
        Log("Debug", $"{msg}");
    }

    /// <summary>
    /// Logs to console.
    /// </summary>
    private static void Log(string level, string msg)
    {
        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {level} {msg}");
    }
}