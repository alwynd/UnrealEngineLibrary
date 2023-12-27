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
    /// Runs Powershell.
    /// </summary>
    public async Task<QueryJQResult> RunCommand(string script, string userQuery, Action<string> resultUpdated, Action<string> errorUpdated, Action completed)
    {
        Logging.Debug($"{GetType().Name}.RunCommand:-- START, script: {script}, userQuery: {userQuery}");
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

            // Write modifiedScript to a new PowerShell (.ps1) file
            string tempScriptFile = "_temp.ps1";
            await File.WriteAllTextAsync(tempScriptFile, modifiedScript);            
            Logging.Debug($"{GetType().Name}.RunCommand: tempScriptFile: {tempScriptFile}, modifiedScript: {modifiedScript}");

            // Set up the process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File {tempScriptFile}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            Logging.Debug($"{GetType().Name}.RunCommand: Running Command: powershell.exe -ExecutionPolicy Bypass -File {tempScriptFile}");
            
            using Process process = new Process { StartInfo = startInfo };

            // Execute the process asynchronously and read the output/error
            process.Start();

            // Setup tasks for streaming results and errors concurrently
            var resultTask = Task.Run(async () =>
            {
                var resultReader = process.StandardOutput;
                string resultLine;
                while ((resultLine = await resultReader.ReadLineAsync()) != null)
                {
                    result.Result += (result.Result.Length > 0 ? Environment.NewLine : "") + resultLine;
                    Logging.Info($"{GetType().Name}.RunCommand Output Received (Len): {resultLine.Length}");
                    Logging.Debug($"{GetType().Name}.RunCommand Output Received: {resultLine}");
                    resultUpdated(resultLine);
                }
            });

            var errorTask = Task.Run(async () =>
            {
                var errorReader = process.StandardError;
                string errorLine;
                while ((errorLine = await errorReader.ReadLineAsync()) != null)
                {
                    result.Error += (result.Error.Length > 0 ? Environment.NewLine : "") + errorLine; 
                    Logging.Info($"{GetType().Name}.RunCommand Error Received (Len): {errorLine.Length}");
                    Logging.Debug($"{GetType().Name}.RunCommand Error Received: {errorLine}");
                    errorUpdated(errorLine);
                }
            });

            // Wait for both tasks to be complete
            await Task.WhenAll(resultTask, errorTask);
            await process.WaitForExitAsync();

            Logging.Info($"{GetType().Name}.RunCommand: result length: {result.Result.Length}");
            Logging.Debug($"{GetType().Name}.RunCommand: result: {result.Result}");
            Logging.Info($"{GetType().Name}.RunCommand: error: {result.Error}");
            completed();
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