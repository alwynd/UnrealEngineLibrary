using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace QueryUELibrary;

/// <summary>
/// CSV Structure.
/// </summary>
public class UELibrary
{
    public string CloudProject { get; set; }
    public string Filler1 { get; set; }
    
    public string ContentFolder { get; set; }
    public string Filler2 { get; set; }
    
    public string Category { get; set; }
    public string Filler3 { get; set; }
    
    public string StoreProject { get; set; }
    public string URL { get; set; }
}


/// <summary>
/// CSV Helper.
/// </summary>
public static class UEProjectCSV
{

    /// <summary>
    /// The library projects.
    /// </summary>
    public static List<UELibrary> UELibraryProjects { get; private set; } = new List<UELibrary>();

    private static readonly string csvFile = "Asset Library.csv"; 
    
    /// <summary>
    /// Initialize this.
    /// </summary>
    public static void Initialize()
    {
        Logging.Debug($"{typeof(UEProjectCSV).Name}.Initialize:-- START, parsing CSV: {csvFile}");
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            HeaderValidated = null,
            MissingFieldFound = null
        };
        
        using var reader = new StreamReader(csvFile);
        using var csv = new CsvReader(reader, config);
        UELibraryProjects = csv.GetRecords<UELibrary>().ToList();
        Logging.Debug($"{typeof(UEProjectCSV).Name}.Initialize, csv: {csvFile}, found UELibraryProjects: {UELibraryProjects.Count}");
        
        // remove all where the URL is null.
        UELibraryProjects.RemoveAll(x => string.IsNullOrEmpty(x.URL));
        Logging.Debug($"{typeof(UEProjectCSV).Name}.Initialize, csv: {csvFile}, found UELibraryProjects: {UELibraryProjects.Count} with URLs.");
        
        // log summary
        UELibraryProjects.ForEach(x =>
        {
            Logging.Debug($"UE Project: {x.CloudProject}, ContentFolder: {x.ContentFolder}, URL: {x.URL}");
        });

    }    
}