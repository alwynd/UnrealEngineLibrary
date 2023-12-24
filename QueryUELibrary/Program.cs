
namespace QueryUELibrary
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logging.Info($"{typeof(Program).Name}:-- START, starting up..");
            
            System.Windows.Forms.Application.ApplicationExit += (sender, e) =>
            {
                System.Console.WriteLine("Shutting down...");
            };            
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new QueryForm());
        }
        
        
        
    }
}