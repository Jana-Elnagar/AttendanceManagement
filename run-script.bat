@echo off
cd /d "%~dp0"
setlocal

REM === CONFIGURATION ===
set WAIT_INTERVAL=2

echo Starting .NET app...
start "DotNetApp" cmd /k "dotnet run --project "src\AttendanceManagement.HttpApi.Host""

echo Waiting for the app to start on port 44359...

:WAIT_LOOP
REM Check if the port is listening
netstat -an | find ":44359" | find "LISTENING" >nul
if %errorlevel%==0 (
    echo App is running on port 44359!
    goto AFTER_START
)

REM If not found yet, wait then retry
timeout /t %WAIT_INTERVAL% /nobreak >nul
goto WAIT_LOOP

:AFTER_START
echo Running the Blazor Project
start "DotNetBlazorApp" cmd /k "dotnet run --project "src\AttendanceManagement.Blazor""

echo Waiting for the app to start on port 44383...

:WAIT_LOOP_SECOND
REM Check if the port is listening
netstat -an | find ":44383" | find "LISTENING" >nul
if %errorlevel%==0 (
    echo App is running on port 44383!
    goto END
)

REM If not found yet, wait then retry
timeout /t %WAIT_INTERVAL% /nobreak >nul
goto WAIT_LOOP_SECOND

:END
echo Done!
endlocal