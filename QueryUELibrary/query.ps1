try {
    $scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
    $iniFilePath = Join-Path $scriptDir 'properties.ini'
    if (Test-Path $iniFilePath) {
        try {
            Add-Type -TypeDefinition @"
                using System.Runtime.InteropServices;
                using System.Text;
                public class IniFile {
                    [DllImport("kernel32")] public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
                }
"@
            $getPath = New-Object Text.StringBuilder(260)
            [IniFile]::GetPrivateProfileString("paths", "UELibraryFolder", "", $getPath, $getPath.Capacity, $iniFilePath) > $null
            $directory = $getPath.ToString()
            [Console]::Error.WriteLine("PS using INI File.")
        } catch {
            [Console]::Error.WriteLine("PS Error when reading INI File. Details: $($_.Exception.Message)")
        }
    } else {
        [Console]::Error.WriteLine("PS INI File not found.")
    }

    if(!$directory) {
        $directory = "../../../../UELibrary"
        [Console]::Error.WriteLine("PS NOT using INI (1).")
    }
} catch {
    $directory = "../../../../UELibrary"
    [Console]::Error.WriteLine("PS NOT using INI (2) Error. Details: $($_.Exception.Message)")
}

[Console]::Error.WriteLine("PS using Library folder: $directory")

$results = @()

Get-ChildItem -Path $directory -Filter *.json | ForEach-Object {
    $filePath = $_.FullName
    try {
        $json = Get-Content -Path $filePath | ConvertFrom-Json
        $matchedEntries = $json.PSObject.Properties.Value | 
        Where-Object {
            # example: $_.AssetPath -match "spruce" -and $_.AssetType -match "staticmesh"
            # example: $_.SizeOnDisk -gt 104857600
            # example: $_.AssetPath -match "combat.*sword" -and $_.AssetType -match "animation"
            {QUERY_JQ}
        }

        if ($matchedEntries) {
            $results += $matchedEntries
        }
    } catch {
        $errorMessage = "Error in file: $filePath"
        Write-Error $errorMessage 2>&1
    }
}

$results | ConvertTo-Json -Depth 10
