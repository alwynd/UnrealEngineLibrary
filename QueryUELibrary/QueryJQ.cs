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
    public async Task<QueryJQResult> RunCommand(string script, string userQuery)
    {
        QueryJQResult result = new QueryJQResult();
        
        if (isRunning) return result;
        isRunning = true;

        try
        {
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
            result.Result = await process.StandardOutput.ReadToEndAsync();
            result.Error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            Logging.Info($"{GetType().Name}.RunCommand: result length: {result.Result.Length}");
            Logging.Debug($"{GetType().Name}.RunCommand: result: {result.Result}");
            if (!string.IsNullOrEmpty(result.Error)) Logging.Info($"{GetType().Name}.RunCommand: error: {result.Error}");
            return result;
        }
        finally
        {
            isRunning = false;
        }
    }

    /// <summary>
    /// Represents the query results.
    /// </summary>
    public class QueryJQResult
    {
        /// <summary>
        /// The result string. 
        /// </summary>
        public string Result { get; set; } = string.Empty;
        
        /// <summary>
        /// Any exceptions.
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }
    
}