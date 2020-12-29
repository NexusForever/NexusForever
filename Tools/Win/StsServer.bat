@echo off
REM // ABOUT:   This script will auto restart the STS Server if it crashes.
REM // INSTALL: Copy this file to the same directory as NexusForever.StsServer.exe
REM // USAGE:   Open this file to launch the STS Server.
REM // AUTHOR:  Digitalroot <digitalroot@gmail.com>
cls
:start
NexusForever.StsServer.exe
goto start
