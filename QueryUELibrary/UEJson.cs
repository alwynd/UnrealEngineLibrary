namespace QueryUELibrary;

/// <summary>
/// THe JSON UE stuff.
/// </summary>
public class UEJson
{
    /// <summary>
    /// THe Asset Path.
    /// </summary>
    public string AssetPath { get; set; }
    
    /// <summary>
    /// The asset sized on disk in bytes. 
    /// </summary>
    public long SizeOnDisk { get; set; }
    
    /// <summary>
    /// The asset type, as per Unreal Engine.
    /// </summary>
    public string AssetType { get; set; }

}