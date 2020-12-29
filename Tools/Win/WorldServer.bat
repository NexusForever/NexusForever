@echo off
REM // ABOUT:   This script will auto restart the World Server if it crashes.
REM // INSTALL: Copy this file to the same directory as NexusForever.WorldServer.exe
REM // USAGE:   Open this file to launch the World Server.
REM // AUTHOR:  Digitalroot <digitalroot@gmail.com>
cls
:start
NexusForever.WorldServer.exe
goto start
