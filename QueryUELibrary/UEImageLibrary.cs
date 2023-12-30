using System.Collections.Concurrent;
using IniParser;
using IniParser.Model;

namespace QueryUELibrary;

/// <summary>
/// Provides the images.
/// </summary>
public class UEImageLibrary
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static readonly UEImageLibrary Instance = new UEImageLibrary();

    /// <summary>
    /// The UE Library path.
    /// </summary>
    public static string UELibraryPath { get; private set; } = defaultUELibraryPath; 

    /// <summary>
    /// The images.
    /// </summary>
    public ConcurrentDictionary<string, long> UELibraryImages { get; private set; } = new ConcurrentDictionary<string, long>();

    /// <summary>
    /// The imaghes without .png at the end.
    /// </summary>
    public HashSet<string> ImagePathsWithoutPng { get; private set; } = new();

    /// <summary>
    /// Initialized?
    /// </summary>
    public bool Initialized { get; private set; } = false;

    private static readonly string defaultUELibraryPath = "../../../../UELibrary";
    private static readonly string propertiesIni = "properties.ini";
    private bool inProgress = false;
    

    /// <summary>
    /// Private constructor.
    /// </summary>
    private UEImageLibrary() { }

    /// <summary>
    /// Initializes this.
    /// </summary>
    public async Task Initialize(Action<int> progress, Action complete)
    {
        if (inProgress) return;
        inProgress = true;

        try
        {
            Initialized = false;
            Logging.Debug($"{GetType().Name}.Initialize:-- START");
            var parser = new FileIniDataParser();
            if (File.Exists(propertiesIni))
            {
                IniData data = parser.ReadFile(propertiesIni);
                UELibraryPath = data["paths"]["UELibraryFolder"];
                if (string.IsNullOrWhiteSpace(UELibraryPath))
                {
                    UELibraryPath = defaultUELibraryPath;
                }
            }
            Logging.Debug($"{GetType().Name}.Initialize UELibraryPath: {UELibraryPath}, Exists: {Directory.Exists(UELibraryPath)}");
            if (!Directory.Exists(UELibraryPath))
            {
                throw new Exception($"UELibrary path: '{UELibraryPath}' does not exist.");
            }
            
            var allFiles = Directory.EnumerateFiles(UELibraryPath, "*.png", SearchOption.AllDirectories);
            var fileBatches = allFiles
                .Select((x, i) => new { File = x, Index = i })
                .GroupBy(x => x.Index / 512)  // adjust the batch size according to your performance requirement.
                .Select(g => g.Select(x => x.File));

            Parallel.ForEach(fileBatches, (files) => 
            {
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    UELibraryImages.TryAdd(file, fileInfo.Length);
                }
                progress?.Invoke(UELibraryImages.Count);
            });            
            
            // Prepare a HashSet out of the keys for a fast look-up. 
            // Remove .png and convert to lower case
            ImagePathsWithoutPng = new HashSet<string>(UEImageLibrary.Instance.UELibraryImages.Keys.Select(key => key.ToLower().Replace("\\", "/").Replace(".png", "")));
            
            Logging.Debug($"{GetType().Name}.Initialize UELibraryPath: {UELibraryPath}, UELibraryImages: {UELibraryImages.Count}");
            Initialized = true;
            complete?.Invoke();
        }
        finally
        {
            inProgress = false;
        }
    }
    
}