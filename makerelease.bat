SETLOCAL ENABLEDELAYEDEXPANSION
SET ReleaseDir=Release
SET SourceDir=QueryUELibrary\bin\Release\net8.0-windows
for /f "delims=" %%# in ('powershell get-date -format "{yyyy-MM-dd-HH-mm}"') do set dateStr=%%#
SET zipFile=QueryUELibrary-%dateStr%.zip

:: Make/Clean the Release folder.
IF NOT EXIST %ReleaseDir% (
    MKDIR %ReleaseDir%
) ELSE (
    DEL /Q /F %ReleaseDir%\*.*
)
IF ERRORLEVEL 1 (
    echo "Failed to clean the Release folder: %ReleaseDir%"
    pause
    exit /b 99
)

:: Copy the necessary files.
XCOPY /Y /S "%SourceDir%\*" %ReleaseDir%\
IF ERRORLEVEL 1 (
    echo "Failed to copy files(1) from %SourceDir% to the Release folder: %ReleaseDir%"
    pause
    exit /b 98
)

XCOPY /Y /S "ReleaseScripts\*" %ReleaseDir%\
IF ERRORLEVEL 1 (
    echo "Failed to copy files(2) from ReleaseScripts to the Release folder: %ReleaseDir%"
    pause
    exit /b 95
)


:: Run a PowerShell command to Zip it UP.
Powershell.exe -Command "& { Add-Type -assembly 'system.io.compression.filesystem'; [io.compression.zipfile]::CreateFromDirectory('%CD%\%ReleaseDir%', '%zipFile%'); }"
IF ERRORLEVEL 1 (
    echo "Failed to create the zip file: %zipFile%"
    pause
    exit /b 90
)

:: Delete copied files
DEL /Q /F %ReleaseDir%\*.*
IF EXIST %zipFile% (
    MOVE %zipFile% %ReleaseDir%\
)

echo Success!
exit /b 0
