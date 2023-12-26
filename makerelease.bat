SETLOCAL ENABLEDELAYEDEXPANSION
SET ReleaseDir=Release
SET SourceDir=QueryUELibrary\bin\Release\net8.0-windows
SET dateStr=%date:~10,4%-%date:~4,2%-%date:~7,2%
SET zipFile=%ReleaseDir%\QueryUELibrary-%dateStr%.zip

:: Make/Clean the Release folder.
IF NOT EXIST %ReleaseDir% (
    MKDIR %ReleaseDir%
) ELSE (
    DEL /Q /F %ReleaseDir%\*.*
)

IF ERRORLEVEL 1 (
    echo Failed to clean the Release folder.
    pause
    exit /b 99
)

:: Copy the necessary files.
XCOPY /Y /S "%SourceDir%\*" %ReleaseDir%\

IF ERRORLEVEL 1 (
    echo Failed to copy files to the Release folder.
    pause
    exit /b 98
)

:: Run a PowerShell command to Zip it UP.
Powershell.exe -Command "
Add-Type -assembly 'system.io.compression.filesystem';
[io.compression.zipfile]::CreateFromDirectory('%CD%\%ReleaseDir%', '%zipFile%');"

IF ERRORLEVEL 1 (
    echo Failed to create a zip file.
    pause
    exit /b 97
)

echo Success!
exit /b 0
