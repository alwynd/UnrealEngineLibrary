$directory = "UELibrary"

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
