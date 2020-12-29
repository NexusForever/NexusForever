@echo off
REM // ABOUT:   This script will auto restart the Auth Server if it crashes.
REM // INSTALL: Copy this file to the same directory as NexusForever.AuthServer.exe
REM // USAGE:   Open this file to launch the Auth Server.
REM // AUTHOR:  Digitalroot <digitalroot@gmail.com>
cls
:start
NexusForever.AuthServer.exe
goto start
