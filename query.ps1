$directory = "UELibrary"

$results = @()

Get-ChildItem -Path $directory -Filter *.json | ForEach-Object {
    $filePath = $_.FullName
    try {
        $json = Get-Content -Path $filePath | ConvertFrom-Json
        $matchedEntries = $json.PSObject.Properties.Value | 
        Where-Object {
            # example: $_.AssetPath -match $keyword1 -and $_.AssetType -match $keyword2
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
