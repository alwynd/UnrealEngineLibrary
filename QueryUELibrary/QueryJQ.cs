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
    /// Runs JQ via Cygwin.
    /// </summary>
    public async Task<string> RunCommand(string script, string userQuery)
    {
        if (isRunning) return string.Empty;
        isRunning = true;

        try
        {
            string result = string.Empty;
            string error = string.Empty;

            // Read the PowerShell script template
            // Replace the placeholder with the actual query
            // Convert the script to a PowerShell script block
            string scriptTemplate = File.ReadAllText(script);
            string modifiedScript = scriptTemplate.Replace("{QUERY_JQ}", userQuery);
            string scriptBlock = $"& {{{modifiedScript}}}";

            // Set up the process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -Command {scriptBlock}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process process = new Process { StartInfo = startInfo };

            // Execute the process asynchronously and read the output/error
            process.Start();
            result = await process.StandardOutput.ReadToEndAsync();
            error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error)) 
                throw new Exception($"{GetType().Name}.RunCommand: Error: {error}");

            Logging.Info($"{GetType().Name}.RunCommand: result length: {result.Length}");
            Logging.Debug($"{GetType().Name}.RunCommand: result: {result}");
            return result;
        }
        finally
        {
            isRunning = false;
        }
    }
    
     
}