using System.Diagnostics;

namespace QueryUELibrary;

/// <summary>
/// Queries JSON files using JQ.
/// </summary>
public class QueryJQ
{

    /// <summary>
    /// Guard.
    /// </summary>
    private bool isRunning = false;
    
    /// <summary>
    /// Specify the path to the Cygwin executable
    /// </summary>
    public static string cygwinExecutablePath { get; set; } = "C:/path/to/cygwin/bin/bash.exe";

    /// <summary>
    /// Runs JQ via Cygwin.
    /// </summary>
    public async Task<string> RunCygwinCommand(string command)
    {
        if (isRunning) return string.Empty;
        isRunning = true;

        try
        {
            string result = string.Empty;
            string error = string.Empty;

            Logging.Info($"{GetType().Name}.RunCygwinCommand:-- START, command: {command}");
            // Set up the process start info
            // Execute the process asynchronously
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = cygwinExecutablePath,
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process process = new Process { StartInfo = startInfo };
            // Read the output
            // Read the error output
            // Wait for the process to exit
            process.Start();

            result = await process.StandardOutput.ReadToEndAsync();
            Logging.Info($"{GetType().Name}.RunCygwinCommand: result length: {result.Length}");
            Logging.Debug($"{GetType().Name}.RunCygwinCommand: result: {result}");

            error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();
            if (!string.IsNullOrEmpty(error)) throw new Exception($"{GetType().Name}.RunCygwinCommand: Error: {error}");

            return result;

        }
        finally
        {
            isRunning = false;
        }

    }    
}